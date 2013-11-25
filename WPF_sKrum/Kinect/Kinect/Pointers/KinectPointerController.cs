using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Linq;
using Kinect.Gestures;

namespace Kinect.Pointers
{
    /// <summary>
    /// This class controls hand events and interactions.
    /// </summary>
    public class KinectPointerController
    {
        private IInteractionClient interactionClient;
        private InteractionStream interactionStream;
        private int trackingId;
        private Skeleton[] skeletons = new Skeleton[0];
        private Kinect.Gestures.KinectGestureUserHandedness userHandedness;

        /// <summary>
        /// Creates a new pointer controller instance.
        /// </summary>
        /// <param name="sensor">Kinect sensor instance that will be used.</param>
        public KinectPointerController(KinectSensor sensor)
        {
            this.interactionClient = new KinectPointerInteractionClient();
            this.interactionStream = new InteractionStream(sensor, this.interactionClient);
            this.interactionStream.InteractionFrameReady += new EventHandler<InteractionFrameReadyEventArgs>(KinectPointer_InteractionFrameReady);
            this.trackingId = -1;
            this.userHandedness = Gestures.KinectGestureUserHandedness.RightHanded;
        }

        /// <summary>
        /// Register client callbacks to invoke when an hand motion is recognized
        /// </summary>
        public event EventHandler<KinectPointerEventArgs> KinectPointerMoved;

        /// <summary>
        /// Updates the hand tracking information.
        /// </summary>
        /// <param name="e">AllFramesReady event arguments</param>
        /// <param name="accelerometerInfo">Accelerometer information of the sensor</param>
        /// <param name="trackingId">Id of the skeleton that should be tracked in the next frame</param>
        public void UpdatePointer(AllFramesReadyEventArgs e, Vector4 accelerometerInfo, int trackingId, Kinect.Gestures.KinectGestureUserHandedness userHandedness)
        {
            // Updates who should the controller be tracking, and the correct hand.
            this.trackingId = trackingId;
            this.userHandedness = userHandedness;

            // Processes image depth data.
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                {
                    // Frame is already too late.
                    return;
                }

                this.interactionStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
            }

            // Processes skeleton data.
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                {
                    // Frame is already too late.
                    return;
                }

                skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(skeletons);
                interactionStream.ProcessSkeleton(skeletons, accelerometerInfo, skeletonFrame.Timestamp);
            }
        }

        /// <summary>
        /// Handles InteractionFrameReady events.
        /// </summary>
        /// <param name="sender">Object that triggered the event.</param>
        /// <param name="e">InteractionFrameReady event arguments.</param>
        private void KinectPointer_InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            if (this.KinectPointerMoved == null)
            {
                // No event callback is registered, so no need
                // to process the event.
                return;
            }

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
                    // Check if left hand is correctly positioned.
                    Skeleton currentSkeleton = skeletons.FirstOrDefault(s => s.TrackingId == this.trackingId);
                    if (currentSkeleton != null)
                    {
                        // Left hand should be left of the left shoulder if user is right handed,
                        // or right hand should be right of right shoulder, if user is left handed.
                        if ((currentSkeleton.Joints[JointType.HandLeft].Position.X < currentSkeleton.Joints[JointType.ShoulderLeft].Position.X && this.userHandedness == KinectGestureUserHandedness.RightHanded) ||
                            (currentSkeleton.Joints[JointType.HandRight].Position.X > currentSkeleton.Joints[JointType.ShoulderRight].Position.X && this.userHandedness == KinectGestureUserHandedness.LeftHanded))
                        {
                            // Left hand should be above left elbow if user is right handed,
                            // or right hand should be above right elbow if user is left handed.
                            if ((currentSkeleton.Joints[JointType.HandLeft].Position.Y > currentSkeleton.Joints[JointType.ElbowLeft].Position.Y && this.userHandedness == KinectGestureUserHandedness.RightHanded) ||
                                (currentSkeleton.Joints[JointType.HandRight].Position.Y > currentSkeleton.Joints[JointType.ElbowRight].Position.Y && this.userHandedness == KinectGestureUserHandedness.LeftHanded))
                            {
                                // Right hand should be above right elbow if user is right handed,
                                // or left hand should be above left elbow if user is left handed.
                                if ((currentSkeleton.Joints[JointType.HandRight].Position.Y > currentSkeleton.Joints[JointType.ElbowRight].Position.Y && this.userHandedness == KinectGestureUserHandedness.RightHanded) ||
                                    (currentSkeleton.Joints[JointType.HandLeft].Position.Y > currentSkeleton.Joints[JointType.ElbowLeft].Position.Y && this.userHandedness == KinectGestureUserHandedness.LeftHanded))
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
            }
        }
    }
}