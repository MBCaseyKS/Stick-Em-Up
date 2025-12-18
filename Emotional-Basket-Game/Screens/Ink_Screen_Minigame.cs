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
        private Ink_TargetRound_Layout roundHandler;

        /// <summary>
        /// TODO: Switch later to a score system handling it.
        /// </summary>
        public Ink_Particle_Stars ScoreParticle { get; protected set; }

        /// <summary>
        /// The time to wait when something is happening.
        /// </summary>
        public double PauseTime { get; set; }

        // X and Y: How much of an offset the mouse needs to have.
        // Z and W: How much the screen actually moves if the offsets are at max.
        private Vector4 ScreenOffsetMultiplier = new Vector4(500f, 100f, 75f, 25f);

        private int introProgress;

        public Ink_Screen_Minigame(Ink_PinGameManager game) : base(game)
        {
            BackgroundColor = Color.Black;
            ScoreParticle = new Ink_Particle_Stars(game, 20, []);
            PauseTime = 1.5;
            introProgress = 0;
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
            roundHandler = new Ink_TargetRound_Layout(this, this.OnRoundEvent, true);
            roundHandler.LoadContent(content);
            roundHandler.OnRefresh();

            reticle = (Ink_HUD_TargetReticle)OpenHUD(new Ink_HUD_TargetReticle(Game, this, 1));
            reticle.RoundHandler = roundHandler;

            AddActor(new Ink_Actor_Billboard(), new Vector2(0, -128), true, 33, -1);
            AddActor(new Ink_OfficeBackground("T_Map_OfficeBG.txt"), new Vector2(-1152, -616), true, 33, -2);
            AddActor(new Ink_Emitter(ScoreParticle), Vector2.Zero, true, 0, 1);

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

            if (PauseTime > 0)
            {
                PauseTime -= delta;
                if (PauseTime <= 0)
                {
                    if (introProgress < 1)
                    {
                        introProgress++;
                        if (roundHandler != null)
                            roundHandler.InitializeTargets();
                    }
                }
            }
            else if (roundHandler != null)
                roundHandler.TickRound(gameTime);
        }

        /// <summary>
        /// Is called when an event in the round occurs.
        /// </summary>
        /// <param name="round">The round handler.</param>
        /// <param name="data">The event.</param>
        public void OnRoundEvent(Ink_TargetRound_Layout round, RoundEventData data)
        {
            if (round != roundHandler) return;

            if (data == RoundEventData.WasFailed)
            {
                MusicManager.SetTrackLayer("Drive", 1);
                CloseHUD(reticle);
                reticle = null;

                Ink_Screen_Results newScreen = (Ink_Screen_Results)Game.AddScreen(new Ink_Screen_Results(Game, 1) { OverlayScreen = this, RoundHandler = roundHandler, ScreenClosedDel = OnResultsClosed });
            }
            else if (data == RoundEventData.WasRestarted)
            {
                if (reticle == null)
                    reticle = (Ink_HUD_TargetReticle)OpenHUD(new Ink_HUD_TargetReticle(Game, this, 1) { RoundHandler = roundHandler });

                for (int i = 0; i < _actors.Count; i++)
                {
                    if (_actors[i] is Ink_Target_Base || _actors[i] is Ink_Dart)
                    {
                        DestroyActor(_actors[i]);
                        i--;
                        continue;
                    }
                }

                OpenHUD(new Ink_HUD_Flashbang(Game, this, 0) { BackgroundColor = Color.Black, FlashTime = 0.5f });
                MusicManager.SetTrackLayer("Drive", 0);
                roundHandler.OnRefresh();
                roundHandler.InitializeTargets();
                PauseTime = 1.0;
            }
        }

        /// <summary>
        /// Is called when the results screen is closed.
        /// </summary>
        /// <param name="screen">The results screen.</param>
        public void OnResultsClosed(Ink_GameScreen_Base screen)
        {
            if (roundHandler != null)
                roundHandler.TriggerRoundDelegate(RoundEventData.WasRestarted);
        }
    }
}
