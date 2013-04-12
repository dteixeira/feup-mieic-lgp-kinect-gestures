using Microsoft.Kinect.Toolkit.Interaction;

namespace Kinect.Pointers
{
    public class KinectPointerInteractionClient : IInteractionClient
    {
        public KinectPointerInteractionClient()
        {
        }

        public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
        {
            // Returns dummy result.
            return new InteractionInfo();
        }
    }
}
