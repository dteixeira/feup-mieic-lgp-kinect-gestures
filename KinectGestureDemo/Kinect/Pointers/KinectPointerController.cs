using System;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;

namespace Kinect.Pointers
{
    public class KinectPointerController
    {
        private InteractionStream interactionStream;
        private IInteractionClient interactionClient;
        private int trackingId;

        public event EventHandler<KinectPointerEventArgs> KinectPointerMoved;

        public KinectPointerController(KinectSensor sensor)
        {
            this.interactionClient = new KinectPointerInteractionClient();
            this.interactionStream = new InteractionStream(sensor, this.interactionClient);
            this.interactionStream.InteractionFrameReady += new EventHandler<InteractionFrameReadyEventArgs>(KinectPointer_InteractionFrameReady);
            this.trackingId = 0;
        }

        public void UpdatePointer(AllFramesReadyEventArgs e, Vector4 accelerometerInfo, int trackingId)
        {
            // Updates who should the controller be tracking.
            this.trackingId = trackingId;

            // Processes image depth data.
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if(depthFrame == null) 
                {
                    // Frame is already too late.
                    return;
                }

                this.interactionStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
            }

            // Processes skeleton data.
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if(skeletonFrame == null) 
                {
                    // Frame is already too late.
                    return;
                }

                Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(skeletons);
                interactionStream.ProcessSkeleton(skeletons, accelerometerInfo, skeletonFrame.Timestamp);
            }
        }

        private void KinectPointer_InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            InteractionFrame interactionFrame = e.OpenInteractionFrame();
            if (interactionFrame == null)
            {
                // Frame is already too late.
                return;
            }

            // Copy user data.
            UserInfo[] users = new UserInfo[6];
            interactionFrame.CopyInteractionDataTo(users);

            // Loop until tracked user is found.
            foreach (UserInfo user in users)
            {
                // Tracked user
                if (user.SkeletonTrackingId == this.trackingId)
                {
                    // Left hand data.
                    KinectPointerHand leftHand = new KinectPointerHand();
                    leftHand.X = user.HandPointers[0].X;
                    leftHand.Y = user.HandPointers[0].Y;
                    leftHand.HandEventType = user.HandPointers[0].HandEventType;

                    // Right hand data.
                    KinectPointerHand rightHand = new KinectPointerHand();
                    rightHand.X = user.HandPointers[1].X;
                    rightHand.Y = user.HandPointers[1].Y;
                    rightHand.HandEventType = user.HandPointers[1].HandEventType;

                    // Create and trigger event.
                    KinectPointerEventArgs args = new KinectPointerEventArgs(this.trackingId, leftHand, rightHand);
                    this.KinectPointerMoved(this, args);
                    return;
                }
            }
        }
    }
}
