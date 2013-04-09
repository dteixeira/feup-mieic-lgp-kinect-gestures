namespace Kinect.Gestures.Swipes
{
    /// <summary>
    /// This classes is used to check for a swipe gesture, from bottom to top.
    /// </summary>
    public class KinectGestureSwipeBottomToTop : KinectGesture
    {
        /// <summary>
        /// Creates a new swipe bottom to top gesture.
        /// </summary>
        public KinectGestureSwipeBottomToTop()
            : base(KinectGestureType.None, null)
        {
            this.gestureType = KinectGestureType.SwipeBottomToTop;
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