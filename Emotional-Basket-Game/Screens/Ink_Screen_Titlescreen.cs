using EmotionalBasketGame.Actors;
using EmotionalBasketGame.Actors.Buttons;
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

        private Texture2D createdByTexture;
        private SoundEffectInstance switchOnInst;

        private Ink_ButtonHandler buttonHandler;
        private Ink_Desk_Intro desk;
        private Ink_Actor_Base mouse;

        private MouseState prevMouseState, currentMouseState;

        private double introProgress;
        private double skipAlpha;

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

            createdByTexture = Content.Load<Texture2D>("T2D_CreatedBy");
            InitSoundInstance(content, "WAV_SwitchOn", out switchOnInst);

            AddActor(new Ink_Button_Square(new Vector2(200, 200), buttonPathName: "T2D_PostItButton", clickSoundPathName: "WAV_Button_Joyous", hoverSoundPathName: "WAV_Button_Hover", buttonClickDel: OnStart), new Vector2(-256,0));

            AddLight(new Ink_PointLight(Color.White, "T2D_Spotlight", 6.0f), new Vector2(-256, 0), true, 0, this);

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
        public Ink_PointLight AddLight(Ink_PointLight light, Vector2 actorPosition, bool doLoad, int depth, Object lightOwner)
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

            if (skipAlpha > 0)
                skipAlpha = MathHelper.Max((float)(skipAlpha - delta), 0.0f);

            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (introProgress < 7 && Game.IsActive && prevMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                PlaySoundInst(switchOnInst, Ink_RandomHelper.RandRange(0.4f, 0.45f), Ink_RandomHelper.RandRange(-0.1f, 0.1f));
                SkipToTitle();
                prev = 7.0;
                introProgress = 7.0;
                skipAlpha = 1.0;
            }

            if (prev < 7 && introProgress >= 7)
            {
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
                mouse = AddActor(new Ink_MouseRep(), Vector2.Zero, true, 0);
            }

            if (introProgress >= 8)
            {
                //Handles the visual offset of moving the mouse around.
                if (ScreenOffsetMultiplier.X > 0 && ScreenOffsetMultiplier.Y > 0)
                {
                    Vector2 goalOffset = Vector2.Zero;

                    if (mouse != null)
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

                    ScreenOffset = Ink_Math.VLerp(ScreenOffset, goalOffset, (float)(1.0f - Math.Pow(0.5, delta*10)));
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

            //Render the intro "hud"
            if (introProgress < 5.0)
            {
                spriteBatch.Begin();

                //Fade
                float alpha = 1.0f - MathHelper.Clamp((float)(introProgress - 4f), 0.0f, 1.0f);
                spriteBatch.Draw(backgroundTexture, GetScreenRectangle(), new Color(0, 0, 0, 1.0f * alpha));

                //Created by
                if (createdByTexture != null)
                {
                    alpha = MathHelper.Clamp((float)(introProgress - 0.25f), 0.0f, 1.0f) - MathHelper.Clamp((float)(introProgress - 4f), 0.0f, 1.0f);
                    Rectangle source = new Rectangle(0, gameTime.TotalGameTime.TotalSeconds % 0.5 > 0.25 ? 512 : 0, 1024, 512);
                    spriteBatch.Draw(createdByTexture, GetScreenCenter(), source, Color.White * alpha, 0, new Vector2(512, 256), 0.5f * screenScale, SpriteEffects.None, 0f);
                }

                spriteBatch.End();
            }

            //Render the flash if you skip to the title.
            if (skipAlpha > 0)
            {
                spriteBatch.Begin();

                float alpha = (float)Math.Pow(skipAlpha, 2);
                spriteBatch.Draw(backgroundTexture, GetScreenRectangle(), Color.White * alpha);

                spriteBatch.End();
            }
        }

        /// <summary>
        /// Skips to the titlescreen.
        /// </summary>
        public void SkipToTitle()
        {
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
            DestroyActor(mouse);
            mouse = null;

            MusicManager.StopTrack("Titlescreen", 0.5);
            MusicManager.StopTrack("Titlescreen_Skipped", 0.5);

            Game.RunLoad(this, new Ink_Screen_Minigame(Game), new Ink_ScreenTransition_Shutters(Game, 0.5, 3, Color.Black), new Ink_ScreenTransition_Fade(Game, 0.5, Color.Black), 0.5);

            return true;
        }

        public override Ink_Actor_Base AddActor(Ink_Actor_Base actor, Vector2 actorPosition, bool doLoad, int depth)
        {
            actor = base.AddActor(actor, actorPosition, doLoad, depth);

            if (actor is Ink_Button_Base button && buttonHandler != null)
                buttonHandler = new Ink_ButtonHandler(Game, this, Actors);

            return actor;
        }
    }
}
