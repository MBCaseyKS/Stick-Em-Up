using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// A struct representing circular bounds.
    /// </summary>
    public struct BoundingCircle
    {
        /// <summary>
        /// The center of the bounding circle.
        /// </summary>
        public Ink_Actor_Base Owner;

        /// <summary>
        /// The radius of the bounding circle.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Constructs a new bounding circle.
        /// </summary>
        /// <param name="center">The center of the bounding circle.</param>
        /// <param name="radius">The radius of the bounding circle.</param>
        public BoundingCircle(Ink_Actor_Base owner, float radius)
        {
            this.Owner = owner;
            this.Radius = radius;
        }

        /// <summary>
        /// Checks if this circle collides with another.
        /// </summary>
        /// <param name="other">The other circle.</param>
        /// <returns>Returns true if colliding, false otherwise.</returns>
        public bool CollidesWith(BoundingCircle other) => CollisionHelper.Collides(this, other);
    }
}
