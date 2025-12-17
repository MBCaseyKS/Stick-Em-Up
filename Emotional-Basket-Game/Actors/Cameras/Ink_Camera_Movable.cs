using Microsoft.Xna.Framework;

namespace EmotionalBasketGame.Actors.Cameras
{
    /// <summary>
    /// A camera whose position is controlled by another actor.
    /// </summary>
    public class Ink_Camera_Movable : ICamera
    {
        // The angle of rotation about the Y-axis
        public float HorizontalAngle { get; set; }

        // The angle of rotation about the X-axis
        public float VerticalAngle { get; set; }

        Vector3 position;

        Game game;

        public Matrix View { get; protected set; }

        public Matrix Projection { get; protected set; }

        /// <summary>
        /// Sets the camera's position.
        /// </summary>
        /// <param name="position">The new position.</param>
        public void SetPosition(Vector3 newPosition)
        {
            this.position = newPosition;

            // determine the direction the camera faces
            var direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(VerticalAngle) * Matrix.CreateRotationY(HorizontalAngle));

            // create the veiw matrix
            View = Matrix.CreateLookAt(position, position + direction, Vector3.Up);
        }

        public void Update(GameTime gameTime) { }

        /// <summary>
        /// Constructs a new FPS Camera
        /// </summary>
        /// <param name="game">The game this camera belongs to</param>
        /// <param name="position">The player's initial position</param>
        public Ink_Camera_Movable(Game game, Vector3 newPosition)
        {
            this.game = game;
            SetPosition(newPosition);

            this.HorizontalAngle = 0;
            this.VerticalAngle = 0;

            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio, 1, 1000);
        }
    }
}
