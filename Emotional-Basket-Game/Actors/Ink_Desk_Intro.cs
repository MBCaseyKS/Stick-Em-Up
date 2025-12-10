using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// A desk used for the background of the intro.
    /// </summary>
    public class Ink_Desk_Intro : Ink_Actor_Base
    {
        /// <summary>
        /// Whether or not to enable effects.
        /// </summary>
        public bool LightsOn {  get; set; }

        /// <summary>
        /// A subtle glow around the target.
        /// </summary>
        public Ink_PointLight TargetGlow { get; set; }

        private Texture2D deskTexture;

        private double activateAlpha;

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            deskTexture = content.Load<Texture2D>("T2D_Titlescreen_Desk");
        }

        /// <summary>
        /// Updates the actor, as in a Tick.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) 
        {
            base.Update(gameTime);

            if (LightsOn)
            {
                activateAlpha = Math.Min(activateAlpha + gameTime.ElapsedGameTime.TotalSeconds, 1);
                if (TargetGlow != null)
                {
                    float sinGlow = 1.0f + 0.05f * (float)Math.Sin(0.5 * Math.PI * gameTime.TotalGameTime.TotalSeconds);
                    TargetGlow.IsActive = true;
                    TargetGlow.OpacityMulti = 0.75f * sinGlow * (float)activateAlpha;
                    TargetGlow.SizeScale = 1.25f * sinGlow;
                    TargetGlow.SizeAlpha = 1.0f;
                }
            }
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
            float screenScale = GetScreenScale();

            spriteBatch.Draw(deskTexture, position, null, Color.White, 0, new Vector2(1024, 1024), 1.0f * screenScale, SpriteEffects.None, 0f);
        }
    }
}
