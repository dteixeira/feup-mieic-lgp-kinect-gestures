using System;
using System.Collections.Generic;

namespace Kinect.Pointers
{
    public class KinectPointerEventArgs : EventArgs
    {
        private int trackingId;
        private KinectPointerHand leftHand;
        private KinectPointerHand rightHand;

        public KinectPointerEventArgs(int trackingId, KinectPointerHand leftHand, KinectPointerHand rightHand)
        {
            this.trackingId = trackingId;
            this.leftHand = leftHand;
            this.rightHand = rightHand;
        }

        public int TrackingId
        {
            get { return this.trackingId; }
            set { this.trackingId = value; }
        }

        public KinectPointerHand LeftHand
        {
            get { return this.leftHand; }
            set { this.leftHand = value; }
        }

        public KinectPointerHand RightHand
        {
            get { return this.rightHand; }
            set { this.rightHand = value; }
        }
    }
}