using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors
{
    public class Ink_PointLight : Ink_Actor_Base
    {
        /// <summary>
        /// Who owns and controls this light.
        /// </summary>
        public Object Owner {  get; set; }

        /// <summary>
        /// What size the light should be at.
        /// </summary>
        public float SizeAlpha { get; set; }

        /// <summary>
        /// The base size scale.
        /// </summary>
        public float SizeScale { get; set; }

        /// <summary>
        /// A multiplier for the light's opacity.
        /// </summary>
        public float OpacityMulti { get; set; } = 1.0f;

        /// <summary>
        /// Whether or not the light is active.
        /// </summary>
        public bool IsActive { get; set; }

        private Texture2D alphaMask;
        private Color lightColor;
        private string pathName;

        /// <summary>
        /// Inits a point light.
        /// </summary>
        /// <param name="lightTextureName">The path name for the light texture.</param>
        public Ink_PointLight(Color lightColor, string pathName, float SizeScale, bool IsActive = true)
        {
            this.lightColor = lightColor;
            this.pathName = pathName;
            this.SizeScale = SizeScale;
            this.IsActive = IsActive;
        }

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            if (pathName != "")
                alphaMask = content.Load<Texture2D>(pathName);
        }

        /// <summary>
        /// Updates the actor's visuals.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">The sprite batch provided.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (!IsActive) return;
            if (SizeAlpha <= 0) return;

            Vector2 position = GetScreenPosition();
            float screenScale = GetScreenScale();

            spriteBatch.Draw(alphaMask, position, null, lightColor * OpacityMulti, 0, new Vector2(128,128), SizeAlpha * SizeScale * screenScale, SpriteEffects.None, 0f);
        }
    }
}
