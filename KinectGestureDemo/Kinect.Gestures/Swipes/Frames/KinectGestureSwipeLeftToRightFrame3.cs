using Microsoft.Kinect;

namespace Kinect.Gestures.Swipes.Frames
{
    /// <summary>
    /// Describes the first frame of a left hand wave.
    /// </summary>
    public class KinectGestureSwipeLeftToRightFrame3 : IKinectGestureFrame
    {
        public KinectGestureResult ProcessFrame(Skeleton skeleton)
        {
            // Checks if left hand is below the left shoulder.
            if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y)
            {
                // Checks if left hand is above the hip
                if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
                {
                    // Checks if the left hand is at the right of the neck.
                    if (skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ShoulderCenter].Position.X)
                    {
                        // The first part of the gesture was completed.
                        return KinectGestureResult.Success;
                    }

                    // Gesture was not completed, but it's still possible to achieve.
                    else
                    {
                        // Will pause recognition and try later.
                        return KinectGestureResult.Waiting;
                    }
                }

                // The standard result is failure.
                return KinectGestureResult.Fail;
            }

            // The standard result is failure.
            return KinectGestureResult.Fail;
        }
    }
}