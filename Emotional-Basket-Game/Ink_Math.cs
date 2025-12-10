using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmotionalBasketGame
{
    /// <summary>
    /// Provides interpolation functions and other stuff.
    /// </summary>
    public static class Ink_Math
    {

        /// <summary>
        /// A modification of the Unity ease out elastic function provided.
        /// </summary>
        /// <param name="linearAlpha">The alpha.</param>
        /// <returns>The eased alpha.</returns>
        public static float EaseOutElastic(float linearAlpha)
        {
            float c4 = (float)(2 * Math.PI) / 3;

            if (linearAlpha <= 0)
                return 0f;
            else if (linearAlpha >= 1)
                return 1f;
            else
                return (float)(Math.Pow(2, -10 * linearAlpha) * Math.Sin((linearAlpha * 10 - 0.75) * c4) + 1);
        }

        /// <summary>
        /// A modification of the Unity ease out bounce function provided.
        /// </summary>
        /// <param name="linearAlpha">The alpha.</param>
        /// <returns>The eased alpha.</returns>
        public static float EaseOutBounce(float linearAlpha)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (linearAlpha < 1 / d1)
                return n1 * linearAlpha * linearAlpha;
            else if (linearAlpha < 2 / d1)
                return n1 * (linearAlpha -= 1.5f / d1) * linearAlpha + 0.75f;
            else if (linearAlpha < 2.5 / d1)
                return n1 * (linearAlpha -= 2.25f / d1) * linearAlpha + 0.9375f;
            else
                return n1 * (linearAlpha -= 2.625f / d1) * linearAlpha + 0.984375f;

        }

        /// <summary>
        /// A modification of the Unity ease out back function provided.
        /// </summary>
        /// <param name="linearAlpha">The alpha.</param>
        /// <returns>The eased alpha.</returns>
        public static float EaseOutBack(float linearAlpha) 
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return (float)(1 + c3 * Math.Pow(linearAlpha - 1f, 3f) + c1 * Math.Pow(linearAlpha - 1f, 2f));
        }

        /// <summary>
        /// A modification of the Unity ease out cubic function provided.
        /// </summary>
        /// <param name="linearAlpha">The alpha.</param>
        /// <returns>The eased alpha.</returns>
        public static float EaseOutCubic(float linearAlpha) => (float)(1 - Math.Pow(1 - linearAlpha, 3));
    }
}
