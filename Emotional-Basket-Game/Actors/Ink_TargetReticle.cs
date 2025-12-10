using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// Follows the mouse, and allows you to throw darts.
    /// </summary>
    public class Ink_TargetReticle : Ink_Actor_Base
    {
        private Texture2D _indicatorTexture;
        private SoundEffectInstance chargeSoundInst;

        MouseState currentMouseState, previousMouseState;

        private bool isCharging;
        private double chargeProgress = -1.0;

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            _indicatorTexture = content.Load<Texture2D>("T2D_Reticle");
            InitSoundInstance(content, "WAV_ThrowCharge", out chargeSoundInst);
        }

        /// <summary>
        /// Updates the actor, as in a Tick.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            Vector2 screenCenter = GetScreenCenter();
            float screenScale = GetScreenScale();

            float x = MathHelper.Clamp((currentMouseState.X - screenCenter.X) / screenScale, -600, 600) + ScreenOffset.X;
            float y = MathHelper.Clamp((currentMouseState.Y - screenCenter.Y) / screenScale, -320, 320) + ScreenOffset.Y;
            Vector2 mousePos = new Vector2(x, y);
            Position = mousePos;

            if (Game.IsActive && currentMouseState.LeftButton == ButtonState.Pressed && (isCharging || chargeProgress < 0))
            {
                if (!isCharging && chargeProgress < 0)
                {
                    isCharging = true;
                    PlaySoundInst(chargeSoundInst, 0.25f, Ink_RandomHelper.RandRange(-0.1f, 0.1f)); //Some variation.
                }
                if (isCharging)
                {
                    if (chargeProgress < 0)
                    {
                        //Will add SFX once learned.
                        chargeProgress = 0;
                    }

                    chargeProgress = MathHelper.Min((float)(chargeProgress + gameTime.ElapsedGameTime.TotalSeconds * 2.0), 1f);
                }
            }
            else if (chargeProgress >= 0)
            {
                if (isCharging)
                {
                    ThrowDart();
                    isCharging = false;
                    chargeProgress = 1.0; //Force the cooldown.

                    if (chargeSoundInst != null)
                        chargeSoundInst.Stop();
                }

                chargeProgress = MathHelper.Max((float)(chargeProgress - gameTime.ElapsedGameTime.TotalSeconds * 5), 0f);
                if (chargeProgress <= 0)
                    chargeProgress = -1;
            }
        }

        /// <summary>
        /// Throws a dart near the reticle's position.
        /// </summary>
        /// <returns>The constructed dart.</returns>
        public Ink_Dart ThrowDart()
        {
            if (World != null)
            {
                float chargeAlpha = (float)Math.Pow(Math.Max(chargeProgress, 0), 3);
                float offsetMax = MathHelper.Lerp(100, 0, chargeAlpha);
                Vector2 randOffset = Ink_RandomHelper.VRand(offsetMax);
                Ink_Dart dart = (Ink_Dart)World.AddActor(new Ink_Dart(), Position + randOffset, true, -32);
                if (dart != null)
                {
                    dart.BeginThrow(chargeAlpha);
                    return dart;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates the actor's visuals.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">The sprite batch provided.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            //Draw the lower indicator.
            Color indicatorColor = new Color(255, 55, 55, 55);
            Vector2 position = GetScreenPosition();
            float screenScale = GetScreenScale();

            float chargeAlpha = (float)Math.Pow(Math.Max(chargeProgress, 0), 3);
            Vector2 randomVect = Ink_RandomHelper.VRand(1.0f * 1.5f * screenScale * chargeAlpha);

            for (int i = 0; i < 4; i++)
            {
                float angle = i * MathHelper.PiOver2;
                Vector2 offset = randomVect + Vector2.Transform(new Vector2(0, -5 * screenScale), Matrix.CreateRotationZ(angle));
                spriteBatch.Draw(_indicatorTexture, position + offset, null, indicatorColor, angle, new Vector2(256, 256), 0.25f * screenScale, SpriteEffects.None, 0f);
            }

            if (chargeProgress >= 0)
            {
                float addOffset = MathHelper.Lerp(-45, -5, chargeAlpha) * screenScale;
                indicatorColor = new Color(1, 0.05f, 0.05f, MathHelper.Lerp(0, 0.75f, chargeAlpha));

                for (int i = 0; i < 4; i++)
                {
                    float angle = i * MathHelper.PiOver2;
                    Vector2 offset = randomVect + Vector2.Transform(new Vector2(0, addOffset), Matrix.CreateRotationZ(angle));
                    spriteBatch.Draw(_indicatorTexture, position + offset, null, indicatorColor, angle, new Vector2(256, 256), 0.25f * screenScale, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
