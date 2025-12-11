using EmotionalBasketGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors.HUDs
{
    /// <summary>
    /// A sudden screen fade.
    /// </summary>
    public class Ink_HUD_Flashbang : Ink_HUD_Base
    {
        /// <summary>
        /// The background color.
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.White;

        /// <summary>
        /// How long it takes the flash to subside.
        /// </summary>
        public double FlashTime { get; set; } = 1.0;

        protected Texture2D backgroundTexture;
        private double progress;

        public Ink_HUD_Flashbang(Ink_PinGameManager game, Ink_GameScreen_Base activeScreen, int renderPriority = 0) : base(game, activeScreen, renderPriority) { }

        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            backgroundTexture = new Texture2D(graphics, 1, 1);
            backgroundTexture.SetData<Color>(new Color[] { Color.White });
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (backgroundTexture != null)
            {
                float alpha;
                if (FlashTime > 0)
                    alpha = 1.0f - (float)Math.Pow(progress / FlashTime, 2);
                else
                    alpha = 1.0f;

                spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 720), BackgroundColor * alpha);
            }
        }

        public override void Update(GameTime gameTime)
        {
            progress = Math.Min(progress + gameTime.ElapsedGameTime.TotalSeconds, FlashTime);
            if (progress >= FlashTime)
                DoClose();
        }
    }
}
