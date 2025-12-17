using EmotionalBasketGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors.HUDs
{
    /// <summary>
    /// Follows the mouse when it's on screen.
    /// </summary>
    public class Ink_HUD_TargetReticle : Ink_HUD_Base
    {
        private Vector2 currentPosition;
        private Texture2D _mouseTexture;
        private Texture2D _indicatorTexture;
        private SoundEffectInstance chargeSoundInst;

        MouseState currentMouseState;

        private bool isCharging;
        private double chargeProgress = -1.0;

        public Ink_HUD_TargetReticle(Ink_PinGameManager game, Ink_GameScreen_Base activeScreen, int renderPriority = 0) : base(game, activeScreen, renderPriority) { }

        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            _indicatorTexture = content.Load<Texture2D>("T2D_Reticle");
            InitSoundInstance(content, "WAV_ThrowCharge", out chargeSoundInst);
        }

        public override void Update(GameTime gameTime)
        {
            currentMouseState = Mouse.GetState();

            Vector2 screenCenter = GetScreenCenter();
            float screenScale = GetScreenScale();

            float x = MathHelper.Clamp((currentMouseState.X - screenCenter.X) / screenScale, -600, 600);
            float y = MathHelper.Clamp((currentMouseState.Y - screenCenter.Y) / screenScale, -320, 320);
            Vector2 mousePos = new Vector2(x, y);
            currentPosition = mousePos;

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
            if (ActiveScreen != null)
            {
                float chargeAlpha = (float)Math.Pow(Math.Max(chargeProgress, 0), 3);
                float offsetMax = MathHelper.Lerp(100, 0, chargeAlpha);
                Vector2 randOffset = Ink_RandomHelper.VRand(offsetMax);
                Ink_Dart dart = (Ink_Dart)ActiveScreen.AddActor(new Ink_Dart() { RenderPriority = 100 }, currentPosition + ActiveScreen.ScreenOffset + randOffset, true, -32);
                if (dart != null)
                {
                    dart.BeginThrow(chargeAlpha);
                    return dart;
                }
            }

            return null;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Draw the lower indicator.
            Color indicatorColor = new Color(255, 55, 55, 55);
            Vector2 position = new Vector2(640, 360) + currentPosition;

            float chargeAlpha = (float)Math.Pow(Math.Max(chargeProgress, 0), 3);
            Vector2 randomVect = Ink_RandomHelper.VRand(1.0f * 1.5f * chargeAlpha);

            for (int i = 0; i < 4; i++)
            {
                float angle = i * MathHelper.PiOver2;
                Vector2 offset = randomVect + Vector2.Transform(new Vector2(0, -5), Matrix.CreateRotationZ(angle));
                spriteBatch.Draw(_indicatorTexture, position + offset, null, indicatorColor, angle, new Vector2(256, 256), 0.25f, SpriteEffects.None, 0f);
            }

            if (chargeProgress >= 0)
            {
                float addOffset = MathHelper.Lerp(-45, -5, chargeAlpha);
                indicatorColor = new Color(255,25,25) * MathHelper.Lerp(0, 0.75f, chargeAlpha);

                for (int i = 0; i < 4; i++)
                {
                    float angle = i * MathHelper.PiOver2;
                    Vector2 offset = randomVect + Vector2.Transform(new Vector2(0, addOffset), Matrix.CreateRotationZ(angle));
                    spriteBatch.Draw(_indicatorTexture, position + offset, null, indicatorColor, angle, new Vector2(256, 256), 0.25f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
