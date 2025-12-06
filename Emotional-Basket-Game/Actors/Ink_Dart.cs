using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System;
using EmotionalBasketGame.Actors.Targets;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// Thrown across the screen, to attack targets, then stick in the back.
    /// </summary>
    public class Ink_Dart : Ink_Actor_Base
    {
        /// <summary>
        /// This actor's collision.
        /// </summary>
        public BoundingCircle Hitbox;

        private Texture2D dartTexture;

        private Color[] dartColors = 
        {
            Color.IndianRed,
            Color.Orange,
            Color.LightGoldenrodYellow,
            Color.DarkOliveGreen,
            Color.LightBlue,
            Color.DarkViolet
        };

        private Color paintedColor;

        /// <summary>
        /// Whether or not the dart should be thrown.
        /// </summary>
        public bool IsThrowing {  get; set; }

        /// <summary>
        /// The speed of the dart as it travels through the BG.
        /// </summary>
        public double ThrowSpeed { get; set; }

        /// <summary>
        /// Constructs a new dart and hitbox.
        /// </summary>
        public Ink_Dart()
        {
            Hitbox = new BoundingCircle(this, 4);
        }

        /// <summary>
        /// Updates the actor, as in a Tick.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsThrowing)
            {
                double prevDepth = Depth;
                Depth = MathHelper.Min((float)(Depth + ThrowSpeed * gameTime.ElapsedGameTime.TotalSeconds), 32);

                //Called once we pass the "centerground".
                if (prevDepth < -1 && Depth >= -1)
                    StrikeAllTargetsInRange();
                if (Depth >= 32)
                {
                    IsThrowing = false;
                }
            }
        }

        /// <summary>
        /// Pins all targets in the area of this dart.
        /// </summary>
        public void StrikeAllTargetsInRange()
        {
            foreach (Ink_Actor_Base actor in World.Actors)
            {
                if (actor is Ink_Target_Base target)
                {
                    if (target.WasPinned) continue;

                    if (CollisionHelper.Collides(Hitbox, target.Hitbox))
                    {
                        if (target.OnPinned(this))
                        {
                            BasedActor = target;
                            IsThrowing = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            System.Random rand = new System.Random();
            paintedColor = dartColors[rand.Next(dartColors.Length)];
            dartTexture = content.Load<Texture2D>("T2D_Pin");
        }

        /// <summary>
        /// Updates the actor's visuals.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">The sprite batch provided.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            //Calculate all the depth values.
            float depthAlpha = MathHelper.Lerp(1.0f, 0.0f, MathHelper.Max((float)(Depth / -32), 0));
            Color dartColor = new Color(paintedColor.R * depthAlpha, paintedColor.G * depthAlpha, paintedColor.B * depthAlpha, 1.0f * depthAlpha);
            Vector2 position = GetScreenPosition();
            float actorScale = GetScreenScale() * GetDepthScale();

            //Create a rotation for the dart based on its position from the center.
            Vector2 angleVector = new Vector2(MathHelper.Clamp(Position.X / 600, -1.0f, 1.0f), MathHelper.Clamp(Position.Y / 320, -1.0f, 1.0f));
            angleVector.Normalize();
            float angle = (float)System.Math.Atan2(-angleVector.X, angleVector.Y);

            //Finally, draw the dart. FINALLY.
            spriteBatch.Draw(dartTexture, position, null, dartColor, angle, new Vector2(256, 256), 0.25f * actorScale, SpriteEffects.None, 0f);
        }
    }
}
