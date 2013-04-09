namespace Kinect.Gestures.Swipes
{
    /// <summary>
    /// This classes is used to check for a swipe gesture, from right to left.
    /// </summary>
    public class KinectGestureSwipeRightToLeft : KinectGesture
    {
        /// <summary>
        /// Creates a new swipe right to left gesture.
        /// </summary>
        public KinectGestureSwipeRightToLeft()
            : base(KinectGestureType.None, null)
        {
            this.gestureType = KinectGestureType.SwipeRightToLeft;
            this.gestureFrames = this.CreateGestureFrames();
        }

        /// <summary>
        /// Creates the frame sequence that defines the gesture.
        /// </summary>
        /// <returns>Gesture's frame sequence</returns>
        private IKinectGestureFrame[] CreateGestureFrames()
        {
            // TODO Finish this.
            return null;
        }
    }
}