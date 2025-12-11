using EmotionalBasketGame.Actors.HUDs;
using EmotionalBasketGame.Screens;
using EmotionalBasketGame.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmotionalBasketGame
{
    public class Ink_PinGameManager : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Ink_LoadHandler _loadHandler;

        private List<Ink_GameScreen_Base> _screens;
        private List<Ink_HUD_Base> _hudElements;

        public Ink_MusicManager MusicManager { get; private set; }

        public Ink_PinGameManager()
        {
            Window.Title = "Stick 'Em Up";

            // Set to 720p
            _graphics = new GraphicsDeviceManager(this);
            //_graphics.PreferredBackBufferWidth = 1280;
            //_graphics.PreferredBackBufferHeight = 720;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            _screens = new();
            _hudElements = new();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Returns the scale of the screen.
        /// </summary>
        /// <returns>The scale of the screen.</returns>
        public float GetScreenScale() => Math.Min(_graphics.PreferredBackBufferWidth / 1280f, _graphics.PreferredBackBufferHeight / 720f);

        /// <summary>
        /// Returns the center of the screen.
        /// </summary>
        /// <returns>The center vector of the screen.</returns>
        public Vector2 GetScreenCenter() => new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);

        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Sets up a new screen.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        /// <param name="doLoad">Whether or not to load content.</param>
        public void AddScreen(Ink_GameScreen_Base screen, bool doLoad)
        {
            _screens.Add(screen);
            if (doLoad)
                screen.LoadContent(_graphics.GraphicsDevice, Content);
        }

        /// <summary>
        /// Removes a screen.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        public void RemoveScreen(Ink_GameScreen_Base screen)
        {
            screen.OnRemoved();
            _screens.Remove(screen);
        }

        /// <summary>
        /// Sets up a new screen.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        public void AddScreen(Ink_GameScreen_Base screen) => AddScreen(screen, true);

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
            if (_loadHandler != null)
                _loadHandler.RunLoad(oldScreen, newScreen, inTransition, outTransition, freezeTime);
        }

        /// <summary>
        /// Opens a HUD element for the game.
        /// </summary>
        /// <param name="InHUD">The HUD to add.</param>
        /// <returns>The HUD once modified.</returns>
        public Ink_HUD_Base OpenHUD(Ink_HUD_Base InHUD)
        {
            if (_hudElements.IndexOf(InHUD) <= -1)
                _hudElements.Add(InHUD);

            InHUD.LoadContent(GraphicsDevice, Content);
            return InHUD;
        }

        /// <summary>
        /// Removes a HUD from the game.
        /// </summary>
        /// <param name="InHUD">The HUD to remove.</param>
        public void CloseHUD(Ink_HUD_Base InHUD)
        {
            _hudElements.Remove(InHUD);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            MusicManager = new(Content);
            _loadHandler = new(this);

            AddScreen(new Ink_Screen_Titlescreen(this), false);

            //Loads content for all existing screens.
            foreach (Ink_GameScreen_Base screen in _screens)
                screen.LoadContent(_graphics.GraphicsDevice, Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MusicManager?.Update(gameTime);
            _loadHandler?.Update(gameTime);

            //Updates each screen
            foreach (Ink_GameScreen_Base screen in _screens)
                screen.Update(gameTime);

            var huds = _hudElements.ToArray();
            foreach (var hud in huds)
                hud.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //Draws each screen.
            foreach (Ink_GameScreen_Base screen in _screens)
                screen.Draw(_spriteBatch, gameTime);

            _loadHandler?.Draw(_spriteBatch, gameTime);

            if (_hudElements.Count > 0)
            {
                Matrix transform = Matrix.CreateScale(GetScreenScale());
                _spriteBatch.Begin(transformMatrix: transform);

                var hudGroups = _hudElements.ToArray().GroupBy(h => h.RenderPriority).OrderBy(g => g.Key).ToArray();
                foreach (var group in hudGroups)
                {
                    foreach (var hud in group)
                        hud.Draw(_spriteBatch, gameTime);
                }

                _spriteBatch.End();
            }    

            base.Draw(gameTime);
        }
    }
}
