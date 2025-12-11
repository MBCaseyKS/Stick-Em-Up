using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EmotionalBasketGame.Particles;
using EmotionalBasketGame;
using System;

namespace ParticleSystemExample
{
    public class Ink_Particle_Stars : ParticleSystem
    {
        Color[] colors = new Color[]
        {
            Color.Fuchsia,
            Color.Red,
            Color.Crimson,
            Color.CadetBlue,
            Color.Aqua,
            Color.HotPink,
            Color.LimeGreen
        };

        Color color;

        public Ink_Particle_Stars(Game game, int maxExplosions, Color[] colors) : base(game, maxExplosions * 25) 
        {
            if (colors != null && colors.Length > 0)
                this.colors = colors;
        }

        protected override void InitializeConstants()
        {
            textureFilename = "T2D_StarParticle";

            minNumParticles = 20;
            maxNumParticles = 25;

            BlendState = BlendState.Additive;
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            var velocity = Ink_RandomHelper.NextDirection() * Ink_RandomHelper.RandRange(40, 200);

            var lifetime = Ink_RandomHelper.RandRange(0.5f, 1f);

            var acceleration = -velocity / lifetime;

            var rotation = Ink_RandomHelper.RandRange(0, MathHelper.TwoPi);

            var angularVelocity = Ink_RandomHelper.RandRange(-MathHelper.PiOver4, MathHelper.PiOver4);

            var scale = Ink_RandomHelper.RandRange(4, 6);

            p.Initialize(where, velocity, acceleration, color, lifetime: lifetime, rotation: rotation, angularVelocity: angularVelocity, scale: scale);
        }

        protected override void UpdateParticle(ref Particle particle, float dt)
        {
            base.UpdateParticle(ref particle, dt);

            float normalizedLifetime = particle.TimeSinceStart / particle.Lifetime;
            float endLifetime = Math.Min((particle.Lifetime - particle.TimeSinceStart) / (particle.Lifetime * 0.2f), 1.0f);

            particle.Scale = (.1f + .25f * normalizedLifetime) * endLifetime;
        }

        public void PlaceFirework(Vector2 where)
        {
            color = colors[Ink_RandomHelper.Next(colors.Length)];
            AddParticles(where);
        }
    }
}
