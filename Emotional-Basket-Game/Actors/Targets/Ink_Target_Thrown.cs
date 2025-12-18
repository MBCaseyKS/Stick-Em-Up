using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors.Targets
{
    /// <summary>
    /// A target thrown upwards from below the screen.
    /// </summary>
    public class Ink_Target_Thrown : Ink_Target_Base
    {
        protected Vector2 currentVelocity;
        protected float gravityStrength = 750;

        public Ink_Target_Thrown(Vector2 currentVelocity, double scaleMulti = 1.0) : base(scaleMulti)
        {
            this.currentVelocity = currentVelocity;
        }

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            PlaySoundInst(throwSoundInst, Ink_RandomHelper.RandRange(0.5f, 0.6f), Ink_RandomHelper.RandRange(-0.2f, 0.2f));
        }

        /// <summary>
        /// Updates the actor, as in a Tick.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!WasPinned)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalSeconds * SpeedMulti);
                Vector2 currentPos = Position;

                currentPos.X += currentVelocity.X * delta;
                currentPos.Y += currentVelocity.Y * delta * 0.5f;
                currentVelocity.Y += gravityStrength * delta;
                currentPos.Y += currentVelocity.Y * delta * 0.5f;
                Position = currentPos;

                if (currentVelocity.Y > 0 && Position.Y >= 450)
                {
                    if (FailedDel != null)
                        FailedDel(this);

                    World.DestroyActor(this);
                }
            }
        }
    }
}
