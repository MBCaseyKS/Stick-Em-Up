using EmotionalBasketGame.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalBasketGame
{
    /// <summary>
    /// The data for a singular track within the node.
    /// </summary>
    public struct TrackData
    {
        /// <summary>
        /// The track instance, its filename, and lengths. For intro and looping songs.
        /// </summary>
        public List<(SoundEffectInstance instance, double length)> TrackInsts;

        /// <summary>
        /// The time it takes for this track to fade in, and the others to fade out.
        /// </summary>
        public double TrackFadeTime;

        /// <summary>
        /// The maximum volume of this track.
        /// </summary>
        public double TrackVolume;

        /// <summary>
        /// The current individual volume of the track, NOT including the overall node's multiplier.
        /// </summary>
        public double CurrVolume;

        /// <summary>
        /// How far along this song is into playing.
        /// </summary>
        public double TrackProgress;

        /// <summary>
        /// The section idx of this song.
        /// </summary>
        public int SectionIdx;

        public TrackData()
        {
            TrackInsts = new();
        }
    }

    /// <summary>
    /// Stores a list of tracks.
    /// </summary>
    public struct MusicNode
    {
        /// <summary>
        /// The current list of tracks.
        /// </summary>
        public List<TrackData> Tracks;

        /// <summary>
        /// The node's key.
        /// </summary>
        public string Key;

        /// <summary>
        /// The active track layer.
        /// </summary>
        public int ActiveIdx;

        /// <summary>
        /// The song's priority.
        /// </summary>
        public int Priority;

        /// <summary>
        /// The node's fade time.
        /// </summary>
        public double NodeFadeTime;

        /// <summary>
        /// Stops and eventually restarts tracks in the node if everything has been fully faded out.
        /// </summary>
        public bool RestartWhenCeaseRelevant;

        /// <summary>
        /// The node's current volume, from 0-1.
        /// </summary>
        public double CurrentTrackVolume;

        /// <summary>
        /// The node's current volume, based on if paused or not, from 0-1.
        /// </summary>
        public double CurrentPauseVolume;

        /// <summary>
        /// Deletes the track once it's fully faded out.
        /// </summary>
        public bool RemoveOnceCeased;

        /// <summary>
        /// Whether or not the node is paused.
        /// </summary>
        public bool IsPaused;

        /// <summary>
        /// Whether or not the node is playing.
        /// </summary>
        public bool IsPlaying;

        /// <summary>
        /// Whether or not the node is being removed or not.
        /// </summary>
        public bool IsBeingRemoved;

        /// <summary>
        /// Initializes a node.
        /// </summary>
        public MusicNode()
        {
            Tracks = new List<TrackData>();
            ActiveIdx = 0;
            Priority = 0;
        }
    }

    /// <summary>
    /// Handles all music for the game.
    /// </summary>
    public class Ink_MusicManager
    {
        protected List<MusicNode> AllNodes;
        protected ContentManager Content;

        public Ink_MusicManager(ContentManager content)
        {
            AllNodes = new List<MusicNode>();
            Content = content;
        }


        /// <summary>
        /// Updates the music manager, as in a Tick.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            int playingIdx = -1;
            double fadeAlpha = 0;
            double delta = gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < AllNodes.Count; i++)
            {
                MusicNode currNode = AllNodes[i];

                if (playingIdx < 0 && !currNode.IsPaused && !currNode.IsBeingRemoved)
                {
                    playingIdx = i;
                    fadeAlpha = currNode.NodeFadeTime;
                }

                double prevVolume = currNode.CurrentTrackVolume;
                currNode = UpdateNode(currNode, i == playingIdx, currNode.IsBeingRemoved ? currNode.NodeFadeTime : fadeAlpha, delta);

                if (prevVolume > 0 && currNode.CurrentTrackVolume <= 0)
                {
                    if (currNode.IsBeingRemoved || currNode.RemoveOnceCeased || currNode.RestartWhenCeaseRelevant)
                    {
                        currNode.IsPlaying = false;
                        SetTracksPlaying(currNode, false);
                    }
                    else
                        SetTracksPaused(currNode, true);

                    if (currNode.IsBeingRemoved || currNode.RemoveOnceCeased)
                    {
                        AllNodes.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
                else if (currNode.CurrentTrackVolume > 0)
                {
                    if (!currNode.IsPlaying)
                    {
                        currNode.IsPlaying = true;
                        SetTracksPlaying(currNode, true);
                        SetTracksPaused(currNode, false);
                    }
                }

                AllNodes[i] = currNode;
            }
        }

        /// <summary>
        /// Updates a music node.
        /// </summary>
        /// <param name="node"></param>
        public MusicNode UpdateNode(MusicNode node, bool isActive, double fadeTime, double delta)
        {
            if (isActive)
            {
                if (fadeTime <= 0)
                    node.CurrentTrackVolume = 1;
                else
                    node.CurrentTrackVolume = Math.Min(node.CurrentTrackVolume + delta / fadeTime, 1);
            }
            else
            {
                if (fadeTime <= 0)
                    node.CurrentTrackVolume = 0;
                else
                    node.CurrentTrackVolume = Math.Max(node.CurrentTrackVolume - delta / fadeTime, 0);
            }

            if (node.IsPaused)
            {
                if (node.NodeFadeTime <= 0)
                    node.CurrentPauseVolume = 0;
                else
                    node.CurrentPauseVolume = Math.Max(node.CurrentPauseVolume - delta / node.NodeFadeTime, 0);
            }
            else
            {
                if (node.NodeFadeTime <= 0)
                    node.CurrentPauseVolume = 1;
                else
                    node.CurrentPauseVolume = Math.Min(node.CurrentPauseVolume + delta / node.NodeFadeTime, 1);
            }

            //bool hasActiveTrack = false;
            for (int i = 0; i < node.Tracks.Count; i++)
            {
                TrackData track = node.Tracks[i];
                if (track.TrackInsts[track.SectionIdx].instance.State == SoundState.Playing || track.TrackInsts[track.SectionIdx].instance.State == SoundState.Stopped)
                {
                    //hasActiveTrack = true;
                    track.TrackProgress += delta;
                    node.Tracks[i] = track;
                    break;
                }
            }

            for (int i = 0; i < node.Tracks.Count; i++)
            {
                TrackData track = node.Tracks[i];

                if (i == node.ActiveIdx)
                {
                    if (node.Tracks[node.ActiveIdx].TrackFadeTime > 0)
                        track.CurrVolume = Math.Min(track.CurrVolume + delta / node.Tracks[node.ActiveIdx].TrackFadeTime, track.TrackVolume);
                    else
                        track.CurrVolume = track.TrackVolume;
                }
                else
                {
                    if (node.Tracks[node.ActiveIdx].TrackFadeTime > 0)
                        track.CurrVolume = Math.Max(track.CurrVolume - delta / node.Tracks[node.ActiveIdx].TrackFadeTime, 0);
                    else
                        track.CurrVolume = 0;
                }

                if (track.TrackInsts[track.SectionIdx].length >= 0 && track.TrackProgress >= track.TrackInsts[track.SectionIdx].length)
                {
                    //We've reached the end of this section.
                    track.SectionIdx++;
                    track.TrackProgress = 0;

                    if (track.SectionIdx >= track.TrackInsts.Count)
                    {
                        node.Tracks.RemoveAt(i);
                        i--;
                        break;
                    }
                    else
                        track.TrackInsts[track.SectionIdx].instance.Play();
                }

                track.TrackInsts[track.SectionIdx].instance.Volume = (float)(track.CurrVolume * node.CurrentTrackVolume * node.CurrentPauseVolume);
                node.Tracks[i] = track;
            }

            return node;
        }

        /// <summary>
        /// Plays all tracks in a node. Does not worry about volume.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <param name="b">Whether or not to play the tracks.</param>
        public void SetTracksPlaying(MusicNode node, bool b)
        {
            foreach (TrackData track in node.Tracks)
            {
                if (track.SectionIdx >= track.TrackInsts.Count) continue;

                if (b)
                    track.TrackInsts[track.SectionIdx].instance.Play();
                else
                    track.TrackInsts[track.SectionIdx].instance.Stop();
            }
        }

        /// <summary>
        /// Pauses/Unpauses all tracks in a node.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <param name="b">Whether or not to pause the tracks.</param>
        public void SetTracksPaused(MusicNode node, bool b)
        {
            foreach (TrackData track in node.Tracks)
            {
                if (track.SectionIdx >= track.TrackInsts.Count) continue;

                for (int i = 0; i < track.TrackInsts.Count; i++)
                {
                    if (b)
                        track.TrackInsts[i].instance.Pause();
                    else
                        track.TrackInsts[i].instance.Resume();
                }
            }
        }

        /// <summary>
        /// Attempts to add a new music node, complete with a 
        /// </summary>
        /// <param name="trackList">The list of track data.</param>
        /// <param name="nodeFadeTime">The fade time for the overall node.</param>
        /// <param name="restartIfCeaseRelevant">Whether or not to fully restart the node if playing.</param>
        /// <returns>Whether or not adding the node was successful (IE: No tracks failed to load.)</returns>
        public bool AddMusicNode(((string, double)[] songSections, double fadeTime, double volume)[] trackList,
            string key = "",
            bool autoPlay = true,
            int priority = 0,
            double nodeFadeTime = 0.1,
            bool restartIfCeaseRelevant = true)
        {
            if (key.Length > 0)
            {
                int idx = AllNodes.FindIndex(x => x.Key.ToLower() == key.ToLower());
                if (idx >= 0)
                    return false; //Cannot add two nodes of the same key.
            }

            MusicNode node = new MusicNode();
            foreach (((string, double)[] songSections, double fadeTime, double volume) track in trackList)
            {
                if (!InitSongInstance(out TrackData newData, track.songSections, track.fadeTime, track.volume)) return false;

                if (node.Tracks.Count > 0)
                    newData.CurrVolume = 0;
                else
                    newData.CurrVolume = newData.TrackVolume;

                node.Tracks.Add(newData);
            }

            node.Key = key;
            node.IsPaused = !autoPlay;
            node.CurrentPauseVolume = autoPlay ? 1 : 0;
            node.Priority = priority;
            node.RestartWhenCeaseRelevant = restartIfCeaseRelevant;
            for (int i = 0; i < AllNodes.Count; i++)
            {
                if (AllNodes[i].Priority > node.Priority) continue;
                AllNodes.Insert(i, node);
                return true;
            }

            //If we reached the end of the array, simply add the node to the end.
            AllNodes.Add(node);
            return true;
        }

        /// <summary>
        /// Sets a track playing.
        /// </summary>
        /// <param name="key">The key of the track playing. If empty, it will use the first available track.</param>
        /// <param name="isPlaying">Whether or not to play.</param>
        public void SetTrackPlaying(string key, bool isPlaying)
        {
            int searchIdx = AllNodes.FindIndex(x => x.Key.ToLower() == key.ToLower());
            if (searchIdx < 0)
                return;

            MusicNode node = AllNodes[searchIdx];
            node.IsPaused = !isPlaying;
            AllNodes[searchIdx] = node;
        }

        /// <summary>
        /// Fades out a track and removes it from the list.
        /// </summary>
        /// <param name="key">The key of the track playing. If empty, it will use the first available track.</param>
        /// <param name="fadeTime">The maximum time before the track is removed.</param>
        public void StopTrack(string key, double fadeTime)
        {
            int searchIdx = AllNodes.FindIndex(x => x.Key.ToLower() == key.ToLower());
            if (searchIdx < 0)
                return;

            MusicNode node = AllNodes[searchIdx];
            node.IsBeingRemoved = true;
            node.NodeFadeTime = fadeTime;
            AllNodes[searchIdx] = node;
        }

        /// <summary>
        /// Fades out all tracks in the queue.
        /// </summary>
        /// <param name="fadeTime">The maximum time before the track is removed.</param>
        public void ClearQueue(double fadeTime)
        {
            for (int i = 0; i < AllNodes.Count; i++)
            {
                MusicNode node = AllNodes[i];
                node.IsBeingRemoved = true;
                node.NodeFadeTime = fadeTime;
                AllNodes[i] = node;
            }
        }

        /// <summary>
        /// Sets a track's layer.
        /// </summary>
        /// <param name="key">The key of the track playing. If empty, it will use the first available track.</param>
        /// <param name="layer">The layer of the track.</param>
        public void SetTrackLayer(string key, int layer)
        {
            int searchIdx = AllNodes.FindIndex(x => x.Key.ToLower() == key.ToLower());
            if (searchIdx < 0)
                return;

            MusicNode node = AllNodes[searchIdx];
            if (node.Tracks.Count <= 0) return;

            layer = Math.Clamp(layer, 0, node.Tracks.Count - 1);
            node.ActiveIdx = layer;
            AllNodes[searchIdx] = node;
        }

        /// <summary>
        /// Attempts to init a song instance.
        /// </summary>
        /// <param name="data">The data that'll be filled.</param>
        /// <param name="fileName">The song's filename from the content folder.</param>
        /// <param name="fadeTime">The fade time for the song to enter.</param>
        /// <param name="volume">The song's maximum volume.</param>
        /// <returns>Whether or not loading the song was successful.</returns>
        public bool InitSongInstance(out TrackData data, (string fileName, double trackLength)[] songSections, double fadeTime, double volume)
        {
            SoundEffect loadedSound;
            data = new TrackData();

            if (songSections.Length <= 0) return false; //PUT IN SOME FILES DUMMY

            try
            {
                data.TrackInsts.Clear();

                for (int i = 0; i < songSections.Length; i++)
                {
                    loadedSound = Content.Load<SoundEffect>(songSections[i].fileName);
                    data.TrackInsts.Add((loadedSound.CreateInstance(), songSections[i].trackLength));
                    data.TrackInsts[i].instance.IsLooped = songSections[i].trackLength < 0;
                    data.TrackInsts[i].instance.Volume = 0; //Start at 0 volume.
                }
            }
            catch (Exception)
            {
                //Whatever you bozos did, it failed to load!
                loadedSound = null;
                return false;
            }

            data.SectionIdx = 0;
            data.TrackFadeTime = fadeTime;
            data.TrackVolume = volume;
            return true;
        }
    }
}
