using EmotionalBasketGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace EmotionalBasketGame
{
    public class EmotionalBasketGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private List<Ink_GameScreen_Base> _screens;

        public EmotionalBasketGame()
        {
            // Set to 720p
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            _screens = new();
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
        /// Sets up a new screen.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        public void AddScreen(Ink_GameScreen_Base screen) => AddScreen(screen, true);

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            AddScreen(new Ink_Screen_Minigame(this), false);

            //Loads content for all existing screens.
            foreach (Ink_GameScreen_Base screen in _screens)
                screen.LoadContent(_graphics.GraphicsDevice, Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Updates each screen
            foreach (Ink_GameScreen_Base screen in _screens)
                screen.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //Draws each screen.
            foreach (Ink_GameScreen_Base screen in _screens)
                screen.Draw(_spriteBatch, gameTime);

            base.Draw(gameTime);
        }
    }
}
