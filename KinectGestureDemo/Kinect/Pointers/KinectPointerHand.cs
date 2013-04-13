using Microsoft.Kinect.Toolkit.Interaction;

namespace Kinect.Pointers
{
    /// <summary>
    /// This class is used to store the information about a specific hand, triggered
    /// by a hand event.
    /// </summary>
    public class KinectPointerHand
    {
        private InteractionHandEventType handEventType;
        private double x;
        private double y;

        /// <summary>
        /// Creates a new hand.
        /// </summary>
        public KinectPointerHand()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="x">Horizontal position of the hand</param>
        /// <param name="y">Vertical position of the hand</param>
        /// <param name="handEventType">Type of the triggered hand event</param>
        public KinectPointerHand(int x, int y, InteractionHandEventType handEventType)
        {
            this.x = x;
            this.y = y;
            this.handEventType = handEventType;
        }

        /// <summary>
        /// Type of the triggered hand event.
        /// </summary>
        public InteractionHandEventType HandEventType
        {
            get { return this.handEventType; }
            set { this.handEventType = value; }
        }

        /// <summary>
        /// Horizontal position of the hand.
        /// </summary>
        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        /// <summary>
        /// Vertical position of the hand.
        /// </summary>
        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }
    }
}