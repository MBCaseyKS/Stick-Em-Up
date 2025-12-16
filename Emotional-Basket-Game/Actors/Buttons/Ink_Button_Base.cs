using Microsoft.Xna.Framework;
using static EmotionalBasketGame.Actors.Ink_TargetRound_Layout;

namespace EmotionalBasketGame.Actors.Buttons
{
    /// <summary>
    /// A button that handles click events.
    /// </summary>
    public abstract class Ink_Button_Base : Ink_Actor_Base
    {
        /// <summary>
        /// Handles when the button is clicked.
        /// </summary>
        /// <param name="button">The button clicked.</param>
        /// <returns>True if it's been handled.</returns>
        public delegate bool OnButtonClicked(Ink_Button_Base button);

        /// <summary>
        /// Set to handle a button click event.
        /// </summary>
        public OnButtonClicked buttonClickDel { get; set; }

        /// <summary>
        /// The layer this button is on.
        /// </summary>
        public int ButtonLayer { get; set; } = 0;

        /// <summary>
        /// Returns true if in range of the mouse.
        /// </summary>
        /// <param name="mousePosition">The mouse's position, scaled to the window size.</param>
        /// <returns>True if in range.</returns>
        public abstract bool IsInRange(Vector2 mousePosition);

        /// <summary>
        /// Handles when first hovered by the mouse.
        /// </summary>
        public abstract void OnHovered(Vector2 mousePosition);

        /// <summary>
        /// Handles when first hovered by the mouse.
        /// </summary>
        public abstract void OnUnhovered(Vector2 mousePosition);

        /// <summary>
        /// Handles when clicked by the mouse.
        /// </summary>
        public virtual bool OnClicked(Vector2 mousePosition)
        {
            if (buttonClickDel != null && buttonClickDel(this)) return true;
            return false;
        }
    }
}
