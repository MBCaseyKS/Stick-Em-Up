using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Screens
{
    /// <summary>
    /// Shakes a screen using spritebatch transforms.
    /// </summary>
    public class Ink_ScreenShake
    {
        /// <summary>
        /// The maximum offset of this shake.
        /// </summary>
        public Vector2 Intensity { get; set; }

        /// <summary>
        /// How long this shake lasts.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// How long it takes for a shake to get to its maximum shake.
        /// </summary>
        public float FadeIn { get; set; }

        /// <summary>
        /// How long it takes for a shake to fade out.
        /// </summary>
        public float FadeOut { get; set; }

        protected float currentDuration;

        /// <summary>
        /// Constructs a new screen shake.
        /// </summary>
        /// <param name="intensity">The maximum offset of this shake.</param>
        /// <param name="duration">How long this shake lasts.</param>
        /// <param name="fadeIn">How long it takes for a shake to get to its maximum shake.</param>
        /// <param name="fadeOut">How long it takes for a shake to fade out.</param>
        public Ink_ScreenShake(Vector2 intensity, float duration = 0.5f, float fadeIn = 0.1f, float fadeOut = 0.1f)
        {
            Intensity = intensity;
            Duration = Math.Max(duration, 0.01f);
            FadeIn = fadeIn;
            FadeOut = fadeOut;
            currentDuration = 0f;
        }

        /// <summary>
        /// Handles updating all screen shakes.
        /// </summary>
        /// <param name="currentShakes"></param>
        /// <returns></returns>
        public static List<Ink_ScreenShake> UpdateAllShakes(List<Ink_ScreenShake> currentShakes, float delta)
        {
            for (int i = 0; i < currentShakes.Count; i++)
            {
                currentShakes[i].currentDuration = Math.Min(currentShakes[i].currentDuration + delta, currentShakes[i].Duration);
                if (currentShakes[i].currentDuration >= currentShakes[i].Duration)
                {
                    currentShakes.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            return currentShakes;
        }

        /// <summary>
        /// Gets the cumulative shake offset from all camera shakes.
        /// </summary>
        /// <param name="currentShakes">The list of shakes.</param>
        /// <returns>The current shake offset.</returns>
        public static Vector2 GetShakeOffset(List<Ink_ScreenShake> currentShakes)
        {
            Vector2 totalOffset = Vector2.Zero;
            for (int i = 0; i < currentShakes.Count; i++)
            {
                Vector2 currOffset = currentShakes[i].Intensity;

                //Intro fade.
                float fadeTime = Math.Min(currentShakes[i].FadeIn, currentShakes[i].Duration);
                if (fadeTime > 0)
                    currOffset *= Math.Min(currentShakes[i].currentDuration / fadeTime, 1.0f);

                //Outro fade.
                fadeTime = Math.Min(currentShakes[i].FadeOut, currentShakes[i].Duration);
                if (fadeTime > 0)
                    currOffset *= Math.Min((currentShakes[i].Duration - currentShakes[i].currentDuration) / fadeTime, 1.0f);

                totalOffset += Ink_RandomHelper.VRand(currOffset);
            }

            return totalOffset;
        }
    }
}
