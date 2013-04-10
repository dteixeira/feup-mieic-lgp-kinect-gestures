using Microsoft.Kinect;

namespace Kinect.Gestures.Swipes.Frames
{
    /// <summary>
    /// Describes the first frame of a left hand wave.
    /// </summary>
    public class KinectGestureSwipeBottomToTopFrame3 : IKinectGestureFrame
    {
        public KinectGestureResult ProcessFrame(Skeleton skeleton)
        {
            // Checks if right hand right of the right shoulder.
            if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderRight].Position.X)
            {
                // Checks if right hand is above the right elbow.
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y)
                {
                    // Checks if the right hand is above the head.
                    if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.Head].Position.Y)
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