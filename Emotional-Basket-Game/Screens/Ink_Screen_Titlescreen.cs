using EmotionalBasketGame.Actors;
using EmotionalBasketGame.Actors.Buttons;
using EmotionalBasketGame.Actors.HUDs;
using EmotionalBasketGame.Screens;
using EmotionalBasketGame.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmotionalBasketGame.Screens
{
    public class Ink_Screen_Titlescreen : Ink_GameScreen_Base
    {
        private List<Ink_PointLight> _lights = new();

        private SoundEffectInstance switchOnInst;

        private Ink_ButtonHandler buttonHandler;
        private Ink_Desk_Intro desk;
        private Ink_HUD_Base mouse, createdBy;
        private Ink_Actor_Base backButton;

        private Vector2 currentScroll, prevScroll, goalScroll;
        private Vector2 currentMouseScroll;
        private double scrollProgress, scrollTime;

        private MouseState prevMouseState, currentMouseState;

        private double introProgress;

        // X = The amount of offset the mouse needs to have from the center.
        // Y = The actual amount the screen is moved.
        private Vector2 ScreenOffsetMultiplier = new Vector2(300, 50);

        /// <summary>
        /// A blend state used to simulate the darkness.
        /// </summary>
        BlendState multiplyBlend = new BlendState
        {
            // Color Blending (for RGB)
            ColorSourceBlend = Blend.DestinationColor, // Source (new color) * Destination (on screen)
            ColorDestinationBlend = Blend.Zero,        // Result is just the multiplication
            ColorBlendFunction = BlendFunction.Add,    // Standard addition for combining results

            // Alpha Blending (for transparency)
            AlphaSourceBlend = Blend.DestinationAlpha, // Source Alpha * Destination Alpha
            AlphaDestinationBlend = Blend.Zero,        // Result is just the multiplication
            AlphaBlendFunction = BlendFunction.Add     // Standard addition
        };

        public Ink_Screen_Titlescreen(Ink_PinGameManager game) : base(game)
        {
            BackgroundColor = Color.Black;
            _lights = new List<Ink_PointLight>();
        }

        /// <summary>
        /// Loads game content for all current actors.
        /// </summary>
        /// <param name="graphics">The graphics device.</param>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            base.LoadContent(graphics, content);

            InitSoundInstance(content, "WAV_SwitchOn", out switchOnInst);

            AddActor(new Ink_Button_Square(new Vector2(200, 200), buttonPathName: "T2D_PostItButton", clickSoundPathName: "WAV_Button_Joyous", hoverSoundPathName: "WAV_Button_Hover", buttonClickDel: OnStart), new Vector2(-360,-128), true, renderPriority: 1);
            AddActor(new Ink_Button_Circular(200f, buttonPathName: "T2D_Button_Settings", clickSoundPathName: "WAV_Button_Scroll", hoverSoundPathName: "WAV_Button_Hover", scale: 0.5f, buttonClickDel: MoveToSettings), new Vector2(0, 75), true, renderPriority: 1);
            AddActor(new Ink_Button_Bomb(150f, buttonPathName: "T2D_Button_Quit", clickSoundPathName: "WAV_Target_Miss", hoverSoundPathName: "WAV_Button_Hover", scale: 0.5f, buttonClickDel: OnQuit), new Vector2(-240, 150), true, renderPriority: 1);

            AddActor(new Ink_Config_Resolution() { ConfigLayer = 1 }, new Vector2(-400, -725), true, renderPriority: 1);
            AddActor(new Ink_Config_FPS() { ConfigLayer = 1 }, new Vector2(-100, -725), true, renderPriority: 1);
            AddActor(new Ink_Config_Fullscreen() { ConfigLayer = 1 }, new Vector2(-425, -575), true, renderPriority: 1);
            AddActor(new Ink_Button_Square(new Vector2(200, 100), buttonPathName: "T2D_Button_SaveConfigs", clickSoundPathName: "WAV_Options_Progress", hoverSoundPathName: "WAV_Button_Hover", buttonClickDel: SaveConfigs, scale: 0.5f) { ButtonLayer = 1 }, new Vector2(-100, -500), true, renderPriority: 1);
            backButton = AddActor(new Ink_Button_Square(new Vector2(200, 100), buttonPathName: "T2D_Button_Back", clickSoundPathName: "WAV_Button_Scroll", hoverSoundPathName: "WAV_Button_Hover", buttonClickDel: MoveToMain, scale: 0.5f) { ButtonLayer = 1, IsHidden = true }, new Vector2(-225, -400), true, renderPriority: 1);

            AddLight(new Ink_PointLight(Color.White, "T2D_Spotlight", 6.0f), new Vector2(-256, 0), true, 0, this);
            AddLight(new Ink_PointLight(Color.White, "T2D_Spotlight", 4.0f), new Vector2(-256, -650), true, 0, this);

            createdBy = OpenHUD(new Ink_HUD_CreatedBy(Game, this));

            desk = (Ink_Desk_Intro)AddActor(new Ink_Desk_Intro(), new Vector2(50, -350), true, 0);
            if (desk != null)
                desk.TargetGlow = AddLight(new Ink_PointLight(Color.Yellow, "T2D_Spotlight", 2.0f), new Vector2(50, -256), true, 0, desk);

            MusicManager.AddMusicNode([
                ([("Music/WAV_OldTricks_Intro", 8.29), ("Music/WAV_OldTricks_Loop", -1)], 0.1, 0.5)],
                key: "Titlescreen",
                nodeFadeTime: 1.0);
        }

        /// <summary>
        /// Spawns an actor and adds it to the list.
        /// </summary>
        /// <param name="actor">The actor to initialize.</param>
        public Ink_PointLight AddLight(Ink_PointLight light, Vector2 actorPosition, bool doLoad, double depth, Object lightOwner)
        {
            light.Game = Game;
            light.World = this;
            light.Position = actorPosition;
            light.Depth = depth;
            light.Owner = lightOwner;
            _lights.Add(light);

            if (doLoad)
                light.LoadContent(Content);

            return light;
        }

        /// <summary>
        /// Handles a game Tick.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            double delta = gameTime.ElapsedGameTime.TotalSeconds;
            double prev = introProgress;
            introProgress += delta;

            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (introProgress < 7 && Game.IsActive && prevMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                PlaySoundInst(switchOnInst, Ink_RandomHelper.RandRange(0.4f, 0.45f), Ink_RandomHelper.RandRange(-0.1f, 0.1f));
                SkipToTitle();
                prev = 7.0;
                introProgress = 7.0;
            }

            if (prev < 7 && introProgress >= 7)
            {
                createdBy = null;
                PlaySoundInst(switchOnInst, Ink_RandomHelper.RandRange(0.4f, 0.45f), Ink_RandomHelper.RandRange(-0.1f, 0.1f));

                if (desk != null)
                    desk.LightsOn = true;
            }
            if (prev < 8 && introProgress >= 8)
            {
                AddActor(new Ink_Button_HiddenSound("WAV_Boop", 16), new Vector2(310, 256));
                AddActor(new Ink_Button_HiddenSound("WAV_Boop", 16), new Vector2(290, -180));
                AddActor(new Ink_Button_HiddenSound("WAV_Boop", 16), new Vector2(600, -170));

                buttonHandler = new Ink_ButtonHandler(Game, this, Actors);
                mouse = OpenHUD(new Ink_HUD_PencilMouse(Game, this));
                buttonHandler.MouseActive = true;
            }

            if (introProgress >= 8)
            {
                //Handles the visual offset of moving the mouse around.
                if (ScreenOffsetMultiplier.X > 0 && ScreenOffsetMultiplier.Y > 0)
                {
                    Vector2 goalOffset = Vector2.Zero;

                    if (scrollProgress < scrollTime)
                    {
                        scrollProgress = Math.Min(scrollProgress + delta, scrollTime);
                        float alpha = (float)(scrollProgress / scrollTime);
                        currentScroll = Ink_Math.VLerp(prevScroll, goalScroll, Ink_Math.EaseOutBack(alpha));

                        if (scrollProgress >= scrollTime)
                        {
                            mouse = OpenHUD(new Ink_HUD_PencilMouse(Game, this));
                            buttonHandler.MouseActive = true;
                            if (backButton != null)
                                backButton.IsHidden = buttonHandler.SceneLayer != 1;
                        }
                    }
                    else if (mouse != null)
                    {
                        Vector2 screenCenter = GetScreenCenter();
                        float screenScale = GetScreenScale();

                        float x = (currentMouseState.X - screenCenter.X) / screenScale;
                        float y = (currentMouseState.Y - screenCenter.Y) / screenScale;
                        Vector2 mousePos = new Vector2(x, y);

                        float dist = (float)Math.Sqrt(Math.Pow(mousePos.X, 2) + Math.Pow(mousePos.Y, 2));
                        float alpha = Ink_Math.EaseOutCubic(Math.Min(dist / ScreenOffsetMultiplier.X, 1.0f));

                        Vector2 normalizedPos = mousePos;
                        normalizedPos.Normalize();

                        goalOffset = normalizedPos * ScreenOffsetMultiplier.Y * alpha;
                    }

                    currentMouseScroll = Ink_Math.VLerp(currentMouseScroll, goalOffset, (float)(1.0f - Math.Pow(0.5, delta*10)));
                    ScreenOffset = currentMouseScroll + currentScroll;
                }
            }

            if (buttonHandler != null)
                buttonHandler.Update(gameTime);
        }

        /// <summary>
        /// Handles drawing the screen.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            RenderTarget2D lightMask;

            //Create a darkness mask.
            float screenScale = GetScreenScale();
            lightMask = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            GraphicsDevice.SetRenderTarget(lightMask);

            float lightAlpha = Ink_Math.EaseOutBack((float)MathHelper.Clamp((float)(introProgress - 7f) * 2, 0.0f, 1.0f));
            GraphicsDevice.Clear(Color.White * MathHelper.Lerp(0.075f, 0.6f, lightAlpha));

            //Draw all lights
            spriteBatch.Begin(blendState: BlendState.Additive);

            foreach (var light in _lights)
            {
                if (light.Owner == this)
                    light.SizeAlpha = lightAlpha;

                light.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            base.Draw(spriteBatch, gameTime);

            if (lightMask != null)
            {
                spriteBatch.Begin(blendState: multiplyBlend);
                spriteBatch.Draw(lightMask, Vector2.Zero, Color.White);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Skips to the titlescreen.
        /// </summary>
        public void SkipToTitle()
        {
            DoScreenShake(new Vector2(2.5f, 2.5f), 0.75f, 0.0f, 0.5f);
            OpenHUD(new Ink_HUD_Flashbang(Game, this, 1) { FlashTime = 0.5 });
            CloseHUD(createdBy);
            createdBy = null;

            MusicManager.StopTrack("Titlescreen", 0.1);
            MusicManager.AddMusicNode([
                ([("Music/WAV_OldTricks_Loop", -1)], 0.1, 0.5)],
                key: "Titlescreen_Skipped",
            nodeFadeTime: 1.0);

            if (desk != null)
                desk.LightsOn = true;
        }

        /// <summary>
        /// Handles when the "start" button is clicked.
        /// </summary>
        /// <param name="button">The button clicked.</param>
        public bool OnStart(Ink_Button_Base button)
        {
            buttonHandler.CleanUp();
            buttonHandler = null;
            CloseHUD(mouse);
            mouse = null;

            MusicManager.StopTrack("Titlescreen", 0.5);
            MusicManager.StopTrack("Titlescreen_Skipped", 0.5);

            Game.RunLoad(this, new Ink_Screen_Minigame(Game), new Ink_ScreenTransition_Shutters(Game, 0.5, 3, Color.Black), new Ink_ScreenTransition_Fade(Game, 0.5, Color.Black), 0.5);

            return true;
        }

        /// <summary>
        /// Handles when the "Back to Main" button is clicked.
        /// </summary>
        /// <param name="button">The button clicked.</param>
        public bool MoveToMain(Ink_Button_Base button)
        {
            if (buttonHandler != null)
                buttonHandler.SceneLayer = 0;

            SetScroll(Vector2.Zero, 1.0);
            return true;
        }

        /// <summary>
        /// Handles when the "start" button is clicked.
        /// </summary>
        /// <param name="button">The button clicked.</param>
        public bool MoveToSettings(Ink_Button_Base button)
        {
            if (buttonHandler != null)
                buttonHandler.SceneLayer = 1;

            SetScroll(new Vector2(-256, -650), 1.0);
            return true;
        }

        /// <summary>
        /// Sets the screen center scroll.
        /// </summary>
        /// <param name="newScroll"></param>
        /// <param name="time"></param>
        public void SetScroll(Vector2 newScroll, double time)
        {
            buttonHandler.MouseActive = false;
            CloseHUD(mouse);
            mouse = null;

            prevScroll = currentScroll;
            goalScroll = newScroll;
            scrollProgress = 0;
            scrollTime = time;
        }

        /// <summary>
        /// Handles when the "Save Configs" button is clicked.
        /// </summary>
        /// <param name="button">The button clicked.</param>
        public bool SaveConfigs(Ink_Button_Base button)
        {
            foreach (Ink_Actor_Base a in Actors)
            {
                if (a is Ink_ConfigHandler_Base handler)
                    handler.ApplySettings();
            }

            Game.SaveSettings();
            return true;
        }

        /// <summary>
        /// Handles when the "Quit" button is clicked.
        /// </summary>
        /// <param name="button">The button clicked.</param>
        public bool OnQuit(Ink_Button_Base button)
        {
            Game.Exit();
            return true;
        }

        public override Ink_Actor_Base AddActor(Ink_Actor_Base actor, Vector2 actorPosition, bool doLoad, double depth = 0, int renderPriority = 0)
        {
            actor = base.AddActor(actor, actorPosition, doLoad, depth, renderPriority);

            if (actor is Ink_Button_Base button && buttonHandler != null)
                buttonHandler = new Ink_ButtonHandler(Game, this, Actors);

            return actor;
        }
    }
}
