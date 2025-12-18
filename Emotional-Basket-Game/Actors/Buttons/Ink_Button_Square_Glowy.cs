using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors.Buttons
{
    public class Ink_Button_Square_Glowy : Ink_Button_Square
    {
        public Ink_Button_Square_Glowy(Vector2 areaSize, string buttonPathName = "", string hoverSoundPathName = "", string clickSoundPathName = "", OnButtonClicked buttonClickDel = null, float scale = 1, string buttonMsg = "", Color? textColor = null, float textScale = 1) : base(areaSize, buttonPathName, hoverSoundPathName, clickSoundPathName, buttonClickDel, scale, buttonMsg, textColor, textScale)
        {
            this.areaSize = areaSize;
            this.buttonPathName = buttonPathName;
            this.hoverSoundPathName = hoverSoundPathName;
            this.clickSoundPathName = clickSoundPathName;
            this.buttonClickDel = buttonClickDel;
            this.Scale = scale;
            this.ButtonMsg = buttonMsg;
            this.textColor = (Color)((textColor != null) ? textColor : Color.Black);
            this.textScale = textScale;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 position = GetScreenPosition();
            float screenScale = GetScreenScale();
            if (buttonTexture != null)
            {
                float alpha = 0.5f + 0.5f * (float)Math.Sin(Math.PI * gameTime.TotalGameTime.TotalSeconds);
                byte R = (byte)MathHelper.Lerp(255, 200, alpha);
                byte G = (byte)MathHelper.Lerp(255, 55, alpha);
                byte B = (byte)MathHelper.Lerp(255, 55, alpha);
                Color buttonColor = new Color(R, G, B);

                spriteBatch.Draw(buttonTexture, position, new Rectangle(isHovered ? ((int)(textureSize.X * 0.5f)) : 0, 0, (int)(textureSize.X * 0.5), (int)textureSize.Y), buttonColor, 0, new Vector2(textureSize.X * 0.25f, textureSize.Y * 0.5f), screenScale * Scale, SpriteEffects.None, 0f);
            }

            if (ButtonMsg != "")
            {
                Vector2 textSize = buttonFont.MeasureString(ButtonMsg);
                spriteBatch.DrawString(buttonFont, ButtonMsg, position, textColor, 0f, textSize * 0.5f, textScale, SpriteEffects.None, 0f);
            }
        }
    }
}
