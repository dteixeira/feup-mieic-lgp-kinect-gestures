using System;

namespace Kinect.Pointers
{
    /// <summary>
    /// This class is used to pass arguments to the callback methods, when an hand
    /// event is triggered.
    /// </summary>
    public class KinectPointerEventArgs : EventArgs
    {
        private KinectPointerHand leftHand;
        private KinectPointerHand rightHand;
        private int trackingId;

        /// <summary>
        /// Creates a new instance of the hand event arguments.
        /// </summary>
        /// <param name="trackingId">TrackingId of the requested skeleton</param>
        /// <param name="leftHand">Information relative to the skeleton's left hand</param>
        /// <param name="rightHand">Information relative to the skeleton's right hand</param>
        public KinectPointerEventArgs(int trackingId, KinectPointerHand leftHand, KinectPointerHand rightHand)
        {
            this.trackingId = trackingId;
            this.leftHand = leftHand;
            this.rightHand = rightHand;
        }

        /// <summary>
        /// Left hand information.
        /// </summary>
        public KinectPointerHand LeftHand
        {
            get { return this.leftHand; }
            set { this.leftHand = value; }
        }

        /// <summary>
        /// Right hand information.
        /// </summary>
        public KinectPointerHand RightHand
        {
            get { return this.rightHand; }
            set { this.rightHand = value; }
        }

        /// <summary>
        /// TrackingId of the skeleton.
        /// </summary>
        public int TrackingId
        {
            get { return this.trackingId; }
            set { this.trackingId = value; }
        }
    }
}