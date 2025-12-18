using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using static EmotionalBasketGame.Actors.Ink_TargetRound_Layout;

namespace EmotionalBasketGame.Actors.Buttons
{
    /// <summary>
    /// A square button that handles click events.
    /// </summary>
    public class Ink_Button_Square : Ink_Button_Base
    {
        /// <summary>
        /// The button scale.
        /// </summary>
        public float Scale { get; set; } = 1.0f;

        /// <summary>
        /// A message to display over the button.
        /// </summary>
        public string ButtonMsg { get; protected set; } = "";

        protected SoundEffectInstance onHoveredSound;
        protected SoundEffectInstance onClickedSound;
        protected Texture2D buttonTexture;
        protected SpriteFont buttonFont;

        protected Color textColor;
        protected Vector2 areaSize;
        protected Vector2 textureSize;
        protected string buttonPathName, hoverSoundPathName, clickSoundPathName;
        protected float textScale;

        protected bool isHovered;

        public Ink_Button_Square(Vector2 areaSize, string buttonPathName = "", string hoverSoundPathName = "", string clickSoundPathName = "", OnButtonClicked buttonClickDel = null, float scale = 1f, string buttonMsg = "", Color? textColor = null, float textScale = 1f)
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

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            buttonFont = content.Load<SpriteFont>("Umeko");
            if (buttonPathName != "")
            {
                buttonTexture = content.Load<Texture2D>(buttonPathName);
                textureSize = new Vector2(buttonTexture.Width, buttonTexture.Height);
            }
            if (hoverSoundPathName != "")
                InitSoundInstance(content, hoverSoundPathName, out onHoveredSound);
            if (clickSoundPathName != "")
                InitSoundInstance(content, clickSoundPathName, out onClickedSound);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            Vector2 position = GetScreenPosition();
            float screenScale = GetScreenScale();
            if (buttonTexture != null)
                spriteBatch.Draw(buttonTexture, position, new Rectangle(isHovered ? ((int)(textureSize.X*0.5f)) : 0, 0, (int)(textureSize.X*0.5), (int)textureSize.Y), Color.White, 0, new Vector2(textureSize.X*0.25f, textureSize.Y*0.5f), screenScale * Scale, SpriteEffects.None, 0f);

            if (ButtonMsg != "")
            {
                Vector2 textSize = buttonFont.MeasureString(ButtonMsg);
                spriteBatch.DrawString(buttonFont, ButtonMsg, position, textColor, 0f, textSize * 0.5f, textScale, SpriteEffects.None, 0f);
            }
        }

        public override bool IsInRange(Vector2 mousePosition)
        {
            return Math.Abs(mousePosition.X - Position.X) <= areaSize.X * 0.5 * Scale && Math.Abs(mousePosition.Y - Position.Y) <= areaSize.Y * 0.5 * Scale;
        }

        public override bool OnClicked(Vector2 mousePosition)
        {
            PlaySoundInst(onClickedSound, Ink_RandomHelper.RandRange(0.5f, 0.6f), Ink_RandomHelper.RandRange(-0.1f, 0.1f));

            if (buttonClickDel != null && buttonClickDel(this)) return true;
            return true;
        }

        public override void OnHovered(Vector2 mousePosition) 
        { 
            isHovered = true;
            PlaySoundInst(onHoveredSound, Ink_RandomHelper.RandRange(0.5f, 0.6f), Ink_RandomHelper.RandRange(-0.1f, 0.1f));
        }

        public override void OnUnhovered(Vector2 mousePosition) { isHovered = false; }
    }
}
