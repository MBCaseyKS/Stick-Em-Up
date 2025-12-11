using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
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
        /// Returns a random integer
        /// </summary>
        /// <returns>An int between 0 and infinity</returns>
        public static int Next() => rand.Next();

        /// <summary>
        /// Returns a random integer between 0 and <paramref name="maxValue"/> 
        /// </summary>
        /// <param name="maxValue">The maximum value to return</param>
        /// <returns>A number between 0 and <paramref name="maxValue"/></returns>
        public static int Next(int maxValue) => rand.Next(maxValue);

        /// <summary>
        /// Returns a random integer within the specified range
        /// </summary>
        /// <param name="minValue">The minimum integer to return</param>
        /// <param name="maxValue">The maximum integer to return</param>
        /// <returns></returns>
        public static int Next(int minValue, int maxValue) => rand.Next(minValue, maxValue);

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

        /// <summary>
        /// Creates a random vector with a range of valueRanges, based on the inputted X and Y.
        /// </summary>
        /// <param name="valueRanges">The vector range, to allow randomness for both X and Y.</param>
        /// <returns>A randomized Vector2.</returns>
        public static Vector2 VRand(Vector2 valueRanges) => new Vector2(RandRange(-valueRanges.X, valueRanges.X), RandRange(-valueRanges.Y, valueRanges.Y));

        /// <summary>
        /// Returns a Vector2 that falls within the supplied <paramref name="bounds"/>
        /// </summary>
        /// <param name="bounds">A rectangle defining the bounds which should contain the point</param>
        /// <returns>A Vector2 wihtin the bounds</returns>
        public static Vector2 RandomPosition(Rectangle bounds) => new Vector2(RandRange(bounds.X, bounds.X + bounds.Width), RandRange(bounds.Y, bounds.Y + bounds.Height));

        /// <summary>
        /// Returns a unit vector in a random direction
        /// </summary>
        /// <returns>A random unit Vector2</returns>
        public static Vector2 NextDirection()
        {
            float angle = RandRange(0, MathHelper.TwoPi);
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }
    }
}
