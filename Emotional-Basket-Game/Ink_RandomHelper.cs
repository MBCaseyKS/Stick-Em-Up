using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame
{
    /// <summary>
    /// A class with several Random-incorporating functions that handle some common actions in the game.
    /// </summary>
    public static class Ink_RandomHelper
    {
        private static Random rand = new Random();

        /// <summary>
        /// Returns a random float value.
        /// </summary>
        /// <param name="maxVal">The maximum value.</param>
        /// <returns>A random float.</returns>
        public static float nextFloat(float maxVal) => (float)(rand.NextDouble() * maxVal);

        /// <summary>
        /// Returns a random float value.
        /// </summary>
        /// <returns>A random float.</returns>
        public static float nextFloat() => nextFloat(1.0f);

        /// <summary>
        /// Returns a number within a range.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value</param>
        /// <returns>A randomized number.</returns>
        public static double RandRange(double min, double max) => min + rand.NextDouble() * (max - min);

        /// <summary>
        /// Returns a number within a range.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value</param>
        /// <returns>A randomized number.</returns>
        public static float RandRange(float min, float max) => (float)(min + rand.NextDouble() * (max - min));

        /// <summary>
        /// Creates a random vector with a range of -maxVal to maxVal.
        /// </summary>
        /// <param name="maxVal">The clamp ends.</param>
        /// <returns>A randomized Vector2.</returns>
        public static Vector2 VRand(float maxVal) => new Vector2(RandRange(-maxVal, maxVal), RandRange(-maxVal, maxVal));

        /// <summary>
        /// Creates a random vector with a range of -1.0f to 1.0f.
        /// </summary>
        /// <returns>A randomized Vector2.</returns>
        public static Vector2 VRand() => VRand(1.0f);
    }
}
