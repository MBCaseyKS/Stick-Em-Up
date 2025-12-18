using EmotionalBasketGame.Actors.Buttons;
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
    /// Displays game results.
    /// </summary>
    public class Ink_HUD_Results : Ink_HUD_Base
    {
        /// <summary>
        /// The round handler.
        /// </summary>
        public Ink_TargetRound_Layout RoundHandler { get; set; }

        /// <summary>
        /// Handles when the options to return or restart are selected.
        /// </summary>
        /// <param name="option">The option selected.</param>
        public delegate void OnMenuSelect(int option);
        public OnMenuSelect MenuSelectDel { get; set; }

        private MouseState prevState, currentState;

        private SpriteFont umekoLarge;
        private SoundEffectInstance statsSfx;
        private Ink_HUD_Base mouseHud;
        private float progress;

        private int personalBestTargets = 0;
        private bool isHighScore;

        public Ink_HUD_Results(Ink_PinGameManager game, Ink_GameScreen_Base activeScreen, int renderPriority = 0) : base(game, activeScreen, renderPriority) { }

        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            umekoLarge = content.Load<SpriteFont>("Umeko_Big");
            InitSoundInstance(content, "WAV_Stats_Appear", out statsSfx);

            UpdateHighScore();
        }

        public override void Update(GameTime gameTime)
        {
            double prev = progress;
            progress = progress + (float)gameTime.ElapsedGameTime.TotalSeconds;

            prevState = currentState;
            currentState = Mouse.GetState();

            if (progress >= 0.5 && progress < 3.5 && prevState.LeftButton == ButtonState.Released && currentState.LeftButton == ButtonState.Pressed)
            {
                prev = 3.5f;
                progress = 3.5f;
                ShowButtons();
            }

            if (progress >= 1.5 && progress < 3.0 && prev % 0.5 > progress % 0.5)
                PlaySoundInst(statsSfx, Ink_RandomHelper.RandRange(0.5f, 0.6f), Ink_RandomHelper.RandRange(-0.1f, 0.1f));
            if (prev < 3.5 && progress >= 3.5)
                ShowButtons();
        }

        /// <summary>
        /// Shows all the buttons.
        /// </summary>
        public void ShowButtons()
        {
            mouseHud = ActiveScreen.OpenHUD(new Ink_HUD_PencilMouse(Game, ActiveScreen, 2));

            //Replay
            ActiveScreen.AddActor(new Ink_Button_Square_Glowy(new Vector2(100, 75),
                buttonPathName: "T2D_Button_Basic",
                hoverSoundPathName: "WAV_Button_Hover",
                clickSoundPathName: "WAV_Button_Scroll",
                buttonClickDel: OnReplay,
                scale: 1.5f,
                buttonMsg: "AGAIN!",
                textColor: Color.Black,
                textScale: 1f), new Vector2(-150, 125), true);

            //Home
            ActiveScreen.AddActor(new Ink_Button_Square(new Vector2(100, 75), 
                buttonPathName: "T2D_Button_Basic", 
                hoverSoundPathName: "WAV_Button_Hover",
                clickSoundPathName: "WAV_Button_Joyous",
                buttonClickDel: MoveToMain,
                scale: 1.5f,
                buttonMsg: "HOME",
                textColor: Color.Black,
                textScale: 1f), new Vector2(150, 125), true);
        }

        /// <summary>
        /// Handles when the "Back to Main" button is clicked.
        /// </summary>
        /// <param name="button">The button clicked.</param>
        public bool OnReplay(Ink_Button_Base button)
        {
            if (MenuSelectDel != null)
                MenuSelectDel(0);

            DoClose();
            return true;
        }

        /// <summary>
        /// Handles when the "Back to Main" button is clicked.
        /// </summary>
        /// <param name="button">The button clicked.</param>
        public bool MoveToMain(Ink_Button_Base button)
        {
            if (MenuSelectDel != null)
                MenuSelectDel(1);

            DoClose();
            return true;
        }

        /// <summary>
        /// Updates the high score with the most targets hit.
        /// </summary>
        public void UpdateHighScore()
        {
            Ink_SaveBitStorage storage = Ink_SettingsHandler.GetBitStorage();
            personalBestTargets = Ink_SettingsHandler.GetBitValue(storage, "MinigameHighScore");
            
            if (RoundHandler.TargetsHit > 0 && RoundHandler.TargetsHit > personalBestTargets)
            {
                isHighScore = true;
                personalBestTargets = RoundHandler.TargetsHit;
                Ink_SettingsHandler.SetBitValue(storage, "MinigameHighScore", personalBestTargets);
            }
        }

        public override void DoClose()
        {
            if (mouseHud != null)
                mouseHud.DoClose();

            base.DoClose();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (umekoLarge != null)
            {
                //THAT'S IT!
                string msg = "THAT'S IT!";
                float alpha = Ink_Math.EaseOutCubic(MathHelper.Clamp((progress - 0.75f) * 2f, 0.0f, 1.0f));
                Vector2 position = Ink_Math.VLerp(new Vector2(640, 360), new Vector2(640, 75), alpha);
                float scale = MathHelper.Lerp(1.0f, 0.25f, alpha);

                Vector2 offset = Vector2.Zero;
                if (progress < 0.5)
                    offset = Ink_RandomHelper.VRand(MathHelper.Lerp(10f, 0f, progress / 0.5f));

                Vector2 textSize = umekoLarge.MeasureString(msg);
                spriteBatch.DrawString(umekoLarge, msg, position + offset, Color.White, 0f, textSize * 0.5f, scale, SpriteEffects.None, 0);

                //Darts thrown
                if (progress >= 1.5)
                {
                    msg = "DARTS THROWN:";
                    textSize = umekoLarge.MeasureString(msg);
                    position = new Vector2(640, 200);
                    scale = 0.25f;
                    offset = new Vector2(-25, 0);

                    if (progress < 1.75)
                        offset += Ink_RandomHelper.VRand(MathHelper.Lerp(5f, 0f, MathHelper.Clamp((progress - 1.5f) * 4f, 0.0f, 1.0f)));

                    spriteBatch.DrawString(umekoLarge, msg, position + offset, Color.White, 0f, new Vector2(textSize.X, textSize.Y * 0.5f), scale, SpriteEffects.None, 0);

                    msg = RoundHandler.DartsThrown.ToString();
                    offset += new Vector2(50, 0);
                    spriteBatch.DrawString(umekoLarge, msg, position + offset, Color.White, 0f, new Vector2(0, textSize.Y * 0.5f), scale, SpriteEffects.None, 0);

                    //Targets hit
                    if (progress >= 2.0)
                    {
                        msg = "TARGETS HIT:";
                        textSize = umekoLarge.MeasureString(msg);
                        position.Y += 75.0f;
                        offset = new Vector2(-25, 0);

                        if (progress < 2.25)
                            offset += Ink_RandomHelper.VRand(MathHelper.Lerp(5f, 0f, MathHelper.Clamp((progress - 2.0f) * 4f, 0.0f, 1.0f)));

                        spriteBatch.DrawString(umekoLarge, msg, position + offset, Color.White, 0f, new Vector2(textSize.X, textSize.Y * 0.5f), scale, SpriteEffects.None, 0);

                        msg = RoundHandler.TargetsHit.ToString();
                        offset += new Vector2(50, 0);
                        spriteBatch.DrawString(umekoLarge, msg, position + offset, Color.White, 0f, new Vector2(0, textSize.Y * 0.5f), scale, SpriteEffects.None, 0);

                        //Accuracy
                        if (progress >= 2.5)
                        {
                            msg = "THROW ACCURACY:";
                            textSize = umekoLarge.MeasureString(msg);
                            position.Y += 75.0f;
                            offset = new Vector2(-25, 0);

                            if (progress < 2.75)
                                offset += Ink_RandomHelper.VRand(MathHelper.Lerp(5f, 0f, MathHelper.Clamp((progress - 2.5f) * 4f, 0.0f, 1.0f)));

                            spriteBatch.DrawString(umekoLarge, msg, position + offset, Color.White, 0f, new Vector2(textSize.X, textSize.Y * 0.5f), scale, SpriteEffects.None, 0);

                            msg = ((RoundHandler.DartsThrown > 0 ? (int)(100.0 - (100.0 * ((double)RoundHandler.DartsMissed / (double)RoundHandler.DartsThrown))) : 0.0)) + "%";
                            offset += new Vector2(50, 0);
                            spriteBatch.DrawString(umekoLarge, msg, position + offset, Color.White, 0f, new Vector2(0, textSize.Y * 0.5f), scale, SpriteEffects.None, 0);

                            if (progress >= 3.5)
                            {
                                msg = "HIGH SCORE: " + personalBestTargets;
                                textSize = umekoLarge.MeasureString(msg);
                                position.Y = 575f;
                                offset = Vector2.Zero;

                                if (isHighScore)
                                    offset += Ink_RandomHelper.VRand(1.5f);

                                spriteBatch.DrawString(umekoLarge, msg, position + offset, isHighScore ? new Color(255,236,80) : Color.White, 0f, textSize * 0.5f, scale, SpriteEffects.None, 0);
                            }
                        }
                    }
                }
            }
        }
    }
}
