using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmotionalBasketGame.Actors.Buttons;

namespace EmotionalBasketGame.Actors
{
    public abstract class Ink_ConfigHandler_Base : Ink_Actor_Base
    {
        /// <summary>
        /// The text color for this config.
        /// </summary>
        public Color TextColor { get; set; } = Color.Black;

        /// <summary>
        /// The layer this button is on.
        /// </summary>
        public int ConfigLayer { get; set; } = 0;

        protected SpriteFont SettingsFont;
        protected Ink_Button_Base leftButton, rightButton;

        protected string configName;

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            SettingsFont = content.Load<SpriteFont>("Umeko");
        }

        /// <summary>
        /// Updates the actor's visuals.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">The sprite batch provided.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            Vector2 position = GetScreenPosition();
            float screenScale = GetScreenScale() * GetDepthScale();

            if (configName != "")
            {
                Vector2 textSize = SettingsFont.MeasureString(configName);
                spriteBatch.DrawString(SettingsFont, configName, position, TextColor, 0f, textSize * 0.5f, 1.0f * screenScale, SpriteEffects.None, 0f);
            }
        }

        /// <summary>
        /// Handles a button being clicked.
        /// </summary>
        /// <param name="button">The button clicked.</param>
        public abstract bool OnButtonClicked(Ink_Button_Base button);

        /// <summary>
        /// Applies the current config's settings.
        /// </summary>
        /// <param name="settings">The settings object.</param>
        public abstract void ApplySettings();
    }
}
