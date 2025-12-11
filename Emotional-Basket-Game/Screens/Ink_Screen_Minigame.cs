using EmotionalBasketGame.Actors;
using EmotionalBasketGame.Actors.HUDs;
using EmotionalBasketGame.Actors.Targets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ParticleSystemExample;
using System;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace EmotionalBasketGame.Screens
{
    public class Ink_Screen_Minigame : Ink_GameScreen_Base
    {
        private Ink_HUD_TargetReticle reticle;

        /// <summary>
        /// TODO: Switch later to a score system handling it.
        /// </summary>
        public Ink_Particle_Stars ScoreParticle { get; protected set; }

        // X and Y: How much of an offset the mouse needs to have.
        // Z and W: How much the screen actually moves if the offsets are at max.
        private Vector4 ScreenOffsetMultiplier = new Vector4(500f, 100f, 75f, 25f);

        public Ink_Screen_Minigame(Ink_PinGameManager game) : base(game)
        {
            BackgroundColor = Color.LightGray;
            ScoreParticle = new Ink_Particle_Stars(game, 20, []);
        }

        /// <summary>
        /// Loads game content for all current actors.
        /// </summary>
        /// <param name="graphics">The graphics device.</param>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            base.LoadContent(graphics, content);

            System.Random rand = new System.Random();
            reticle = (Ink_HUD_TargetReticle)OpenHUD(new Ink_HUD_TargetReticle(Game, this, 1));

            AddActor(new Ink_Emitter(ScoreParticle), Vector2.Zero, true, 0, 1);

            for (int i = 0; i < 10; i++)
                AddActor(new Ink_Target_Base(), new Vector2((float)rand.NextDouble() * 1240 - 620, (float)rand.NextDouble() * 680 - 340), true, 1);

            MusicManager.AddMusicNode([
                ([("Music/WAV_Drive_Normal_Loop", -1)], 0.1, 0.5),
                ([("Music/WAV_Drive_Muffled_Loop", -1)], 0.1, 0.3)],
                key: "Drive",
                nodeFadeTime: 1.0);
        }

        /// <summary>
        /// Handles a game Tick.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Math.Abs(ScreenOffsetMultiplier.Z) > 0 || Math.Abs(ScreenOffsetMultiplier.W) > 0)
            {
                Vector2 goalOffset = Vector2.Zero;

                if (reticle != null)
                {
                    MouseState currentMouseState = Mouse.GetState();
                    Vector2 screenCenter = GetScreenCenter();
                    float screenScale = GetScreenScale();

                    float x = (currentMouseState.X - screenCenter.X) / screenScale;
                    float y = (currentMouseState.Y - screenCenter.Y) / screenScale;

                    goalOffset.X = MathHelper.Lerp(-ScreenOffsetMultiplier.Z, ScreenOffsetMultiplier.Z, Ink_Math.EaseInOutCubic(0.5f + 0.5f * Math.Clamp(x / ScreenOffsetMultiplier.X, -1.0f, 1.0f)));
                    goalOffset.Y = MathHelper.Lerp(-ScreenOffsetMultiplier.W, ScreenOffsetMultiplier.W, Ink_Math.EaseInOutCubic(0.5f + 0.5f * Math.Clamp(y / ScreenOffsetMultiplier.Y, -1.0f, 1.0f)));
                }

                ScreenOffset = Ink_Math.VLerp(ScreenOffset, goalOffset, (float)(1.0f - Math.Pow(0.5, delta * 10)));
            }
        }
    }
}
