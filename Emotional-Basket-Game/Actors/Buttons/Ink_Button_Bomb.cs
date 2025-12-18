using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors.Buttons
{
    /// <summary>
    /// IT'S A BO
    /// </summary>
    public class Ink_Button_Bomb : Ink_Button_Circular
    {
        public Ink_Button_Bomb(float radius, string buttonPathName = "", string hoverSoundPathName = "", string clickSoundPathName = "", OnButtonClicked buttonClickDel = null, float scale = 1.0f) : base(radius, buttonPathName, hoverSoundPathName, clickSoundPathName, buttonClickDel, scale)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (buttonTexture != null)
            {
                Vector2 position = GetScreenPosition();
                float screenScale = GetScreenScale() * Scale;
                spriteBatch.Draw(buttonTexture, position, new Rectangle(isHovered ? 512 : 0, 0, 512, 512), Color.White, 0, new Vector2(256, 256), screenScale * (isHovered ? (1.0f + 0.1f*(float)Math.Sin(Math.PI * gameTime.TotalGameTime.TotalSeconds)) : 1.0f), SpriteEffects.None, 0f);
            }
        }
    }
}
