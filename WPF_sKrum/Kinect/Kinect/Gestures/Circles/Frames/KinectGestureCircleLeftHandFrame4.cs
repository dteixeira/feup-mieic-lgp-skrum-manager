﻿using Microsoft.Kinect;

namespace Kinect.Gestures.Circles.Frames
{
    /// <summary>
    /// Describes the fourth frame of a left hand circle.
    /// </summary>
    public class KinectGestureCircleLeftHandFrame4 : IKinectGestureFrame
    {
        /// <summary>
        /// Checks if the given skeleton's tracking data matches
        /// the gesture represented by this frame.
        /// </summary>
        /// <param name="skeleton">Skeleton to analize</param>
        /// <returns>
        /// Success if the gesture was correct, Waiting if the
        /// gesture was not quite right, but still possible, Fail otherwise.
        /// </returns>
        public KinectGestureResult ProcessFrame(Skeleton skeleton)
        {
            // Checks if the left hand is right of the left elbow
            if (skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ElbowLeft].Position.X)
            {
                // Checks if the right hand is above the right elbow
                if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y)
                {
                    // The fourth part of the gesture was completed.
                    return KinectGestureResult.Success;
                }
                else
                {
                    // Gesture recognition will pause.
                    return KinectGestureResult.Waiting;
                }
            }

            // The standard result is failure.
            return KinectGestureResult.Fail;
        }
    }
}