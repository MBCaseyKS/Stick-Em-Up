using Microsoft.Xna.Framework;

namespace EmotionalBasketGame
{
    /// <summary>
    /// Stores game settings to be saved between loads.
    /// </summary>
    public class Ink_GameSettings
    {
        /// <summary>
        /// The game's X resolution.
        /// </summary>
        public int ScreenWidth { get; set; } = 1280;

        /// <summary>
        /// The game's Y resolution.
        /// </summary>
        public int ScreenHeight { get; set; } = 720;

        /// <summary>
        /// Whether or not the game is fullscreen.
        /// </summary>
        public bool IsFullscreen { get; set; } = false;

        /// <summary>
        /// The game's framerate.
        /// </summary>
        public int FPS { get; set; } = 60;
    }
}
