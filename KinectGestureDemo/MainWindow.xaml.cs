using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Linq;
using System.Windows.Input;
using Kinect.Gestures;
using Kinect.Gestures.Waves;

namespace KinectGestureDemo
{
    public partial class MainWindow : Window
    {
        //Instantiate the Kinect runtime. Required to initialize the device.
        //IMPORTANT NOTE: You can pass the device ID here, in case more than one Kinect device is connected.
        KinectSensor sensor = KinectSensor.KinectSensors[0];
        Skeleton[] skeletons;
        KinectGestureController gestureController;
        int gestureCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            //Runtime initialization is handled when the window is opened. When the window
            //is closed, the runtime MUST be unitialized.
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
            sensor.SkeletonStream.Enable();
        }

        void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (sensor.ElevationAngle <= 22)
                {
                    sensor.ElevationAngle += 5;
                }
            }
            else if (e.Key == Key.Down)
            {
                if (sensor.ElevationAngle >= -22)
                {
                    sensor.ElevationAngle -= 5;
                }
            }
        }

        void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool receivedData = false;

            using (SkeletonFrame SFrame = e.OpenSkeletonFrame())
            {
                if (SFrame == null)
                {
                    // The image processing took too long. More than 2 frames behind.
                }
                else
                {
                    skeletons = new Skeleton[SFrame.SkeletonArrayLength];
                    SFrame.CopySkeletonDataTo(skeletons);
                    receivedData = true;
                }
            }

            if (receivedData)
            {

                Skeleton currentSkeleton = (from s in skeletons
                                            where s.TrackingState == SkeletonTrackingState.Tracked
                                            select s).FirstOrDefault();

                if (currentSkeleton != null)
                {
                    SetEllipsePosition(head, currentSkeleton.Joints[JointType.Head]);
                    SetEllipsePosition(leftHand, currentSkeleton.Joints[JointType.HandLeft]);
                    SetEllipsePosition(rightHand, currentSkeleton.Joints[JointType.HandRight]);
                    
                    // Update gestures.
                    this.gestureController.UpdateGestures(currentSkeleton);
                }
            }
        }

        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {
            Microsoft.Kinect.SkeletonPoint vector = new Microsoft.Kinect.SkeletonPoint();
            vector.X = ScaleVector(800, joint.Position.X);
            vector.Y = ScaleVector(600, -joint.Position.Y);
            vector.Z = joint.Position.Z;

            Joint updatedJoint = new Joint();
            updatedJoint = joint;
            updatedJoint.TrackingState = JointTrackingState.Tracked;
            updatedJoint.Position = vector;

            Canvas.SetLeft(ellipse, updatedJoint.Position.X);
            Canvas.SetTop(ellipse, updatedJoint.Position.Y);
        }

        private float ScaleVector(int length, float position)
        {
            float value = (((((float)length) / 1f) / 2f) * position) + (length / 2);
            if (value > length)
            {
                return (float)length;
            }
            if (value < 0f)
            {
                return 0f;
            }
            return value;
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            sensor.Stop();
        }

        void SetupKinectSensor()
        {
            // Enable near mode.
            sensor.DepthStream.Range = DepthRange.Near;
            sensor.SkeletonStream.EnableTrackingInNearRange = true;

            // Enable seated mode.
            sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;

            // Register skeleton frame ready callback.
            sensor.SkeletonFrameReady += runtime_SkeletonFrameReady;
        }

        void SetupGestureController()
        {
            // Create the gesture controller.
            this.gestureController = new KinectGestureController();

            // Add new gestures.
            this.gestureController.AddGesture(new KinectGestureWaveRightHand());
            this.gestureController.AddGesture(new KinectGestureWaveLeftHand());

            // Register success callback.
            this.gestureController.KinectGestureRecognized += new EventHandler<KinectGestureEventArgs>(GestureRegognized);
        }

        private void GestureRegognized(object sender, KinectGestureEventArgs e)
        {
            // Update the gesture count.
            this.gestureCount++;

            switch (e.GestureType)
            {
                case KinectGestureType.SwipeBottomToTop:
                    gestureLabel.Content = this.gestureCount + " : Swipe Bottom to Up";
                    break;
                case KinectGestureType.SwipeLeftToRight:
                    gestureLabel.Content = this.gestureCount + " : Swipe Left to Right";
                    break;
                case KinectGestureType.SwipeRightToLeft:
                    gestureLabel.Content = this.gestureCount + " : Swipe Right to Left";
                    break;
                case KinectGestureType.SwipeTopToBottom:
                    gestureLabel.Content = this.gestureCount + " : Swipe Top to Bottom";
                    break;
                case KinectGestureType.WaveLeftHand:
                    gestureLabel.Content = this.gestureCount + " : Wave Left Hand";
                    break;
                case KinectGestureType.WaveRightHand:
                    gestureLabel.Content = this.gestureCount + " : Wave Right Hand";
                    break;
                default:
                    break;
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Setup sensor.
            this.SetupKinectSensor();

            // Register sensor tilt callback.
            this.KeyUp += MainWindow_KeyUp;

            // Setup gestures.
            this.SetupGestureController();

            // Start sensor.
            this.sensor.Start();
        }
    }
}
