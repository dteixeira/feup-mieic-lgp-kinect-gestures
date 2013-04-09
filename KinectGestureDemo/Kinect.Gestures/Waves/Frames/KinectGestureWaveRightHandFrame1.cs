﻿using Microsoft.Kinect;

namespace Kinect.Gestures.Waves.Frames
{
    /// <summary>
    /// Describes the first frame of a right hand wave.
    /// </summary>
    public class KinectGestureWaveRightHandFrame1 : IKinectGestureFrame
    {
        public KinectGestureResult ProcessFrame(Skeleton skeleton)
        {
            // Checks if the right hand is above the right elbow.
            if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y)
            {
                // Checks if the right hand is at the right of the right elbow.
                if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ElbowRight].Position.X)
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