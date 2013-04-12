using Microsoft.Kinect;

namespace Kinect.Gestures.Circles.Frames
{
    /// <summary>
    /// Describes the first frame of a left hand wave.
    /// </summary>
    public class KinectGestureCircleLeftHandFrame1 : IKinectGestureFrame
    {
        public KinectGestureResult ProcessFrame(Skeleton skeleton)
        {
            // Checks if the left hand is above of the left elbow
            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y)
            {
                // Checks if the left hand is left of the left elbow
                if (skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ElbowLeft].Position.X)
                {
                    // The first part of the gesture was completed.
                    return KinectGestureResult.Success;
                }

                // The standard result is failure.
                return KinectGestureResult.Waiting;
            }

            // The standard result is failure.
            return KinectGestureResult.Fail;
        }
    }
}