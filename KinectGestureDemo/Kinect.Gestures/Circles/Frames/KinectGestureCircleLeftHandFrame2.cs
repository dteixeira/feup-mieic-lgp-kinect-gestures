using Microsoft.Kinect;

namespace Kinect.Gestures.Circles.Frames
{
    /// <summary>
    /// Describes the first frame of a left hand wave.
    /// </summary>
    public class KinectGestureCircleLeftHandFrame2 : IKinectGestureFrame
    {
        public KinectGestureResult ProcessFrame(Skeleton skeleton)
        {
            // Checks if the left hand is left of the left elbow
            if (skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ElbowLeft].Position.X)
            {
                // Checks if the left hand is below the left elbow
                if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ElbowLeft].Position.Y)
                {
                    // The first part of the gesture was completed.
                    return KinectGestureResult.Success;
                }
                else
                {
                    // Gesture recognition will pause.
                    return KinectGestureResult.Waiting;
                }
            }

            // The standard result is failure.
            return KinectGestureResult.Fail;
        }
    }
}