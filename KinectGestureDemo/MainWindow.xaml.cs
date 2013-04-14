using Kinect.Gestures;
using Kinect.Pointers;
using Kinect.Gestures.Waves;
using Kinect.Gestures.Swipes;
using Kinect.Gestures.Circles;
using Kinect.Sensor;
using Microsoft.Kinect;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Kinect.Toolkit.Interaction;

namespace KinectGestureDemo
{
    public partial class MainWindow : Window
    {
        private const double JointThickness = 5;
        private const float RenderHeight = 480.0f;
        private const float RenderWidth = 640.0f;
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 2);
        private readonly Brush inferredJointBrush = Brushes.IndianRed;
        private readonly Pen trackedBonePen = new Pen(Brushes.Indigo, 6);
        private readonly Brush trackedJointBrush = Brushes.SkyBlue;
        private DrawingGroup drawingGroup;
        private int gestureCount = 0;
        private DrawingImage imageSource;
        private Skeleton[] skeletons;
        private int trackingId = -1;
        private KinectSensorController sensorController;

        /// <summary>
        /// Creates the demo window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
        }

        /// <summary>
        /// Draws a bone line between two joints.
        /// </summary>
        /// <param name="skeleton">Skeleton to draw bones from</param>
        /// <param name="drawingContext">Drawing context to draw to</param>
        /// <param name="jointType0">Joint to start drawing from</param>
        /// <param name="jointType1">Joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit.
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred.
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked.
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            // Draw line between the two joints.
            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        /// <summary>
        /// Draws a skeleton's bones and joints.
        /// </summary>
        /// <param name="skeleton">Skeleton to draw</param>
        /// <param name="drawingContext">Drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render head and neck.
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);

            /* Unused joints.
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
             */

            // Render left arm.
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Render right arm.
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Render left leg
            /* Unused joints.
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);
             */

            // Render right leg
            /* Unused joints.
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);
             */

            // Render joints.
            var joints =
                from j in skeleton.Joints
                let t = j.JointType
                where t == JointType.Head || t == JointType.ShoulderCenter || t == JointType.ShoulderLeft ||
                    t == JointType.ShoulderRight || t == JointType.ElbowRight || t == JointType.ElbowLeft ||
                    t == JointType.HandRight || t == JointType.HandLeft || t == JointType.Spine ||
                    t == JointType.HipCenter || t == JointType.WristLeft || t == JointType.WristRight
                select j;
            foreach (Joint joint in joints)
            {
                Brush drawBrush = null;

                // Use a different brush for tracked and inferred joints.
                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    // Draw the joint, converting the rendered point position to a screen
                    // position, taking into account the distance between the user and the sensor.
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);

                    // Draw Vidal's face if joint is the head.
                    if (joint.JointType == JointType.Head)
                    {
                        Point head = this.SkeletonPointToScreen(joint.Position);
                        Canvas.SetLeft(this.Vidal, head.X + Vidal.Width / 2.0);
                        Canvas.SetTop(this.Vidal, head.Y + Vidal.Height / 2.0);
                    }
                }
            }
        }

        /// <summary>
        /// Callback for "gesture recognized" events.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void GestureRegognized(object sender, KinectGestureEventArgs e)
        {
            // Update the gesture count.
            this.gestureCount++;

            // Do a different action depending on gesture type.
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
                    if (this.trackingId == -1)
                    {
                        gestureLabel.Content = this.gestureCount + " : Wave Right Hand | ENGAGE";
                        this.trackingId = e.TrackingId;
                        this.sensorController.StartTrackingSkeleton(this.trackingId);
                    }
                    else
                    {
                        gestureLabel.Content = this.gestureCount + " : Wave Right Hand | DISENGAGE";
                        this.trackingId = -1;
                        this.sensorController.StopTrackingSkeleton();
                    }
                    break;

                case KinectGestureType.CircleRightHand:
                    gestureLabel.Content = this.gestureCount + " : Circle Right Hand";
                    break;

                case KinectGestureType.CircleLeftHand:
                    gestureLabel.Content = this.gestureCount + " : Circle Left Hand";
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Callback for KeyUp events.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            // If the up arrow was pressed, and the sensor is not already at the
            // max tilt, increase its tilt by 5 degrees.
            if (e.Key == Key.Up)
            {
                if (this.sensorController.Sensor.ElevationAngle <= 22)
                {
                    this.sensorController.Sensor.ElevationAngle += 5;
                }
            }

            // If the down arrow was pressed, and the sensor is not already at the
            // minimum tilt, decrease its tilt by 5 degrees.
            else if (e.Key == Key.Down)
            {
                if (this.sensorController.Sensor.ElevationAngle >= -22)
                {
                    this.sensorController.Sensor.ElevationAngle -= 5;
                }
            }
        }

        /// <summary>
        /// Callback for the Loaded event. Initialize all important data and configure
        /// the sensor.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            // Register sensor tilt callback.
            this.KeyUp += MainWindow_KeyUp;

            // Setup sensor.
            this.sensorController = new KinectSensorController();
            if (!this.sensorController.FoundSensor())
            {
                this.Close();
                return;
            }

            // Setup gestures.
            this.sensorController.Gestures.AddGesture(new KinectGestureWaveRightHand());
            this.sensorController.Gestures.AddGesture(new KinectGestureWaveLeftHand());
            this.sensorController.Gestures.AddGesture(new KinectGestureSwipeRightToLeft());
            this.sensorController.Gestures.AddGesture(new KinectGestureSwipeLeftToRight());
            this.sensorController.Gestures.AddGesture(new KinectGestureCircleRightHand());
            this.sensorController.Gestures.AddGesture(new KinectGestureCircleLeftHand());
            this.sensorController.Gestures.KinectGestureRecognized += new EventHandler<KinectGestureEventArgs>(GestureRegognized);

            // Setup pointer controller.
            this.sensorController.Pointers.KinectPointerMoved += new EventHandler<KinectPointerEventArgs>(this.PointerMoved);

            // Register skeleton frame ready callback;
            this.sensorController.Sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(runtime_SkeletonFrameReady);

            // Start the sensor.
            this.sensorController.StartSensor();
        }

        private void PointerMoved(object sender, KinectPointerEventArgs e)
        {
            // Position right hand.
            if (e.RightHand.HandEventType == InteractionHandEventType.GripRelease)
            {
                this.RightHand.Fill = Brushes.SkyBlue;
                this.RightHand.Stroke = Brushes.DarkBlue;
            }
            else if (e.RightHand.HandEventType == InteractionHandEventType.Grip)
            {
                this.RightHand.Fill = Brushes.IndianRed;
                this.RightHand.Stroke = Brushes.DarkRed;
            }
            Canvas.SetLeft(this.RightHand, e.RightHand.X * 800);
            Canvas.SetTop(this.RightHand, e.RightHand.Y * 600);

            // Position left hand.
            if (e.LeftHand.HandEventType == InteractionHandEventType.GripRelease)
            {
                this.LeftHand.Fill = Brushes.SkyBlue;
                this.LeftHand.Stroke = Brushes.DarkBlue;
            }
            else if (e.LeftHand.HandEventType == InteractionHandEventType.Grip)
            {
                this.LeftHand.Fill = Brushes.IndianRed;
                this.LeftHand.Stroke = Brushes.DarkRed;
            }
            Canvas.SetLeft(this.LeftHand, e.LeftHand.X * 800);
            Canvas.SetTop(this.LeftHand, e.LeftHand.Y * 600);
        }

        /// <summary>
        /// Callback for the Unloaded event. Stops the sensor.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.sensorController.StopSensor();
        }

        /// <summary>
        /// Callback for SkeletonFrameReady event. Draws the tracked skeleton
        /// and updates the gesture recognition
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
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
                // Gets the first tracked skeleton.
                Skeleton currentSkeleton = (from s in skeletons
                                            where s.TrackingState == SkeletonTrackingState.Tracked
                                            select s).FirstOrDefault();

                if (currentSkeleton != null)
                {
                    using (DrawingContext dc = this.drawingGroup.Open())
                    {
                        // Draw a transparent background to set the render size.
                        dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                        // Draw the skeleton bones and joints.
                        this.DrawBonesAndJoints(currentSkeleton, dc);

                        // prevent drawing outside of our render area
                        this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                    }
                }
                else
                {
                    using (DrawingContext dc = this.drawingGroup.Open())
                    {
                        // Clear screen.
                        dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                        Canvas.SetLeft(this.Vidal, -100);
                        Canvas.SetTop(this.Vidal, -100);
                    }
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.
            DepthImagePoint depthPoint = this.sensorController.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
    }
}