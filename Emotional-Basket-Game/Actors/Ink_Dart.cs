using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System;
using EmotionalBasketGame.Actors.Targets;
using Microsoft.Xna.Framework.Audio;
using EmotionalBasketGame.Screens;
using System.Reflection;

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

        /// <summary>
        /// The round handler.
        /// </summary>
        public Ink_TargetRound_Layout RoundHandler { get; set; }

        private Texture2D dartTexture;

        private SoundEffectInstance throwSoundInst;
        private SoundEffectInstance stickSoundInst;
        private SoundEffectInstance multiHitSoundInst;

        private int randomColor;
        private double downwardVelocity;

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
                    EndThrow();
                else if (Depth >= 0)
                {
                    downwardVelocity += 4000 * gameTime.ElapsedGameTime.TotalSeconds * 0.5;
                    Position = Position + new Vector2(0, (float)(downwardVelocity * gameTime.ElapsedGameTime.TotalSeconds));
                    downwardVelocity += 4000 * gameTime.ElapsedGameTime.TotalSeconds * 0.5; //Heard this fixes some lag issues doing it this way?
                }
            }
        }

        /// <summary>
        /// Pins all targets in the area of this dart.
        /// </summary>
        public void StrikeAllTargetsInRange()
        {
            int targetsHit = 0;
            foreach (Ink_Actor_Base actor in World.Actors)
            {
                if (actor is Ink_Target_Base target)
                {
                    if (target.WasPinned) continue;

                    if (CollisionHelper.Collides(Hitbox, target.Hitbox))
                    {
                        targetsHit++;
                        if (target.OnPinned(this))
                        {
                            BasedActor = target;
                            IsThrowing = false;
                            break;
                        }
                    }
                }
            }

            if (targetsHit > 0)
            {
                if (RoundHandler != null)
                    RoundHandler.TargetsHit += targetsHit;

                int additionalTargets = Math.Max(targetsHit - 1, 0);
                World.DoScreenShake(new Vector2(targetsHit > 1 ? 7.5f : 5f), 0.25f + additionalTargets * 0.25f, 0.1f, 0.1f + additionalTargets * 0.2f);

                if (additionalTargets > 0)
                    PlaySoundInst(multiHitSoundInst, 0.7f, Ink_RandomHelper.RandRange(-0.15f, 0.15f)); //Some variation.
            }
            else if (RoundHandler != null)
                RoundHandler.DartsMissed++;
        }

        /// <summary>
        /// Is called at the start of a dart being thrown.
        /// </summary>
        public void BeginThrow(float chargeAlpha)
        {
            IsThrowing = true;
            ThrowSpeed = MathHelper.Lerp(256, 384, chargeAlpha); //Up to 1.5x throw speed, depending on charge.
            PlaySoundInst(throwSoundInst, MathHelper.Lerp(0.75f, 1.0f, chargeAlpha), 
                MathHelper.Lerp(-0.25f, 0.25f, chargeAlpha) + Ink_RandomHelper.RandRange(-0.1f, 0.1f)); //Higher pitch for stronger throws, plus some variation.
        }

        /// <summary>
        /// Is called upon the dart ending its throw.
        /// </summary>
        public void EndThrow()
        {
            IsThrowing = false;
            PlaySoundInst(stickSoundInst, 0.2f, Ink_RandomHelper.RandRange(-0.15f, 0.15f)); //Some variation.
        }

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            System.Random rand = new System.Random();
            randomColor = rand.Next(8);
            dartTexture = content.Load<Texture2D>("T2D_Pin");

            InitSoundInstance(content, "WAV_WhipThrow", out throwSoundInst);
            InitSoundInstance(content, "WAV_ArrowShot", out stickSoundInst);
            InitSoundInstance(content, "WAV_Multihit", out multiHitSoundInst);
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
            Color dartColor = new Color(1.0f * depthAlpha, 1.0f * depthAlpha, 1.0f * depthAlpha, 1.0f * depthAlpha);
            Vector2 position = GetScreenPosition();
            float actorScale = GetScreenScale() * GetDepthScale();

            //Create a rotation for the dart based on its position from the center.
            Vector2 angleVector = new Vector2(MathHelper.Clamp(Position.X / 600, -1.0f, 1.0f), MathHelper.Clamp(Position.Y / 320, -1.0f, 1.0f));
            angleVector.Normalize();
            float angle = (float)System.Math.Atan2(-angleVector.X, angleVector.Y);

            //Finally, draw the dart. FINALLY.
            Rectangle source = new Rectangle((randomColor % 4) * 256, (randomColor / 4) * 256, 256, 256);
            spriteBatch.Draw(dartTexture, position, source, dartColor, angle, new Vector2(128, 128), 0.5f * actorScale, SpriteEffects.None, 0f);
        }
    }
}
