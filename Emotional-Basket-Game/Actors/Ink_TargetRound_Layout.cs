using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// What style of target throw it is.
    /// </summary>
    public enum TargetType
    {
        Static,
        FromSides,
        FromBelow
    }

    /// <summary>
    /// Contains the data to specify everything about a target being thrown.
    /// </summary>
    public struct TargetData
    {
        /// <summary>
        /// The type of target this one is.
        /// </summary>
        public TargetType Type;

        /// <summary>
        /// The delay for this target to be thrown after the previous one.
        /// </summary>
        public double TargetDelay;

        /// <summary>
        /// The speed multiplier for the target.
        /// </summary>
        public double SpeedMulti;

        /// <summary>
        /// Where the target is thrown from. If from sides, and set negative, it is thrown from the left.
        /// </summary>
        public float ThrowLocation;

        /// <summary>
        /// The upwards strength of the throw.
        /// </summary>
        public double ThrowStrength;
    }

    /// <summary>
    /// Handles the spawning of targets during a round.
    /// </summary>
    public class Ink_TargetRound_Layout
    {
        /// <summary>
        /// Is called when a round is completed, and whatever's hosting the round will handle the effects afterwards.
        /// </summary>
        /// <param name="round">The round that was complete</param>
        public delegate void OnRoundComplete(Ink_TargetRound_Layout round);
        private OnRoundComplete roundCompleteDel;

        /// <summary>
        /// The list of all target data.
        /// </summary>
        private List<TargetData> dataArr = new();

        /// <summary>
        /// The progress before the next data is called.
        /// </summary>
        private double dataProgress;

        /// <summary>
        /// Adds new target data.
        /// </summary>
        /// <param name="type">The type of target is.</param>
        /// <param name="delay">The delay for this target to be thrown after the previous one.</param>
        /// <param name="speedMulti">The speed multiplier for the target</param>
        /// <param name="throwLocation">The area this target is thrown from.</param>
        /// <param name="throwStrength">The strength of the throw.</param>
        /// <returns></returns>
        public TargetData AddData(TargetType type, double delay, double speedMulti, float throwLocation, double throwStrength)
        {
            TargetData newData = new()
            {
                Type = type,
                TargetDelay = delay,
                SpeedMulti = speedMulti,
                ThrowLocation = throwLocation,
                ThrowStrength = throwStrength
            };

            return newData;
        }

        /// <summary>
        /// Updates the round when called.
        /// </summary>
        /// <param name="gameTime"></param>
        public void TickRound(GameTime gameTime)
        {
            if (dataArr != null && dataArr.Count > 0)
            {
                dataProgress += gameTime.ElapsedGameTime.TotalSeconds;
                while (dataArr.Count > 0 && dataProgress >= dataArr[0].TargetDelay)
                {
                    dataProgress = MathHelper.Max((float)(dataProgress - dataArr[0].TargetDelay), 0);
                    dataArr.RemoveAt(0);
                }

                if (dataArr.Count <= 0 && roundCompleteDel != null)
                    roundCompleteDel(this);
            }
        }

        /// <summary>
        /// Creates a round.
        /// </summary>
        /// <param name="roundCompleteDel">The delegate for completing a round.</param>
        public Ink_TargetRound_Layout(OnRoundComplete roundCompleteDel)
        {
            this.roundCompleteDel = roundCompleteDel;
        }

        /// <summary>
        /// Creates a round.
        /// </summary>
        public Ink_TargetRound_Layout() { }
    }
}
