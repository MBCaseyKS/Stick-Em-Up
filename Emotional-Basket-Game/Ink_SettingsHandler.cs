
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text.Json;

namespace EmotionalBasketGame
{
    /// <summary>
    /// Loads and handles settings.
    /// </summary>
    public class Ink_SettingsHandler
    {
        private Ink_GameSettings currentSettings = null;
        private const string settingsPath = "settings.json";

        /// <summary>
        /// Applies game settings.
        /// </summary>
        /// <param name="graphics">The graphics device.</param>
        public void ApplyGraphicsSettings(Game game, GraphicsDeviceManager graphics)
        {
            if (currentSettings == null)
                currentSettings = GetSettings();
            if (currentSettings == null) 
                return;

            game.IsFixedTimeStep = currentSettings.FPS > 0;
            game.TargetElapsedTime = game.IsFixedTimeStep ? TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / currentSettings.FPS)) : TimeSpan.FromTicks(166667L);

            graphics.PreferredBackBufferWidth = (int)currentSettings.ScreenWidth;
            graphics.PreferredBackBufferHeight = (int)currentSettings.ScreenHeight;
            graphics.IsFullScreen = currentSettings.IsFullscreen;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Saves the player settings.
        /// </summary>
        public void SaveSettings()
        {
            if (currentSettings == null) return;

            try
            {
                string serializedText = JsonSerializer.Serialize<Ink_GameSettings>(currentSettings);
                File.WriteAllText(settingsPath, serializedText);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Tries to load the player's settings. If failed, returns default settings.
        /// </summary>
        /// <returns>The filled in settings.</returns>
        public Ink_GameSettings GetSettings(bool alwaysLoadNew = false)
        {
            if (currentSettings != null && !alwaysLoadNew)
                return currentSettings;

            try
            {
                var fileContents = File.ReadAllText(settingsPath);
                Ink_GameSettings deserializedContent = JsonSerializer.Deserialize<Ink_GameSettings>(fileContents);
                return deserializedContent;
            }
            catch (Exception)
            {
                //Failed to load customized settings, we'll move onto a default one.
            }

            return new Ink_GameSettings();
        }
    }
}
