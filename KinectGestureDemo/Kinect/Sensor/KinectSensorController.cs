using Kinect.Gestures;
using Kinect.Pointers;
using Microsoft.Kinect;

namespace Kinect.Sensor
{
    /// <summary>
    /// This class manages the Kinect sensor, and interaction with the
    /// gestures and pointer controllers.
    /// </summary>
    public class KinectSensorController
    {
        private KinectGestureController gestureController;
        private KinectPointerController pointerController;
        private KinectSensor sensor;
        private int trackingId;

        /// <summary>
        /// Builds a new sensor controller instance.
        /// </summary>
        public KinectSensorController()
        {
            // Select a kinect sensor, if one is available.
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors[0];
                this.trackingId = -1;
                this.ConfigureSensor();
                this.ConfigureGestureController();
                this.ConfigurePointerController();
            }
        }

        /// <summary>
        /// Gesture controller instance.
        /// </summary>
        public KinectGestureController Gestures
        {
            get { return this.gestureController; }
        }

        /// <summary>
        /// Pointer controller instance.
        /// </summary>
        public KinectPointerController Pointers
        {
            get { return this.pointerController; }
        }

        /// <summary>
        /// Kinect sensor intance.
        /// </summary>
        public KinectSensor Sensor
        {
            get { return this.sensor; }
        }

        /// <summary>
        /// Checks if a sensor was found.
        /// </summary>
        /// <returns>true if a sensor was found, false otherwise</returns>
        public bool FoundSensor()
        {
            return this.sensor != null;
        }

        /// <summary>
        /// Start the sensor.
        /// </summary>
        public void StartSensor()
        {
            if (this.sensor != null)
            {
                this.sensor.Start();
            }
        }

        /// <summary>
        /// Set a specific skeleton to track.
        /// </summary>
        /// <param name="trackingId">TrackingId of the skeleton</param>
        public void StartTrackingSkeleton(int trackingId)
        {
            this.trackingId = trackingId;
            this.sensor.SkeletonStream.AppChoosesSkeletons = true;
            this.sensor.SkeletonStream.ChooseSkeletons(this.trackingId);
        }

        /// <summary>
        /// Stop the sensor.
        /// </summary>
        public void StopSensor()
        {
            if (this.sensor != null)
            {
                this.sensor.SkeletonStream.Disable();
                this.sensor.DepthStream.Disable();
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Stop tracking a specific skeleton.
        /// </summary>
        public void StopTrackingSkeleton()
        {
            this.trackingId = -1;
            this.sensor.SkeletonStream.AppChoosesSkeletons = false;
        }

        /// <summary>
        /// Processes sensor information and updates the gesture and
        /// pointer controllers.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">All frames ready event parameters</param>
        private void callback_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // Updates the pointer controller.
            this.pointerController.UpdatePointer(e, this.sensor.AccelerometerGetCurrentReading(), this.trackingId);

            // Update the gesture controller.
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                // If null, the frame is already too late.
                if (skeletonFrame == null)
                {
                    return;
                }

                // Copy skeleton data.
                Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(skeletons);

                // Process the closest tracking skeleton.
                if (this.trackingId == -1)
                {
                    double closestDistance = double.MaxValue;
                    Skeleton closestSkeleton = null;

                    foreach (Skeleton skeleton in skeletons)
                    {
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked && skeleton.Position.Z < closestDistance)
                        {
                            closestDistance = skeleton.Position.Z;
                            closestSkeleton = skeleton;
                        }
                    }

                    if (closestSkeleton != null)
                    {
                        this.gestureController.UpdateGestures(closestSkeleton);
                    }
                }

                // Process only one skeleton.
                else
                {
                    foreach (Skeleton skeleton in skeletons)
                    {
                        // Process the skeleton if it is found and then return.
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked && skeleton.TrackingId == this.trackingId)
                        {
                            this.gestureController.UpdateGestures(skeleton);
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Configures the gesture controller.
        /// </summary>
        private void ConfigureGestureController()
        {
            this.gestureController = new KinectGestureController();
        }

        /// <summary>
        /// Configures the pointer controller.
        /// </summary>
        private void ConfigurePointerController()
        {
            this.pointerController = new KinectPointerController(this.sensor);
        }

        /// <summary>
        /// Configures the Kinect sensor.
        /// </summary>
        private void ConfigureSensor()
        {
            // Enable near mode.
            this.sensor.DepthStream.Range = DepthRange.Near;
            this.sensor.SkeletonStream.EnableTrackingInNearRange = true;

            // Disable specific skeleton tracking.
            this.sensor.SkeletonStream.AppChoosesSkeletons = false;

            // Enable needed streams.
            this.sensor.SkeletonStream.Enable();
            this.sensor.DepthStream.Enable();

            // Register all frames ready callback.
            this.sensor.AllFramesReady += callback_AllFramesReady;
        }
    }
}