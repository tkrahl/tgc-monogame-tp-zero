using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TGC.MonoGame.TP
{
    /// <summary>
    /// A Camera that follows objects
    /// </summary>
    class FollowCamera
    {
        private const float AxisDistanceToTarget = 1000f;

        private const float AngleFollowSpeed = 0.015f;

        private const float AngleThreshold = 0.85f;

        public Matrix Projection { get; private set; }

        public Matrix View { get; private set; }

        private Vector3 CurrentRightVector { get; set; } = Vector3.Right;

        private float RightVectorInterpolator { get; set; } = 0f;

        private Vector3 PastRightVector { get; set; } = Vector3.Right;

        /// <summary>
        /// Creates a FollowCamera to follow a specific World matrix
        /// </summary>
        /// <param name="aspectRatio"></param>
        public FollowCamera(float aspectRatio)
        {
            // Orthographic camera
            // Projection = Matrix.CreateOrthographic(screenWidth, screenHeight, 0.01f, 10000f);

            // Perspective camera
            // Use 60° as FOV, the aspect ratio, set 0.1 as near plane and 100000 (a lot) as a far plane distance
            Projection = Matrix.CreatePerspectiveFieldOfView(MathF.PI / 3f, aspectRatio, 0.1f, 100000f);
        }

        /// <summary>
        /// Updates the Camera using an updated World matrix to follow
        /// </summary>
        /// <param name="gameTime">The Game Time to calculate framerate-independent movement</param>
        /// <param name="followedWorld">The World matrix to follow</param>
        public void Update(GameTime gameTime, Matrix followedWorld)
        {
            // Extract the Elapsed Time
            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            // Get the position of the followed world matrix
            var followedPosition = followedWorld.Translation;

            // Get the Right vector from the followed world matrix
            var followedRight = followedWorld.Right;

            // If the dot product of the past right vector
            // and the current right vector is greater than a threshold,
            // move the Interpolator variable (from 0 to 1) closer to one
            if (Vector3.Dot(followedRight, PastRightVector) > AngleThreshold)
            {
                // Add an increment to Interpolator
                RightVectorInterpolator += elapsedTime * AngleFollowSpeed;

                // Clamp Interpolator to 1 maximum
                RightVectorInterpolator = MathF.Min(RightVectorInterpolator, 1f);

                // Calculate the right vector from the interpolation
                // This moves the Right vector to match the followed Right vector
                // In this case use a x^2 curve to make it smoother
                // Interpolator will eventually become 1
                CurrentRightVector = Vector3.Lerp(CurrentRightVector, followedRight, RightVectorInterpolator * RightVectorInterpolator);
            }
            else
                // If the angle does not pass the threshold, set it back to zero
                RightVectorInterpolator = 0f;

            // Store the Right vector to use it the next iteration
            PastRightVector = followedRight;
            
            // Calculate the camera position
            // Take the followed position, add an offset in the Y and Followed Right axis
            var offsetedPosition = followedPosition 
                + CurrentRightVector * AxisDistanceToTarget
                + Vector3.Up * AxisDistanceToTarget;

            // Calculate the updated Up vector
            // Note that the default Up vector (0, 1, 0) can't be used as 
            // it is not correct, calculate the correct up vector instead
            // (by using this cross-product trick)

            // Calculate the Forward vector by subtracting the destination to the origin
            // Then normalizing it (expensive!)
            // (This operation needs normalized vectors)
            var forward = (followedPosition - offsetedPosition);
            forward.Normalize();

            // Get the right vector by assuming the camera has the Up vector pointing upwards
            // and it is not rotated in the X axis (roll)
            var right = Vector3.Cross(forward, Vector3.Up);

            // Once the correct Camera Right vector is calculated, get the correct Camera Up vector by using 
            // another cross product
            var cameraCorrectUp = Vector3.Cross(right, forward);

            // Calculate the View matrix by using the Camera Position, the Position the Camera is looking at,
            // and the Camera Up vector
            View = Matrix.CreateLookAt(offsetedPosition, followedPosition, cameraCorrectUp);
        }
    }
}
