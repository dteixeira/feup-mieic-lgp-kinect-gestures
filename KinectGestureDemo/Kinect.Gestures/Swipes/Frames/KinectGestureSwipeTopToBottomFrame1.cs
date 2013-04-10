using Microsoft.Kinect;

namespace Kinect.Gestures.Swipes.Frames
{
    /// <summary>
    /// Describes the first frame of a left hand wave.
    /// </summary>
    public class KinectGestureSwipeTopToBottomFrame1 : IKinectGestureFrame
    {
        public KinectGestureResult ProcessFrame(Skeleton skeleton)
        {
            // Checks if left hand left of the left shoulder.
            if (skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderLeft].Position.X)
            {
                // Checks if left hand is below the left shoulder.
                if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y)
                {
                    // Checks if the left hand is below the left elbow.
                    if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ElbowLeft].Position.Y)
                    {
                        // The first part of the gesture was completed.
                        return KinectGestureResult.Success;
                    }
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