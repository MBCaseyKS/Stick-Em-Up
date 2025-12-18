using EmotionalBasketGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors.HUDs
{
    /// <summary>
    /// A HUD rendered over the entire game, and can maintain between screens. Automatically scaled to fit game screens with SpriteBatch.
    /// </summary>
    public abstract class Ink_HUD_Base
    {
        /// <summary>
        /// The game manager.
        /// </summary>
        public Ink_PinGameManager Game { get; set; }

        /// <summary>
        /// The active game screen, to use for things like positional offset aiming.
        /// </summary>
        public Ink_GameScreen_Base ActiveScreen { get; set; }

        /// <summary>
        /// Gets the game's music manager.
        /// </summary>
        public Ink_MusicManager MusicManager { get => Game.MusicManager; }

        /// <summary>
        /// The order of which HUDs may be rendered. Defaults to zero.
        /// </summary>
        public int RenderPriority { get; set; } = 0;

        /// <summary>
        /// Inits a new HUD.
        /// </summary>
        /// <param name="game">The game manager.</param>
        /// <param name="activeScreen">The active game screen, to use for things like positional offset aiming.</param>
        public Ink_HUD_Base(Ink_PinGameManager game, Ink_GameScreen_Base activeScreen, int renderPriority = 0)
        {
            Game = game;
            ActiveScreen = activeScreen;
            RenderPriority = renderPriority;
        }

        /// <summary>
        /// An event called when the HUD is removed from the game manager.
        /// </summary>
        public virtual void OnClosed() { }

        /// <summary>
        /// Closes the HUD itself.
        /// </summary>
        public virtual void DoClose()
        {
            Game.CloseHUD(this);
        }

        /// <summary>
        /// Loads game content.
        /// </summary>
        /// <param name="graphics">The graphics device.</param>
        /// <param name="content">The content manager.</param>
        public abstract void LoadContent(GraphicsDevice graphics, ContentManager content);

        /// <summary>
        /// Handles a game Tick.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Handles drawing the screen.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        /// <summary>
        /// Returns the scale of the screen.
        /// </summary>
        /// <returns>The scale of the screen.</returns>
        public float GetScreenScale() => Game.GetScreenScale();

        /// <summary>
        /// Returns the center of the screen.
        /// </summary>
        /// <returns>The center vector of the screen.</returns>
        public Vector2 GetScreenCenter() => Game.GetScreenCenter();

        /// <summary>
        /// Inits a soundeffectinstance, so we do not need to store the SoundEffect itself.
        /// </summary>
        /// <param name="content">The content manager.</param>
        /// <param name="fileName">The file name for the sound sample.</param>
        /// <param name="inst">The instance variable that will be filled.</param>
        public void InitSoundInstance(ContentManager content, string fileName, out SoundEffectInstance inst)
        {
            inst = null;
            SoundEffect sound = content.Load<SoundEffect>(fileName);
            if (sound != null)
                inst = sound.CreateInstance();
        }

        /// <summary>
        /// A condensed form of playing a soundeffect instance.
        /// </summary>
        /// <param name="inst">The instance.</param>
        /// <param name="volume">The volume.</param
        /// <param name="pitch">The pitch.</param>
        /// <param name="bLooping">Whether or not to loop the instance.</param>
        public void PlaySoundInst(SoundEffectInstance inst, float volume = 1.0f, float pitch = 1.0f, bool bLooping = false)
        {
            if (inst == null) return;
            if (inst.State == SoundState.Playing) return;

            volume = MathHelper.Clamp(volume, 0.0f, 1.0f);
            inst.Volume = volume;
            inst.Pitch = pitch;
            inst.IsLooped = bLooping;

            inst.Play();
        }

        /// <summary>
        /// A condensed form of playing a soundeffect instance.
        /// </summary>
        /// <param name="inst">The instance.</param>
        /// <param name="volume">The volume.</param
        /// <param name="pitch">The pitch.</param>
        public void PlaySoundInst(SoundEffectInstance inst, float volume = 1.0f, float pitch = 1.0f) => PlaySoundInst(inst, volume, pitch, false);

        /// <summary>
        /// Returns if the SpriteBatch should be remade with a new blend state.
        /// </summary>
        /// <param name="blendState">The custom blend state to use.</param>
        /// <returns>Returns if the SpriteBatch should be remade with a new blend state.</returns>
        public virtual bool GetCustomBlend(out BlendState blendState)
        {
            blendState = null;
            return false;
        }
    }
}
