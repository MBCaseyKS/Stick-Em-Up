using EmotionalBasketGame.Actors.Targets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// Follows the mouse when it's on screen.
    /// </summary>
    public class Ink_MouseRep : Ink_Actor_Base
    {
        private Texture2D _mouseTexture;

        MouseState currentMouseState, previousMouseState;
        private bool isMouseHeld;

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            _mouseTexture = content.Load<Texture2D>("T2D_PencilMouse");
        }

        /// <summary>
        /// Updates the actor, as in a Tick.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            isMouseHeld = currentMouseState.LeftButton == ButtonState.Pressed;
            Vector2 screenCenter = GetScreenCenter();
            float screenScale = GetScreenScale();

            float x = (currentMouseState.X - screenCenter.X) / screenScale;
            float y = (currentMouseState.Y - screenCenter.Y) / screenScale;
            Vector2 mousePos = new Vector2(x, y);

            Position = mousePos;
        }

        /// <summary>
        /// Updates the actor's visuals.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">The sprite batch provided.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            float screenScale = GetScreenScale();

            if (Math.Abs(Position.X) > 640 || Math.Abs(Position.Y) > 360) return;

            Vector2 position = GetScreenPosition(false);
            Rectangle source = new Rectangle(isMouseHeld ? 256 : 0, 0, 256, 256);
            spriteBatch.Draw(_mouseTexture, position, source, Color.White, 0, Vector2.Zero, 0.25f * screenScale, SpriteEffects.None, 0f);
        }
    }
}
