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
        private SoundEffectInstance onHoveredSound;
        private SoundEffectInstance onClickedSound;
        private Texture2D buttonTexture;

        private Vector2 areaSize;
        private string buttonPathName, hoverSoundPathName, clickSoundPathName;

        private bool isHovered;

        public Ink_Button_Square(Vector2 areaSize, string buttonPathName = "", string hoverSoundPathName = "", string clickSoundPathName = "", OnButtonClicked buttonClickDel = null)
        {
            this.areaSize = areaSize;
            this.buttonPathName = buttonPathName;
            this.hoverSoundPathName = hoverSoundPathName;
            this.clickSoundPathName = clickSoundPathName;
            this.buttonClickDel = buttonClickDel;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            if (buttonPathName != "")
                buttonTexture = content.Load<Texture2D>(buttonPathName);
            if (hoverSoundPathName != "")
                InitSoundInstance(content, hoverSoundPathName, out onHoveredSound);
            if (clickSoundPathName != "")
                InitSoundInstance(content, clickSoundPathName, out onClickedSound);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (buttonTexture != null)
            {
                Vector2 position = GetScreenPosition();
                float screenScale = GetScreenScale();
                spriteBatch.Draw(buttonTexture, position, new Rectangle(isHovered ? 512 : 0, 0, 512, 512), Color.White, 0, new Vector2(256, 256), screenScale, SpriteEffects.None, 0f);
            }
        }

        public override bool IsInRange(Vector2 mousePosition)
        {
            return Math.Abs(mousePosition.X - Position.X) <= areaSize.X * 0.5 && Math.Abs(mousePosition.Y - Position.Y) <= areaSize.Y * 0.5;
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
