using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors.Buttons
{
    /// <summary>
    /// Plays a sound when hovered.
    /// </summary>
    public class Ink_Button_HiddenSound : Ink_Button_Base
    {
        private Texture2D debugTexture;
        private SoundEffectInstance onClickedSound;
        private string soundPathName;
        private double radius;

        public Ink_Button_HiddenSound(string soundPathName, double radius)
        {
            this.soundPathName = soundPathName;
            this.radius = radius;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            InitSoundInstance(content, soundPathName, out onClickedSound);
            //debugTexture = content.Load<Texture2D>("T2D_Target");
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (debugTexture != null)
            {
                Vector2 position = GetScreenPosition();
                float screenScale = GetScreenScale();
                spriteBatch.Draw(debugTexture, position, new Rectangle(0,0,512,512), Color.White, 0, new Vector2(256, 256), 0.25f * screenScale, SpriteEffects.None, 0f);
            }
        }

        public override bool IsInRange(Vector2 mousePosition)
        {
            return Math.Pow(radius, 2) >=
                Math.Pow(Position.X - mousePosition.X, 2) +
                Math.Pow(Position.Y - mousePosition.Y, 2);
        }

        public override bool OnClicked(Vector2 mousePosition)
        {
            PlaySoundInst(onClickedSound, Ink_RandomHelper.RandRange(0.85f, 1.0f), Ink_RandomHelper.RandRange(-0.2f, 0.2f));
            return true;
        }

        public override void OnHovered(Vector2 mousePosition) { }
        public override void OnUnhovered(Vector2 mousePosition) { }
    }
}
