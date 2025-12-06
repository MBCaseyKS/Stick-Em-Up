using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace EmotionalBasketGame.Actors.Targets
{
    /// <summary>
    /// A target that can be damaged by a dart.
    /// </summary>
    public class Ink_Target_Base : Ink_Actor_Base
    {
        private Texture2D targetTexture;

        double animProgress;

        bool wasDamaged;

        /// <summary>
        /// This actor's collision.
        /// </summary>
        public BoundingCircle Hitbox;

        /// <summary>
        /// Whether or not we've been pinned.
        /// </summary>
        public bool WasPinned { get; protected set; }

        /// <summary>
        /// Creates a new target actor.
        /// </summary>
        public Ink_Target_Base()
        {
            Hitbox = new BoundingCircle(this, 32);
        }

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            targetTexture = content.Load<Texture2D>("T2D_Target");
        }


        /// <summary>
        /// Updates the actor, as in a Tick.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            animProgress = (animProgress + gameTime.ElapsedGameTime.TotalSeconds * (WasPinned ? 16 : 1.5)) % 2.0;
        }

        /// <summary>
        /// Updates the actor's visuals.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">The sprite batch provided.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            //Draw the target.
            Vector2 position = GetScreenPosition();
            float screenScale = GetScreenScale() * GetDepthScale();

            int currFrame = (int)animProgress;
            var source = new Rectangle(currFrame * 512, WasPinned ? 512 : 0, 512, 512);
            spriteBatch.Draw(targetTexture, position, source, Color.White, 0, new Vector2(256, 256), 0.25f * screenScale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Is called when this target is struck by a pin.
        /// </summary>
        /// <param name="dart">The pin that struck us.</param>
        /// <returns>True if we want the pin to NOT keep travelling, and instead stick with US.</returns>
        public virtual bool OnPinned(Ink_Dart dart)
        {
            WasPinned = true;
            BasedActor = dart;
            Depth = dart.Depth + 1; //So we'll always be "behind" the dart.
            return false;
        }
    }
}
