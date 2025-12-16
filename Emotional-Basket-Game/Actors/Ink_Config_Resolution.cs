using EmotionalBasketGame.Actors.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// Handles screen resolution.
    /// </summary>
    public class Ink_Config_Resolution : Ink_ConfigHandler_Base
    {
        private List<Vector2> options =
        [
            new Vector2(854, 480),
            new Vector2(1280, 720),
            new Vector2(1600, 900),
            new Vector2(1920, 1080),
            new Vector2(2560, 1440),
            new Vector2(3840, 2160)
        ];

        private Ink_GameSettings gameSettings;
        private int selectedIdx;

        public Ink_Config_Resolution()
        {
            configName = "Screen Resolution";
        }

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            gameSettings = Game.GetSettings();

            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].X == gameSettings.ScreenWidth && options[i].Y == gameSettings.ScreenHeight)
                {
                    selectedIdx = i;
                    break;
                }
            }

            if (selectedIdx < 0)
                selectedIdx = 1;

            leftButton = (Ink_Button_Base)World.AddActor(new Ink_Button_Square(new Vector2(128), buttonPathName: "T2D_Arrow_Settings_Inverted", hoverSoundPathName: "WAV_Button_Hover", clickSoundPathName: "WAV_Options_Progress", buttonClickDel: OnButtonClicked) { ButtonLayer = ConfigLayer, Scale = 0.25f }, Position + new Vector2(-125, 64), true, Depth, RenderPriority);
            rightButton = (Ink_Button_Base)World.AddActor(new Ink_Button_Square(new Vector2(128), buttonPathName: "T2D_Arrow_Settings", hoverSoundPathName: "WAV_Button_Hover", clickSoundPathName: "WAV_Options_Progress", buttonClickDel: OnButtonClicked) { ButtonLayer = ConfigLayer, Scale = 0.25f }, Position + new Vector2(125, 64), true, Depth, RenderPriority);
        }

        /// <summary>
        /// Updates the actor's visuals.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">The sprite batch provided.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            float screenScale = GetScreenScale() * GetDepthScale();
            Vector2 position = GetScreenPosition() + new Vector2(0, 64 * screenScale);

            if (options != null)
            {
                string displayStr = options[selectedIdx].X + " x " + options[selectedIdx].Y;
                Vector2 textSize = SettingsFont.MeasureString(displayStr);
                spriteBatch.DrawString(SettingsFont, displayStr, position, TextColor, 0f, textSize * 0.5f, 0.75f * screenScale, SpriteEffects.None, 0f);
            }
        }

        public override void ApplySettings()
        {
            if (gameSettings == null) return;
            gameSettings.ScreenWidth = (int)options[selectedIdx].X;
            gameSettings.ScreenHeight = (int)options[selectedIdx].Y;
        }

        public override bool OnButtonClicked(Ink_Button_Base button)
        {
            if (button == leftButton)
            {
                selectedIdx = (selectedIdx <= 0) ? options.Count-1 : (selectedIdx - 1);
                return true;
            }
            else if (button == rightButton)
            {
                selectedIdx = (selectedIdx + 1) % options.Count;
                return true;
            }

            return false;
        }
    }
}
