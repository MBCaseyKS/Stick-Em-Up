using Microsoft.Xna.Framework;

namespace EmotionalBasketGame.Actors.Cameras
{
    /// <summary>
    /// An interface defining a camera
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// The view matrix
        /// </summary>
        Matrix View { get; }

        /// <summary>
        /// The projection matrix
        /// </summary>
        Matrix Projection { get; }

        public void Update(GameTime gameTime);
    }
}
