using Microsoft.Kinect;

namespace Kinect.Gestures.Circles.Frames
{
    /// <summary>
    /// Describes the first frame of a left hand wave.
    /// </summary>
    public class KinectGestureCircleRightHandFrame1 : IKinectGestureFrame
    {
        public KinectGestureResult ProcessFrame(Skeleton skeleton)
        {
            // Checks if the right hand is above of the right elbow
            if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y)
            {
                // Checks if the right hand is right of the right elbow
                if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ElbowRight].Position.X)
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