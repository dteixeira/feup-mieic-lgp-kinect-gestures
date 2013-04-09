namespace Kinect.Gestures.Swipes
{
    /// <summary>
    /// This classes is used to check for a swipe gesture, from top to bottom.
    /// </summary>
    public class KinectGestureSwipeTopToBottom : KinectGesture
    {
        /// <summary>
        /// Creates a new swipe top to bottom gesture.
        /// </summary>
        public KinectGestureSwipeTopToBottom()
            : base(KinectGestureType.None, null)
        {
            this.gestureType = KinectGestureType.SwipeTopToBottom;
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