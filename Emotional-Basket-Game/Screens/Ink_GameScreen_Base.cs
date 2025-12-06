using EmotionalBasketGame.Actors;
using Microsoft.Xna.Framework;
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

        /// <summary>
        /// The actors on this screen.
        /// </summary>
        public List<Ink_Actor_Base> Actors { get => _actors.ToList(); }

        /// <summary>
        /// The game manager.
        /// </summary>
        public EmotionalBasketGame Game { get; set; }

        /// <summary>
        /// The content manager.
        /// </summary>
        public ContentManager Content { get; set; }

        /// <summary>
        /// The background color.
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.White;

        // Handles drawing the background.
        Texture2D backgroundTexture;

        public Ink_GameScreen_Base(EmotionalBasketGame game)
        {
            Game = game;
            _actors = new List<Ink_Actor_Base>();
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
        /// Spawns an actor and adds it to the list.
        /// </summary>
        /// <param name="actor">The actor to initialize.</param>
        public Ink_Actor_Base AddActor(Ink_Actor_Base actor, Vector2 actorPosition, bool doLoad, int depth)
        {
            actor.Game = Game;
            actor.World = this;
            actor.Position = actorPosition;
            actor.Depth = depth;
            _actors.Add(actor);

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
        /// Destroys an actor.
        /// </summary>
        /// <param name="actor">The actor to remove.</param>
        public void DestroyActor(Ink_Actor_Base actor)
        {
            _actors.Remove(actor);
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
        }

        /// <summary>
        /// Handles drawing the screen.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();

            //Draw the BG
            spriteBatch.Draw(backgroundTexture, GetScreenRectangle(), BackgroundColor);

            IEnumerable<Ink_Actor_Base> currentActors = _actors.ToArray().OrderByDescending(actor => actor.Depth);
            foreach (Ink_Actor_Base actor in currentActors)
                actor.Draw(gameTime, spriteBatch);

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
    }
}
