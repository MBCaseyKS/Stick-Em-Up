using EmotionalBasketGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors
{
    public abstract class Ink_Actor_Base
    {
        private Ink_Actor_Base basedActor;
        protected Vector2 position;
        protected double depth;

        /// <summary>
        /// The actor's position.
        /// </summary>
        public Vector2 Position
        {
            get => position;
            set
            {
                foreach (Ink_Actor_Base actor in Attached)
                    actor.Position += (value - position);

                position = value;
            }
        }

        /// <summary>
        /// The actor's depth.
        /// </summary>
        public double Depth
        {
            get => depth;
            set
            {
                foreach (Ink_Actor_Base actor in Attached)
                    actor.Depth += (value - depth);

                depth = value;
            }
        }

        /// <summary>
        /// Moves with this actor.
        /// </summary>
        public Ink_Actor_Base BasedActor
        {
            get => basedActor;
            set
            {
                if (value != null && IsBasedOnThisActor(value)) return; //Do NOT attach actors to each other.
                if (basedActor != null)
                    basedActor.Attached.Remove(this);

                basedActor = value;
                basedActor.Attached.Add(this);
            }
        }

        /// <summary>
        /// A list of all actors attached to this actor.
        /// </summary>
        public List<Ink_Actor_Base> Attached = new();

        /// <summary>
        /// The game.
        /// </summary>
        public Ink_PinGameManager Game { get; set; }

        /// <summary>
        /// The world.
        /// </summary>
        public Ink_GameScreen_Base World { get; set; }

        /// <summary>
        /// Gets the game's music manager.
        /// </summary>
        public Ink_MusicManager MusicManager { get => Game.MusicManager; }

        /// <summary>
        /// Gets and/or sets the world's screen offset.
        /// </summary>
        public Vector2 ScreenOffset
        {
            get => World != null ? World.ScreenOffset : Vector2.Zero;
            set
            {
                if (World != null)
                    World.ScreenOffset = value;
            }
        }

        /// <summary>
        /// Sorts the actors and renders them in ascending order.
        /// </summary>
        public int RenderPriority { get; set; } = 0;

        /// <summary>
        /// The active screen offset.
        /// </summary>
        public Matrix ActiveOffset { get; set; } = Matrix.Identity;

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public virtual void LoadContent(ContentManager content) { }

        /// <summary>
        /// Updates the actor, as in a Tick.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// Updates the actor's visuals.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="spriteBatch">The sprite batch provided.</param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }

        /// <summary>
        /// Spawns an actor.
        /// </summary>
        /// <param name="actor">The actor to spawn.</param>
        /// <param name="actorPosition">Where the actor should spawn.</param>
        public void Spawn(Ink_Actor_Base actor, Vector2 actorPosition)
        {
            if (World != null)
                World.AddActor(actor, actorPosition);
        }

        /// <summary>
        /// Inits a soundeffectinstance, so we do not need to store the SoundEffect itself.
        /// </summary>
        /// <param name="content">The content manager.</param>
        /// <param name="fileName">The file name for the sound sample.</param>
        /// <param name="inst">The instance variable that will be filled.</param>
        public void InitSoundInstance(ContentManager content, string fileName, out SoundEffectInstance inst)
        {
            inst = null;
            SoundEffect sound = content.Load<SoundEffect>(fileName);
            if (sound != null)
                inst = sound.CreateInstance();
        }

        /// <summary>
        /// A condensed form of playing a soundeffect instance.
        /// </summary>
        /// <param name="inst">The instance.</param>
        /// <param name="volume">The volume.</param
        /// <param name="pitch">The pitch.</param>
        /// <param name="bLooping">Whether or not to loop the instance.</param>
        public void PlaySoundInst(SoundEffectInstance inst, float volume = 1.0f, float pitch = 1.0f, bool bLooping = false)
        {
            if (inst == null) return;
            if (inst.State == SoundState.Playing) return;

            volume = MathHelper.Clamp(volume, 0.0f, 1.0f);
            inst.Volume = volume;
            inst.Pitch = pitch;
            inst.IsLooped = bLooping;

            inst.Play();
        }

        /// <summary>
        /// A condensed form of playing a soundeffect instance.
        /// </summary>
        /// <param name="inst">The instance.</param>
        /// <param name="volume">The volume.</param
        /// <param name="pitch">The pitch.</param>
        public void PlaySoundInst(SoundEffectInstance inst, float volume = 1.0f, float pitch = 1.0f) => PlaySoundInst(inst, volume, pitch, false);

        /// <summary>
        /// Returns the scale of the screen.
        /// </summary>
        /// <returns>The scale of the screen.</returns>
        public float GetScreenScale() => Game.GetScreenScale();

        /// <summary>
        /// Returns the center of the screen.
        /// </summary>
        /// <returns>The center vector of the screen.</returns>
        public Vector2 GetScreenCenter() => Game.GetScreenCenter();

        /// <summary>
        /// Returns the screen position of this actor.
        /// </summary>
        /// <returns>The adjusted screen position.</returns>
        public Vector2 GetScreenPosition(bool applyOffset = true)
        {
            Vector2 currentPos = Position;
            if (applyOffset)
                currentPos -= ScreenOffset;

            return GetScreenCenter() + currentPos * GetScreenScale() * GetDepthOffsetMulti();
        }

        /// <summary>
        /// Returns how an actor should be scaled with depth.
        /// </summary>
        /// <returns></returns>
        public float GetDepthScale()
        {
            if (Depth < 0)
                return 1.0f + (float)(Math.Abs(Depth) / 32);

            return 1.0f / (1.0f + (float)(Depth / 64));
        }

        /// <summary>
        /// Displays how far an actor should be offsetted from its actual position, based on Depth.
        /// </summary>
        /// <returns></returns>
        public float GetDepthOffsetMulti()
        {
            if (Depth < 0)
                return 1.0f + (float)(Math.Abs(Depth) / 32);

            return 1.0f / (1.0f + (float)(Depth / 64));
        }

        /// <summary>
        /// Recursively returns whether or an actor is based on this actor.
        /// </summary>
        /// <param name="other">The other actor.</param>
        /// <returns>Whether or not this actor is based on the other.</returns>
        public bool IsBasedOnThisActor(Ink_Actor_Base other)
        {
            if (BasedActor == null) return false;
            if (other == null) return false;
            if (other.BasedActor == null) return false;
            if (other.BasedActor == this) return true;

            return IsBasedOnThisActor(other.BasedActor);
        }

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        /// <returns>The mouse position.</returns>
        public Vector2 GetMousePosition()
        {
            MouseState currentMouseState = Mouse.GetState();
            return new Vector2(currentMouseState.X, currentMouseState.Y);
        }

        /// <summary>
        /// Returns if a mouse is in the area or not.
        /// </summary>
        /// <param name="position">The area's position.</param>
        /// <param name="area">The area's size.</param>
        /// <returns></returns>
        public bool IsMouseInArea(Vector2 position, Vector2 area)
        {
            Vector2 mousePos = GetMousePosition();
            return Math.Abs(mousePos.X - position.X) <= area.X*0.5 && Math.Abs(mousePos.Y - position.Y) <= area.Y * 0.5;
        }

        /// <summary>
        /// Returns if a mouse is in the area or not.
        /// </summary>
        /// <param name="position">The area's position.</param>
        /// <param name="area">The area's size.</param>
        /// <returns></returns>
        public bool IsMouseInRadius(Vector2 position, double radius)
        {
            Vector2 mousePos = GetMousePosition();
            return Math.Pow(radius, 2) <= Math.Pow(position.X - mousePos.X, 2) + Math.Pow(position.Y - mousePos.Y, 2);
        }

        /// <summary>
        /// Returns if the SpriteBatch should be remade with a new blend state.
        /// </summary>
        /// <param name="blendState">The custom blend state to use.</param>
        /// <returns>Returns if the SpriteBatch should be remade with a new blend state.</returns>
        public virtual bool GetCustomBlend(out BlendState blendState)
        {
            blendState = null;
            return false;
        }
    }
}
