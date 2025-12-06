using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors
{
    public static class CollisionHelper
    {
        /// <summary>
        /// Checks if two bounding circles collide.
        /// </summary>
        /// <param name="a">The first circle.</param>
        /// <param name="b">The second circle.</param>
        /// <returns>True for collision, false for not.</returns>
        public static bool Collides(BoundingCircle a, BoundingCircle b)
        {
            if (a.Owner == null ||  b.Owner == null) return false;

            return Math.Pow(a.Radius + b.Radius, 2) >= 
                Math.Pow(a.Owner.Position.X - b.Owner.Position.X, 2) + 
                Math.Pow(a.Owner.Position.Y - b.Owner.Position.Y, 2);
        }
    }
}
