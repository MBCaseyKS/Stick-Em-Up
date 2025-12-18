
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
        private const string bitPath = "savebits.json";

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

        /// <summary>
        /// Tries to load the player's save bits. If failed, returns a fresh storage.
        /// </summary>
        /// <returns>The filled in settings.</returns>
        public static Ink_SaveBitStorage GetBitStorage()
        {
            try
            {
                var fileContents = File.ReadAllText(bitPath);
                Ink_SaveBitStorage deserializedContent = JsonSerializer.Deserialize<Ink_SaveBitStorage>(fileContents, options: new JsonSerializerOptions() { IncludeFields = true });
                return deserializedContent;
            }
            catch (Exception)
            {
                //Failed to load customized settings, we'll move onto a default one.
            }

            return new Ink_SaveBitStorage();
        }

        /// <summary>
        /// Gets a bit's value.
        /// </summary>
        /// <param name="storage">The bit storage.</param>
        /// <returns>The found value.</returns>
        public static int GetBitValue(Ink_SaveBitStorage storage, string id)
        {
            if (storage == null) return 0;
            if (storage.Values == null) return 0;

            id = id.ToLower();
            for (int i = 0; i < storage.Values.Count; i++)
            {
                if (storage.Values[i].id.ToLower() == id)
                    return storage.Values[i].value;
            }

            return 0;
        }

        /// <summary>
        /// Sets a bit's value.
        /// </summary>
        /// <param name="storage">The bit storage.</param>
        /// <returns>The new value.</returns>
        public static void SetBitValue(Ink_SaveBitStorage storage, string id, int value)
        {
            if (storage == null)
            {
                storage = GetBitStorage();
                if (storage == null) return;
            }

            if (storage.Values == null)
                storage.Values = new();

            id = id.ToLower();
            for (int i = 0; i < storage.Values.Count; i++)
            {
                if (storage.Values[i].id.ToLower() == id)
                {
                    storage.Values[i] = (id, value);
                    SaveBitStorage(storage);
                    return;
                }
            }

            storage.Values.Add((id, value));
            SaveBitStorage(storage);
        }

        /// <summary>
        /// Saves a bit storage.
        /// </summary>
        /// <param name="storage">The bit storage.</param>
        public static void SaveBitStorage(Ink_SaveBitStorage storage)
        {
            try
            {
                string serializedText = JsonSerializer.Serialize<Ink_SaveBitStorage>(storage, options: new JsonSerializerOptions() { IncludeFields = true });
                File.WriteAllText(bitPath, serializedText);
            }
            catch (Exception)
            {

            }
        }
    }
}
