using EmotionalBasketGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors.HUDs
{
    /// <summary>
    /// Shows the created by at the start of the game.
    /// </summary>
    public class Ink_HUD_CreatedBy : Ink_HUD_Base
    {
        /// <summary>
        /// The background color.
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.Black;

        private Texture2D createdByTexture;
        protected Texture2D backgroundTexture;
        private double introProgress;

        public Ink_HUD_CreatedBy(Ink_PinGameManager game, Ink_GameScreen_Base activeScreen, int renderPriority = 0) : base(game, activeScreen, renderPriority) { }

        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            createdByTexture = content.Load<Texture2D>("T2D_CreatedBy");
            backgroundTexture = new Texture2D(graphics, 1, 1);
            backgroundTexture.SetData<Color>(new Color[] { Color.White });
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Fade
            float alpha = 1.0f - MathHelper.Clamp((float)(introProgress - 4f), 0.0f, 1.0f);
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 720), BackgroundColor * alpha);

            //Created by
            if (createdByTexture != null)
            {
                alpha = MathHelper.Clamp((float)(introProgress - 0.25f), 0.0f, 1.0f) - MathHelper.Clamp((float)(introProgress - 4f), 0.0f, 1.0f);
                Rectangle source = new Rectangle(0, gameTime.TotalGameTime.TotalSeconds % 0.5 > 0.25 ? 512 : 0, 1024, 512);
                spriteBatch.Draw(createdByTexture, new Vector2(640, 360), source, Color.White * alpha, 0, new Vector2(512, 256), 0.5f, SpriteEffects.None, 0f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            introProgress = Math.Min(introProgress + gameTime.ElapsedGameTime.TotalSeconds, 5.0f);
            if (introProgress >= 5)
                DoClose();
        }
    }
}
