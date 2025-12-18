using EmotionalBasketGame.Actors.HUDs;
using EmotionalBasketGame.Actors.Targets;
using EmotionalBasketGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmotionalBasketGame.Actors
{
    /// <summary>
    /// What style of target throw it is.
    /// </summary>
    public enum TargetType
    {
        FromSides,
        FromBelow
    }

    /// <summary>
    /// Sends some data about rounds.
    /// </summary>
    public enum RoundEventData
    {
        WasFailed,
        WasCompleted,
        WasRestarted,
        WasExit
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
        /// The speed multiplier for the target.
        /// </summary>
        public double SizeMulti;

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
        /// The world.
        /// </summary>
        public Ink_GameScreen_Base World {  get; set; }

        /// <summary>
        /// The amount of darts thrown.
        /// </summary>
        public int DartsThrown { get; set; }

        /// <summary>
        /// The amount of darts thrown.
        /// </summary>
        public int DartsMissed { get; set; }

        /// <summary>
        /// The amount of targets hit.
        /// </summary>
        public int TargetsHit { get; set; }

        /// <summary>
        /// The invincibility frames.
        /// </summary>
        public double BlinkTime { get; private set; }

        /// <summary>
        /// The current health.
        /// </summary>
        public int Health { get; private set; }

        /// <summary>
        /// The maximum health.
        /// </summary>
        public int HealthMax { get; private set; } = 3;

        /// <summary>
        /// Whether or not the round is paused.
        /// </summary>
        public bool IsPaused { get; private set; } = false;

        /// <summary>
        /// Is called when a round is completed, and whatever's hosting the round will handle the effects afterwards.
        /// </summary>
        /// <param name="round">The round that was complete</param>
        public delegate void OnRoundEvent(Ink_TargetRound_Layout round, RoundEventData data);
        private OnRoundEvent roundEventDel;

        /// <summary>
        /// The list of all target data.
        /// </summary>
        private List<TargetData> dataArr = new();

        private SoundEffectInstance failedSoundInst;
        private Ink_HUD_Misses missesHud;

        /// <summary>
        /// The progress before the next data is called.
        /// </summary>
        private double dataProgress;

        private bool isEndless;
        private int currentDifficulty;

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
            if (IsPaused) return;

            if (BlinkTime > 0)
                BlinkTime -= gameTime.ElapsedGameTime.TotalSeconds;

            if (dataArr != null && dataArr.Count > 0)
            {
                dataProgress += gameTime.ElapsedGameTime.TotalSeconds;
                while (dataArr.Count > 0 && dataProgress >= dataArr[0].TargetDelay)
                {
                    ThrowTarget(dataArr[0]);
                    dataProgress = MathHelper.Max((float)(dataProgress - dataArr[0].TargetDelay), 0);
                    dataArr.RemoveAt(0);
                }

                if (dataArr.Count <= 0)
                {
                    if (isEndless)
                    {
                        currentDifficulty++;
                        InitializeTargets();
                    }
                    else
                        TriggerRoundDelegate(RoundEventData.WasCompleted);
                }
            }
        }

        /// <summary>
        /// Throws a target.
        /// </summary>
        /// <param name="data">The data for the target.</param>
        public void ThrowTarget(TargetData data)
        {
            Ink_Target_Base target = null;
            switch (data.Type)
            {
                case TargetType.FromBelow:
                    target = (Ink_Target_Base)World.AddActor(new Ink_Target_Thrown(new Vector2(Ink_RandomHelper.RandRange(-100f, 100f), (float)(-data.ThrowStrength)), data.SizeMulti), new Vector2((float)data.ThrowLocation, 450), true, 1);
                    target.FailedDel = this.OnTargetMissed;
                    target.SpeedMulti = data.SpeedMulti;
                    break;
                case TargetType.FromSides:
                    int leftMulti = Ink_RandomHelper.Next(2) == 0 ? -1 : 1;
                    target = (Ink_Target_Base)World.AddActor(new Ink_Target_Thrown(new Vector2(Ink_RandomHelper.RandRange(500f, 700f) * -leftMulti, (float)(-data.ThrowStrength)), data.SizeMulti), new Vector2(800 * leftMulti, data.ThrowLocation), true, 1);
                    target.FailedDel = this.OnTargetMissed;
                    target.SpeedMulti = data.SpeedMulti;
                    break;
            }
        }

        /// <summary>
        /// Initializes the next line of targets.
        /// </summary>
        public void InitializeTargets()
        {
            var typeArr = Enum.GetValues<TargetType>();
            List<int> randomArr = GetRandomOrderedIdxArray(0, 1, typeArr.Length * 2);
            dataArr.Clear();

            for (int i = 0; i < randomArr.Count; i++)
            {
                TargetType currentType = typeArr[randomArr[i] % typeArr.Length];
                TargetData newData = new TargetData();
                newData.Type = currentType;
                newData.TargetDelay = Ink_RandomHelper.RandRange(1.0, MathHelper.Lerp(3.0f, 2.0f, MathHelper.Min(currentDifficulty / 5.0f, 1.0f)));
                newData.SizeMulti = MathHelper.Lerp(2.5f, 1.25f, MathHelper.Min(currentDifficulty / 10.0f, 1.0f));

                if (currentType == TargetType.FromBelow)
                {
                    newData.ThrowLocation = Ink_RandomHelper.RandRange(-500, 500);
                    newData.ThrowStrength = Ink_RandomHelper.RandRange(800, 1200);
                    newData.SpeedMulti = 1.0 + MathHelper.Min(currentDifficulty, 20) * 0.375 * Ink_RandomHelper.RandRange(0.15f, 0.2f);
                }
                else
                {
                    newData.ThrowLocation = Ink_RandomHelper.RandRange(-250, 100);
                    newData.ThrowStrength = Ink_RandomHelper.RandRange(300, 500);
                    newData.SpeedMulti = 1.0 + MathHelper.Min(currentDifficulty, 20) * 0.25 * Ink_RandomHelper.RandRange(0.15f, 0.2f);
                }

                dataArr.Add(newData);
            }
        }

        /// <summary>
        /// Gets an ascending list of integers in a randomized order. Fun fact: I use this a lot in Hat mods!
        /// </summary>
        /// <param name="StartIdx">The starting index.</param>
        /// <param name="IdxSpacing">The increments between indexes.</param>
        /// <param name="TotalIdx">The length of the array.</param>
        /// <param name="LastUsedIdx">Avoids this being the first entry if it's set.</param>
        /// <returns>A random ordered list.</returns>
        public List<int> GetRandomOrderedIdxArray(int StartIdx, int IdxSpacing, int TotalIdx, int? LastUsedIdx = null)
        {
            int CurrIdx = StartIdx;
            List<int> IdxArr = new List<int>();

            for (int i = 0; i < TotalIdx; i++)
            {
                IdxArr.Add(CurrIdx);
                CurrIdx += IdxSpacing;
            }

            List<int> RandArr = new List<int>();
            for (int i = 0; i < IdxArr.Count; i++)
            {
                int RandInt = Ink_RandomHelper.Next(IdxArr.Count);
                if (LastUsedIdx != null && IdxArr[RandInt] == LastUsedIdx && RandArr.Count <= 0 && IdxArr.Count > 1)
                    RandInt = (RandInt + 1) % IdxArr.Count;

                RandArr.Add(IdxArr[RandInt]);
                IdxArr.RemoveAt(RandInt);
                i--;
            }

            return RandArr;
        }

        /// <summary>
        /// Triggers the event delegate.
        /// </summary>
        /// <param name="data">The data for the event.</param>
        public void TriggerRoundDelegate(RoundEventData data)
        {
            if (roundEventDel != null)
                roundEventDel(this, data);
        }

        /// <summary>
        /// Is called when a target fails.
        /// </summary>
        /// <param name="target">The failed target.</param>
        public void OnTargetMissed(Ink_Target_Base target)
        {
            if (BlinkTime > 0) return;
            if (Health <= 0) return;

            World.PlaySoundInst(failedSoundInst, Ink_RandomHelper.RandRange(0.5f, 0.6f), Ink_RandomHelper.RandRange(-0.1f, 0.1f));
            World.DoScreenShake(new Vector2(10f), 0.75f, 0.01f, 0.25f);
            BlinkTime = 1.5;
            Health--;

            if (Health <= 0)
                OnFailed();
        }

        /// <summary>
        /// Fails the round.
        /// </summary>
        public void OnFailed()
        {
            IsPaused = true;
            TriggerRoundDelegate(RoundEventData.WasFailed);
        }

        /// <summary>
        /// Creates a round.
        /// </summary>
        /// <param name="roundCompleteDel">The delegate for completing a round.</param>
        public Ink_TargetRound_Layout(Ink_GameScreen_Base world, OnRoundEvent roundEventDel, bool isEndless = false)
        {
            World = world;
            this.roundEventDel = roundEventDel;
            this.isEndless = isEndless;
        }

        /// <summary>
        /// Loads the actor's content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content)
        {
            World.InitSoundInstance(content, "WAV_Target_Miss", out failedSoundInst);
            missesHud = (Ink_HUD_Misses)World.OpenHUD(new Ink_HUD_Misses(World.Game, World));
            missesHud.RoundHandler = this;
            missesHud.OnRefresh();
        }

        /// <summary>
        /// Handles when a round is restarted.
        /// </summary>
        public void OnRefresh()
        {
            Health = HealthMax;
            IsPaused = false;
            currentDifficulty = 0;
            DartsThrown = 0;
            DartsMissed = 0;
            TargetsHit = 0;
            BlinkTime = 0;
            dataArr.Clear();

            if (missesHud != null)
                missesHud.OnRefresh();
        }

        /// <summary>
        /// Cleans up the round.
        /// </summary>
        public void CleanUp()
        {
            missesHud.DoClose();
        }

        /// <summary>
        /// Creates a round.
        /// </summary>
        public Ink_TargetRound_Layout() { }
    }
}
