using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Transitions
{
    /// <summary>
    /// A basic fade screen transition.
    /// </summary>
    public class Ink_ScreenTransition_Shutters : Ink_ScreenTransition_Base
    {
        /// <summary>
        /// The background color.
        /// </summary>
        public Color FadeColor { get; set; } = Color.White;

        /// <summary>
        /// The amount of shutters on each half of the screen.
        /// </summary>
        public int Shutters { get; set; }

        // Handles drawing the fade.
        protected Texture2D backgroundTexture;

        public Ink_ScreenTransition_Shutters(Ink_PinGameManager game, double transitionTime) : base(game, transitionTime) 
        {
            FadeColor = Color.White;
            Shutters = 3;
        }

        public Ink_ScreenTransition_Shutters(Ink_PinGameManager game, double transitionTime, int shutterCount, Color fadeColor) : base(game, transitionTime)
        {
            FadeColor = fadeColor;
            Shutters = Math.Max(shutterCount, 1);
        }

        public Ink_ScreenTransition_Shutters(Ink_PinGameManager game, double transitionTime, Color fadeColor) : base(game, transitionTime)
        {
            FadeColor = fadeColor;
            Shutters = 3;
        }

        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            base.LoadContent(graphics, content);

            backgroundTexture = new Texture2D(graphics, 1, 1);
            backgroundTexture.SetData<Color>(new Color[] { Color.White });
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float alpha;

            float shutterCloseTime = (float)(TransitionTime / Shutters);
            float shutterSpacing = (float)(TransitionTime / (Shutters*1.5));
            Rectangle screenRectangle = GetScreenRectangle();

            for (int i = 0; i < Shutters; i++)
            {
                if (shutterCloseTime <= 0)
                    alpha = 1.0f;
                else
                    alpha = Ink_Math.EaseOutCubic((float)Math.Clamp((transitionProgress - i*shutterSpacing) / shutterCloseTime, 0.0f, 1.0f));

                Rectangle shutterRectangle = new Rectangle(0, 0, screenRectangle.Width / (Shutters * 2) + 5, screenRectangle.Height);
                float yPosition = (i % 2 == 0) ? MathHelper.Lerp(screenRectangle.Height, 0, alpha) : MathHelper.Lerp(-screenRectangle.Height, 0, alpha);

                spriteBatch.Draw(backgroundTexture, new Vector2((shutterRectangle.Width - 5) * i - 2.5f, yPosition), shutterRectangle, FadeColor);
                spriteBatch.Draw(backgroundTexture, new Vector2(screenRectangle.Width - (shutterRectangle.Width - 5) * (i+1) - 2.5f, yPosition), shutterRectangle, FadeColor);
            }
        }
    }
}
