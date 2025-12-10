using EmotionalBasketGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Transitions
{
    public enum LoadState
    {
        Inactive,
        LoadingIn,
        Freeze,
        LoadingOut
    }

    /// <summary>
    /// Handles loading between screens.
    /// </summary>
    public class Ink_LoadHandler
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
        /// Gets the game's music manager.
        /// </summary>
        public Ink_MusicManager MusicManager { get => Game.MusicManager; }

        /// <summary>
        /// Gets the game's music manager.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get => Game.GraphicsDevice; }

        /// <summary>
        /// The screen to transition out of.
        /// </summary>
        public Ink_GameScreen_Base OldScreen { get; set; }

        /// <summary>
        /// The screen to transition into.
        /// </summary>
        public Ink_GameScreen_Base NewScreen { get; set; }

        /// <summary>
        /// Handles going IN to the transition.
        /// </summary>
        public Ink_ScreenTransition_Base InTransition { get; set; }

        /// <summary>
        /// Handles going OUT of the transition.
        /// </summary>
        public Ink_ScreenTransition_Base OutTransition { get; set; }

        /// <summary>
        /// The amount of time to wait after a load has completed before de-loading.
        /// </summary>
        public double FreezeTime { get; set; }

        private LoadState currentLoadState = LoadState.Inactive;

        /// <summary>
        /// Creates a new load handler.
        /// </summary>
        /// <param name="game">The game manager.</param>
        public Ink_LoadHandler(Ink_PinGameManager game)
        {
            Game = game;
        }

        /// <summary>
        /// Handles a game Tick.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            if (currentLoadState == LoadState.Inactive) return; //Slightly better performance when not loading?

            if (currentLoadState == LoadState.LoadingIn)
                InTransition?.Update(gameTime);
            else if (currentLoadState == LoadState.Freeze)
            {
                FreezeTime = Math.Max(FreezeTime - gameTime.ElapsedGameTime.TotalSeconds, 0);
                if (FreezeTime <= 0)
                {
                    currentLoadState = LoadState.LoadingOut;
                    InTransition = null;

                    if (NewScreen != null)
                        Game.AddScreen(NewScreen, true);

                    OutTransition?.OnBegin(true, OnLoadComplete);
                }
            }
            else if (currentLoadState == LoadState.LoadingOut)
                OutTransition?.Update(gameTime);
        }

        /// <summary>
        /// Handles drawing the screen.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (currentLoadState == LoadState.Inactive) return;

            spriteBatch.Begin();

            if (currentLoadState == LoadState.LoadingIn || currentLoadState == LoadState.Freeze)
                InTransition?.Draw(spriteBatch, gameTime);
            else if (currentLoadState == LoadState.LoadingOut)
                OutTransition?.Draw(spriteBatch, gameTime);

            spriteBatch.End();
        }

        /// <summary>
        /// Loads game content for all current actors.
        /// </summary>
        /// <param name="graphics">The graphics device.</param>
        /// <param name="content">The content manager.</param>
        public virtual void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            Content = content;
            InTransition?.LoadContent(graphics, content);
            OutTransition?.LoadContent(graphics, content);
        }

        /// <summary>
        /// Starts a new load.
        /// </summary>
        /// <param name="oldScreen">The screen to transition out of.</param>
        /// <param name="newScreen">The screen to transition into.</param>
        /// <param name="inTransition">Handles going IN to the transition.</param>
        /// <param name="outTransition">Handles going OUT of the transition.</param>
        /// <param name="freezeTime">The amount of time to wait after a load has completed before de-loading.</param>
        public void RunLoad(Ink_GameScreen_Base oldScreen, Ink_GameScreen_Base newScreen, Ink_ScreenTransition_Base inTransition, Ink_ScreenTransition_Base outTransition, double freezeTime = 0.5)
        {
            OldScreen = oldScreen;
            NewScreen = newScreen;
            InTransition = inTransition;
            OutTransition = outTransition;
            FreezeTime = freezeTime;

            currentLoadState = LoadState.LoadingIn;
            LoadContent(GraphicsDevice, Content);
            InTransition?.OnBegin(false, OnLoadComplete);
        }

        /// <summary>
        /// Is called when a transition completes.
        /// </summary>
        /// <param name="transition">The completed transition.</param>
        public void OnLoadComplete(Ink_ScreenTransition_Base transition)
        {
            if (currentLoadState == LoadState.LoadingIn && transition == InTransition)
            {
                currentLoadState = LoadState.Freeze;
                if (OldScreen != null)
                {
                    Game.RemoveScreen(OldScreen);
                    OldScreen = null;
                }
            }
            else if (currentLoadState == LoadState.LoadingOut && transition == OutTransition)
            {
                currentLoadState = LoadState.Inactive;
                OutTransition = null;
                NewScreen = null;
            }
        }
    }
}
