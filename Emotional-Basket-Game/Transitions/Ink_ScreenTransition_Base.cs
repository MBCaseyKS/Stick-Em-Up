using EmotionalBasketGame.Actors;
using EmotionalBasketGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Transitions
{
    /// <summary>
    /// Transitions between screens.
    /// </summary>
    public abstract class Ink_ScreenTransition_Base
    {
        /// <summary>
        /// The game manager.
        /// </summary>
        public Ink_PinGameManager Game { get; set; }

        /// <summary>
        /// The content manager.
        /// </summary>
        public ContentManager Content { get; set; }

        /// <summary>
        /// The content manager.
        /// </summary>
        public double TransitionTime { get; set; }

        /// <summary>
        /// Gets the game's music manager.
        /// </summary>
        public Ink_MusicManager MusicManager { get => Game.MusicManager; }

        protected double transitionProgress;
        protected bool IsReversing;

        /// <summary>
        /// Is called when a transition completes.
        /// </summary>
        /// <param name="transition">The completed transition.</param>
        public delegate void OnLoadComplete(Ink_ScreenTransition_Base transition);

        public OnLoadComplete loadCompleteDel { get; set; }

        public Ink_ScreenTransition_Base(Ink_PinGameManager game, double transitionTime)
        {
            Game = game;
            TransitionTime = transitionTime;
        }

        /// <summary>
        /// Handles when this transition is put into use.
        /// </summary>
        /// <param name="isLoadOut"></param>
        public virtual void OnBegin(bool isLoadOut, OnLoadComplete loadCompleteDel = null)
        {
            IsReversing = isLoadOut;
            transitionProgress = isLoadOut ? TransitionTime : 0;
            this.loadCompleteDel = loadCompleteDel;
        }

        /// <summary>
        /// Handles when this animation has completed.
        /// </summary>
        public virtual void OnComplete()
        {
            if (loadCompleteDel != null)
                loadCompleteDel(this);
        }

        /// <summary>
        /// Loads game content for all current actors.
        /// </summary>
        /// <param name="graphics">The graphics device.</param>
        /// <param name="content">The content manager.</param>
        public virtual void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            Content = content;
        }

        /// <summary>
        /// Handles a game Tick.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (IsReversing)
            {
                transitionProgress = Math.Max(transitionProgress - gameTime.ElapsedGameTime.TotalSeconds, 0);
                if (transitionProgress <= 0)
                    OnComplete();
            }
            else
            {
                transitionProgress = Math.Min(transitionProgress + gameTime.ElapsedGameTime.TotalSeconds, TransitionTime);
                if (transitionProgress >= TransitionTime)
                    OnComplete();
            }
        }

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
