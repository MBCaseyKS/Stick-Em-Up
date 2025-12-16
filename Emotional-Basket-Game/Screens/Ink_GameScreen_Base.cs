using EmotionalBasketGame.Actors;
using EmotionalBasketGame.Actors.HUDs;
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
using System.Windows.Forms;

namespace EmotionalBasketGame.Screens
{
    /// <summary>
    /// A screen with its own subset of actors.
    /// </summary>
    public class Ink_GameScreen_Base
    {
        protected List<Ink_Actor_Base> _actors;
        protected List<Ink_ScreenShake> _screenShakes;
        private IEnumerable<IGrouping<int, Ink_Actor_Base>> _sortedActors;

        /// <summary>
        /// The actors on this screen.
        /// </summary>
        public List<Ink_Actor_Base> Actors { get => _actors.ToList(); }

        /// <summary>
        /// The game manager.
        /// </summary>
        public Ink_PinGameManager Game { get; set; }

        /// <summary>
        /// The content manager.
        /// </summary>
        public ContentManager Content { get; set; }

        /// <summary>
        /// Gets the game's music manager.
        /// </summary>
        public Ink_MusicManager MusicManager { get => Game.MusicManager; }

        /// <summary>
        /// Gets the game's music manager.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get => Game.GraphicsDevice; }

        /// <summary>
        /// The background color.
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.White;

        /// <summary>
        /// The offset a screen might have.
        /// </summary>
        public Vector2 ScreenOffset { get; set; }

        // Handles drawing the background.
        protected Texture2D backgroundTexture;

        public Ink_GameScreen_Base(Ink_PinGameManager game)
        {
            Game = game;
            _actors = new List<Ink_Actor_Base>();
            _screenShakes = new List<Ink_ScreenShake>();
        }

        /// <summary>
        /// Loads game content for all current actors.
        /// </summary>
        /// <param name="graphics">The graphics device.</param>
        /// <param name="content">The content manager.</param>
        public virtual void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            Content = content;

            backgroundTexture = new Texture2D(graphics, 1, 1);
            backgroundTexture.SetData<Color>(new Color[] { Color.White });

            foreach (Ink_Actor_Base actor in _actors)
                actor.LoadContent(Content);
        }

        /// <summary>
        /// Is called when the screen is removed from the game.
        /// </summary>
        public virtual void OnRemoved()
        {
            
        }

        /// <summary>
        /// Spawns an actor and adds it to the list.
        /// </summary>
        /// <param name="actor">The actor to initialize.</param>
        public virtual Ink_Actor_Base AddActor(Ink_Actor_Base actor, Vector2 actorPosition, bool doLoad, double depth = 0, int renderPriority = 0)
        {
            actor.Game = Game;
            actor.World = this;
            actor.Position = actorPosition;
            actor.Depth = depth;
            actor.RenderPriority = renderPriority;
            _actors.Add(actor);
            UpdateActorSorting();

            if (doLoad)
                actor.LoadContent(Content);

            return actor;
        }

        /// <summary>
        /// Spawns an actor and adds it to the list.
        /// </summary>
        /// <param name="actor"></param>
        public Ink_Actor_Base AddActor(Ink_Actor_Base actor, Vector2 actorPosition, bool doLoad) => AddActor(actor, actorPosition, doLoad, 0);

        /// <summary>
        /// Spawns an actor and adds it to the list.
        /// </summary>
        /// <param name="actor"></param>
        public Ink_Actor_Base AddActor(Ink_Actor_Base actor, Vector2 actorPosition) => AddActor(actor, actorPosition, true, 0);

        /// <summary>
        /// Spawns an actor and adds it to the list.
        /// </summary>
        public Ink_Actor_Base AddActor(Ink_Actor_Base actor) => AddActor(actor, Vector2.Zero, true, 0);

        /// <summary>
        /// Destroys an actor.
        /// </summary>
        /// <param name="actor">The actor to remove.</param>
        public void DestroyActor(Ink_Actor_Base actor)
        {
            _actors.Remove(actor);
            UpdateActorSorting();
        }

        /// <summary>
        /// Updates and sorts the sorted actors array.
        /// </summary>
        public void UpdateActorSorting()
        {
            _sortedActors = Actors
                .GroupBy(w => w.RenderPriority)
                .OrderBy(g => g.Key);
        }

        /// <summary>
        /// Opens a HUD element for the game.
        /// </summary>
        /// <param name="InHUD"></param>
        /// <returns></returns>
        public Ink_HUD_Base OpenHUD(Ink_HUD_Base InHUD) => Game?.OpenHUD(InHUD);


        /// <summary>
        /// Removes a HUD from the game.
        /// </summary>
        /// <param name="InHUD">The HUD to remove.</param>
        public void CloseHUD(Ink_HUD_Base InHUD) => Game?.CloseHUD(InHUD);

        /// <summary>
        /// Adds a new screen shake.
        /// </summary>
        /// <param name="intensity">The maximum offset of this shake.</param>
        /// <param name="duration">How long this shake lasts.</param>
        /// <param name="fadeIn">How long it takes for a shake to get to its maximum shake.</param>
        /// <param name="fadeOut">How long it takes for a shake to fade out.</param>
        /// <returns>The initialized screen shake.</returns>
        public Ink_ScreenShake DoScreenShake(Vector2 intensity, float duration = 0.5f, float fadeIn = 0.1f, float fadeOut = 0.1f)
        {
            Ink_ScreenShake shake = new(intensity, duration, fadeIn, fadeOut);
            if (shake != null)
                _screenShakes.Add(shake);

            return shake;
        }

        /// <summary>
        /// Handles a game Tick.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public virtual void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            List<Ink_Actor_Base> currentActors = new(_actors);

            foreach (Ink_Actor_Base actor in currentActors)
                actor.Update(gameTime);

            if (_screenShakes.Count > 0)
                _screenShakes = Ink_ScreenShake.UpdateAllShakes(_screenShakes, (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        /// Handles drawing the screen.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Matrix transform = Matrix.Identity;
            if (_screenShakes.Count > 0)
            {
                Vector2 shakeOffset = Ink_ScreenShake.GetShakeOffset(_screenShakes) * GetScreenScale();
                transform = Matrix.CreateTranslation(shakeOffset.X, shakeOffset.Y, 0);
            }

            spriteBatch.Begin(transformMatrix: transform);

            //Draw the BG
            spriteBatch.Draw(backgroundTexture, GetScreenRectangle(), BackgroundColor);

            var sortedGroups = _sortedActors.ToArray();
            foreach (var group in sortedGroups)
            {
                var depthSortedActors = group.ToArray().OrderByDescending(actor => actor.Depth);
                foreach (Ink_Actor_Base actor in depthSortedActors)
                {
                    if (actor.IsHidden) continue;

                    bool hasCustomBlend = actor.GetCustomBlend(out BlendState customState);
                    if (hasCustomBlend)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(transformMatrix: transform, blendState: customState);
                    }

                    actor.Draw(gameTime, spriteBatch);

                    if (hasCustomBlend)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(transformMatrix: transform);
                    }
                }
            }

            spriteBatch.End();
        }

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
        /// Gets a rectangle the size of the screen.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetScreenRectangle()
        {
            float screenScale = GetScreenScale();
            Rectangle rect = new(0, 0, (int)(1280 * screenScale), (int)(720 * screenScale));
            return rect;
        }

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

            inst.Volume = volume;
            inst.Pitch = pitch;
            inst.IsLooped = bLooping;
            inst.Play();
        }
    }
}
