using EmotionalBasketGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors.HUDs
{
    /// <summary>
    /// Displays the amount of fails made.
    /// </summary>
    public class Ink_HUD_Misses : Ink_HUD_Base
    {
        /// <summary>
        /// The round handler.
        /// </summary>
        public Ink_TargetRound_Layout RoundHandler { get; set; }

        private List<double> missAlphas;
        private Texture2D missIcon;
        private double blinkProgress;
        private double idleProgress;

        public Ink_HUD_Misses(Ink_PinGameManager game, Ink_GameScreen_Base activeScreen, int renderPriority = 0) : base(game, activeScreen, renderPriority) { }

        /// <summary>
        /// Re-inits the HUD.
        /// </summary>
        public void OnRefresh()
        {
            missAlphas = new List<double>();
            for (int i = 0; i < RoundHandler.HealthMax; i++)
                missAlphas.Add(0);
        }

        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            missIcon = content.Load<Texture2D>("T2D_MissIcon");
        }

        public override void Update(GameTime gameTime)
        {
            if (RoundHandler != null)
            {
                idleProgress += gameTime.ElapsedGameTime.TotalSeconds * (1.0 + (RoundHandler.HealthMax - RoundHandler.Health));
                if (RoundHandler.Health > 0 && RoundHandler.BlinkTime > 0)
                    blinkProgress = Math.Max(blinkProgress, 0) + gameTime.ElapsedGameTime.TotalSeconds * MathHelper.Lerp(2.0f, 1.0f, MathHelper.Min((float)(RoundHandler.BlinkTime / 0.5), 1.0f));
                else
                    blinkProgress = -1;

                for (int i = 0; i < missAlphas.Count; i++)
                {
                    if (RoundHandler.Health <= i)
                        missAlphas[i] = Math.Min(missAlphas[i] + gameTime.ElapsedGameTime.TotalSeconds * 4, 1.0);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (RoundHandler == null) return;
            if (blinkProgress > 0 && blinkProgress % 0.25 < 0.125) return;

            if (missIcon != null)
            {
                float spacing = 75.0f;
                Vector2 currentPosition = new Vector2(640f + (RoundHandler.HealthMax-1) * spacing * 0.5f, 675f);
                for (int i = 0; i < RoundHandler.HealthMax; i++)
                {
                    double time = idleProgress + i * 0.5;
                    Vector2 offset = new Vector2(5f * (float)Math.Cos(time * 0.5 * Math.PI), 5f * (float)Math.Sin(time * 0.5 * Math.PI));
                    spriteBatch.Draw(missIcon, currentPosition + offset, null, new Color(75, 55, 55, 55), 0, new Vector2(256, 256), 0.1f, SpriteEffects.None, 0f);

                    if (missAlphas.Count > i && missAlphas[i] > 0)
                        spriteBatch.Draw(missIcon, currentPosition + offset, null, new Color(200, 55, 55, 155) * (float)Math.Min(missAlphas[i]*8, 1.0), 0, new Vector2(256, 256), MathHelper.Lerp(0.125f, 0.5f, 1.0f - Ink_Math.EaseOutCubic((float)missAlphas[i])), SpriteEffects.None, 0f);

                    currentPosition.X -= spacing;
                }
            }
        }
    }
}
