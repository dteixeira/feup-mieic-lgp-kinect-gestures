using Microsoft.Kinect;

namespace Kinect.Gestures.Waves.Frames
{
    public class KinectGestureWaveLeftHandFrame1 : IKinectGestureFrame
    {
        public KinectGestureResult ProcessFrame(Skeleton skeleton)
        {
            // Checks if the left hand is above the left elbow.
            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y)
            {
                // Checks if the left hand is at the left of the left elbow.
                if (skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ElbowLeft].Position.X)
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
    }
}
