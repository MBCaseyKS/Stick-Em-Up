using EmotionalBasketGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors.HUDs
{
    /// <summary>
    /// Follows the mouse when it's on screen.
    /// </summary>
    public class Ink_HUD_PencilMouse : Ink_HUD_Base
    {
        private Texture2D _mouseTexture;

        public Ink_HUD_PencilMouse(Ink_PinGameManager game, Ink_GameScreen_Base activeScreen, int renderPriority = 0) : base(game, activeScreen, renderPriority) { }

        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            _mouseTexture = content.Load<Texture2D>("T2D_PencilMouse");
        }

        public override void Update(GameTime gameTime) { }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            bool isMouseHeld = currentMouseState.LeftButton == ButtonState.Pressed;
            Vector2 screenCenter = GetScreenCenter();
            float screenScale = GetScreenScale();

            float x = (currentMouseState.X - screenCenter.X) / screenScale;
            float y = (currentMouseState.Y - screenCenter.Y) / screenScale;
            Vector2 mousePos = new Vector2(x, y);

            if (Math.Abs(mousePos.X) > 640 || Math.Abs(mousePos.Y) > 360) return;

            Vector2 position = new Vector2(640,360) + mousePos;
            Rectangle source = new Rectangle(isMouseHeld ? 256 : 0, 0, 256, 256);
            spriteBatch.Draw(_mouseTexture, position, source, Color.White, 0, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
        }
    }
}
