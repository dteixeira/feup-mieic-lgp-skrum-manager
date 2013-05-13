﻿using Microsoft.Kinect;

namespace Kinect.Gestures.Waves.Frames
{
    /// <summary>
    /// Describes the second frame of a left hand wave.
    /// </summary>
    public class KinectGestureWaveLeftHandFrame2 : IKinectGestureFrame
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
            // Checks if the left hand is above the left elbow.
            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y)
            {
                // Checks if the left hand is at the right of the left elbow.
                if (skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ElbowLeft].Position.X)
                {
                    // The second part of the gesture was completed.
                    return KinectGestureResult.Success;
                }

                // Gesture was not completed, but it's still possible to achieve.
                else
                {
                    // Will pause recognition and try later.
                    return KinectGestureResult.Waiting;
                }
            }

            // The standard result is failure.
            return KinectGestureResult.Fail;
        }
    }
}