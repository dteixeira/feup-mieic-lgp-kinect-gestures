using Microsoft.Kinect.Toolkit.Interaction;

namespace Kinect.Pointers
{
    /// <summary>
    /// This is a dummy class that is needed to configure the interaction stream.
    /// </summary>
    public class KinectPointerInteractionClient : IInteractionClient
    {
        /// <summary>
        /// Creates an instance of an interaction stream client.
        /// </summary>
        public KinectPointerInteractionClient()
        {
        }

        /// <summary>
        /// Dummy interface method.
        /// </summary>
        /// <param name="skeletonTrackingId">TrackingId of a specific skeleton</param>
        /// <param name="handType">Type of hand event that was triggered</param>
        /// <param name="x">Horizontal position of the hand</param>
        /// <param name="y">Vertical position of the hand</param>
        /// <returns>Always return a dummy InteractionInfo instance</returns>
        public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
        {
            // Returns dummy result.
            return new InteractionInfo();
        }
    }
}