using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using EmotionalBasketGame.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// Holds a particlesystem.
    /// </summary>
    public class Ink_Emitter : Ink_Actor_Base
    {
        public ParticleSystem myParticle { get; set; }

        public Ink_Emitter(ParticleSystem particle)
        {
            myParticle = particle;
        }

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            if (myParticle != null)
                myParticle.LoadContent(content);
        }

        /// <summary>
        /// Updates the actor, as in a Tick.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (myParticle != null)
                myParticle.Update(gameTime);
        }


        /// <summary>
        /// Updates the actor's visuals.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">The sprite batch provided.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            if (myParticle != null)
                myParticle.Draw(gameTime, spriteBatch, GetScreenCenter(), ScreenOffset, GetScreenScale());
        }

        /// <summary>
        /// Returns if the SpriteBatch should be remade with a new blend state.
        /// </summary>
        /// <param name="blendState">The custom blend state to use.</param>
        /// <returns>Returns if the SpriteBatch should be remade with a new blend state.</returns>
        public override bool GetCustomBlend(out BlendState blendState)
        {
            if (myParticle == null)
            {
                blendState = null;
                return false;
            }

            blendState = myParticle.BlendState;
            return true;
        }
    }
}
