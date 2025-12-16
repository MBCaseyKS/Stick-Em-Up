using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// Draws the billboard in the background.
    /// </summary>
    public class Ink_Actor_Billboard : Ink_Actor_Base
    {
        private Texture2D billboardTexture;

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            billboardTexture = content.Load<Texture2D>("T2D_Billboard"); //Whoops messed up with the filename. Oh well!
        }


        /// <summary>
        /// Updates the actor's visuals.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">The sprite batch provided.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            Vector2 position = GetScreenPosition();
            float tileScale = GetScreenScale() * GetDepthScale();

            spriteBatch.Draw(billboardTexture, position, null, new Color(200, 200, 200), 0f, new Vector2(512), 1.75f * tileScale, SpriteEffects.None, 0f);
        }
    }
}
