using Microsoft.Kinect.Toolkit.Interaction;

namespace Kinect.Pointers
{
    public class KinectPointerHand
    {
        private double x;
        private double y;
        private InteractionHandEventType handEventType;

        public KinectPointerHand()
        {
        }

        public InteractionHandEventType HandEventType
        {
            get { return this.handEventType; }
            set { this.handEventType = value; }
        }

        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        public KinectPointerHand(int x, int y, InteractionHandEventType handEventType)
        {
            this.x = x;
            this.y = y;
            this.handEventType = handEventType;
        }
    }
}
