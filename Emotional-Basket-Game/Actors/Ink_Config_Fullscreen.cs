using EmotionalBasketGame.Actors.Buttons;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// Handles fullscreen.
    /// </summary>
    public class Ink_Config_Fullscreen : Ink_ConfigHandler_Base
    {
        private List<bool> options =
        [
            false,
            true
        ];

        private Ink_GameSettings gameSettings;
        private int selectedIdx;

        public Ink_Config_Fullscreen()
        {
            configName = "Fullscreen";
        }

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            gameSettings = Game.GetSettings();
            selectedIdx = options.IndexOf(gameSettings.IsFullscreen);
            if (selectedIdx < 0)
                selectedIdx = 0;

            leftButton = (Ink_Button_Base)World.AddActor(new Ink_Button_Square(new Vector2(128), buttonPathName: "T2D_Arrow_Settings_Inverted", hoverSoundPathName: "WAV_Button_Hover", clickSoundPathName: "WAV_Options_Progress", buttonClickDel: OnButtonClicked) { ButtonLayer = ConfigLayer, Scale = 0.25f }, Position + new Vector2(-100, 64), true, Depth, RenderPriority);
            rightButton = (Ink_Button_Base)World.AddActor(new Ink_Button_Square(new Vector2(128), buttonPathName: "T2D_Arrow_Settings", hoverSoundPathName: "WAV_Button_Hover", clickSoundPathName: "WAV_Options_Progress", buttonClickDel: OnButtonClicked) { ButtonLayer = ConfigLayer, Scale = 0.25f }, Position + new Vector2(100, 64), true, Depth, RenderPriority);
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
                string displayStr = selectedIdx == 1 ? "Enabled" : "Disabled";
                Vector2 textSize = SettingsFont.MeasureString(displayStr);
                spriteBatch.DrawString(SettingsFont, displayStr, position, TextColor, 0f, textSize * 0.5f, 0.75f * screenScale, SpriteEffects.None, 0f);
            }
        }
        public override void ApplySettings()
        {
            if (gameSettings == null) return;
            gameSettings.IsFullscreen = options[selectedIdx];
        }

        public override bool OnButtonClicked(Ink_Button_Base button)
        {
            if (button == leftButton)
            {
                selectedIdx = (selectedIdx <= 0) ? options.Count - 1 : (selectedIdx - 1);
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
