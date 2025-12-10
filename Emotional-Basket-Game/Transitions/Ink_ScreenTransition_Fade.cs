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
    public class Ink_ScreenTransition_Fade : Ink_ScreenTransition_Base
    {
        /// <summary>
        /// The background color.
        /// </summary>
        public Color FadeColor { get; set; } = Color.White;

        // Handles drawing the fade.
        protected Texture2D backgroundTexture;

        public Ink_ScreenTransition_Fade(Ink_PinGameManager game, double transitionTime) : base(game, transitionTime) 
        {
            FadeColor = Color.White;
        }

        public Ink_ScreenTransition_Fade(Ink_PinGameManager game, double transitionTime, Color fadeColor) : base(game, transitionTime)
        {
            FadeColor = fadeColor;
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
            if (TransitionTime <= 0)
                alpha = 1.0f;
            else
                alpha = (float)(transitionProgress / TransitionTime);

            spriteBatch.Draw(backgroundTexture, GetScreenRectangle(), FadeColor * alpha);
        }
    }
}
