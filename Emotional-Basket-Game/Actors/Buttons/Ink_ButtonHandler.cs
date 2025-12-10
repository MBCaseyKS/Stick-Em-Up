using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors.Buttons
{
    /// <summary>
    /// Handles all button events, making sure you can only press one button at a time.
    /// </summary>
    public class Ink_ButtonHandler
    {
        /// <summary>
        /// The game manager.
        /// </summary>
        public Ink_PinGameManager Game { get; set; }

        private List<Ink_Button_Base> _buttons;
        private MouseState prevMouseState, currentMouseState;

        private Ink_Button_Base currentHoveredButton;

        /// <summary>
        /// Initializes the handler with the actors array, to filter and optimize on creation.
        /// </summary>
        /// <param name="actors"></param>
        public Ink_ButtonHandler(Ink_PinGameManager Game, List<Ink_Actor_Base> actors)
        {
            this.Game = Game;
            _buttons = new List<Ink_Button_Base>();
            foreach (var actor in actors)
            {
                if (!(actor is Ink_Button_Base button)) continue;
                _buttons.Add(button);
            }
        }

        /// <summary>
        /// Updates the actor, as in a Tick.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            Vector2 screenCenter = Game.GetScreenCenter();
            float screenScale = Game.GetScreenScale();

            float x = (currentMouseState.X - screenCenter.X) / screenScale;
            float y = (currentMouseState.Y - screenCenter.Y) / screenScale;
            Vector2 mousePos = new Vector2(x, y);

            Ink_Button_Base newHoveredButton = null;
            foreach (var button in _buttons)
            {
                if (button.IsInRange(mousePos))
                {
                    newHoveredButton = button;
                    break;
                }
            }

            if (newHoveredButton != currentHoveredButton)
            {
                if (newHoveredButton == null)
                {
                    if (currentHoveredButton != null)
                        currentHoveredButton.OnUnhovered(mousePos);
                }
                else
                    newHoveredButton.OnHovered(mousePos);

                currentHoveredButton = newHoveredButton;
            }

            if (prevMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (currentHoveredButton != null)
                    currentHoveredButton.OnClicked(mousePos);
            }
        }

        /// <summary>
        /// Un-hovers any buttons that were hovered.
        /// </summary>
        public void CleanUp()
        {
            if (currentHoveredButton != null)
            {
                currentHoveredButton.OnUnhovered(currentHoveredButton.Position);
                currentHoveredButton = null;
            }
        }
    }
}
