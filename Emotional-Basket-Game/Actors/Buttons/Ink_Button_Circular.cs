using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EmotionalBasketGame.Actors.Buttons.Ink_Button_Base;

namespace EmotionalBasketGame.Actors.Buttons
{
    public class Ink_Button_Circular : Ink_Button_Base
    {
        /// <summary>
        /// The button scale.
        /// </summary>
        public float Scale { get; set; } = 1.0f;

        protected SoundEffectInstance onHoveredSound;
        protected SoundEffectInstance onClickedSound;
        protected Texture2D buttonTexture;

        protected float radius;
        protected string buttonPathName, hoverSoundPathName, clickSoundPathName;

        protected bool isHovered;

        public Ink_Button_Circular(float radius, string buttonPathName = "", string hoverSoundPathName = "", string clickSoundPathName = "", OnButtonClicked buttonClickDel = null, float scale = 1.0f)
        {
            this.radius = radius;
            this.buttonPathName = buttonPathName;
            this.hoverSoundPathName = hoverSoundPathName;
            this.clickSoundPathName = clickSoundPathName;
            this.buttonClickDel = buttonClickDel;
            this.Scale = scale;
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
                float screenScale = GetScreenScale() * Scale;
                spriteBatch.Draw(buttonTexture, position, new Rectangle(isHovered ? 512 : 0, 0, 512, 512), Color.White, 0, new Vector2(256, 256), screenScale, SpriteEffects.None, 0f);
            }
        }

        public override bool IsInRange(Vector2 mousePosition)
        {
            return radius <= 0 || Ink_Math.VSize(mousePosition, Position) <= radius * Scale;
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
