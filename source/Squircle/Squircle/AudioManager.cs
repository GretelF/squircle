using Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squircle
{
    public class AudioManager
    {
        public Game Game { get; set; }

        public AudioEngine Engine { get; set; }
        public WaveBank Waves { get; set; }
        public SoundBank Sounds { get; set; }

        public string GlobalSettingsFileName { get; set; }
        public string WaveBankFileName { get; set; }
        public string SoundBankFileName { get; set; }

        public IList<Cue> PlayedCues { get; set; }

        public AudioManager(Game game)
        {
            Game = game;
            PlayedCues = new List<Cue>();
        }

        public void Initialize(ConfigSection section)
        {
            GlobalSettingsFileName = section["settings"];
            WaveBankFileName = section["waveBank"];
            SoundBankFileName = section["soundBank"];
        }

        public void CleanUp()
        {
            Sounds.Dispose();
            Waves.Dispose();
            Engine.Dispose();
        }

        public void LoadContent(ContentManager content)
        {
            if (GlobalSettingsFileName == null || Engine != null) return;

            Engine = new AudioEngine(GlobalSettingsFileName);
            Waves = new WaveBank(Engine, WaveBankFileName);
            Sounds = new SoundBank(Engine, SoundBankFileName);
        }

        public void Update(GameTime gameTime)
        {
            if (Engine == null) return;

            Engine.Update();
        }

        public void PlayCueAndStopAllOthers(string cueName)
        {
            foreach (var cue in PlayedCues.Where(c => c.Name != cueName))
            {
                StopCue(cue);
            }
            PlayCue(cueName);
        }

        public void PlayCue(string cueName)
        {
            var cue = PlayedCues.SingleOrDefault(c => c.Name == cueName);
            if (cue == null)
            {
                cue = Sounds.GetCue(cueName);
                PlayedCues.Add(cue);
            }
            PlayCue(cue);
        }

        public void PlayCue(Cue cue)
        {
            if (cue.IsStopped)
            {
                cue = AddOrReplaceCue(cue);
            }
            if (!cue.IsPlaying)
            {
                cue.Play();
            }
        }

        public void StopCue(string cueName, AudioStopOptions options = AudioStopOptions.AsAuthored)
        {
            var cue = PlayedCues.SingleOrDefault(c => c.Name == cueName);
            if (cue == null) { return; }

            StopCue(cue, options);
        }

        public void StopCue(Cue cue, AudioStopOptions options = AudioStopOptions.AsAuthored)
        {
            if (!cue.IsStopped)
            {
                cue.Stop(options);
            }
        }

        public void StopAllCues()
        {
            foreach (var cue in PlayedCues)
            {
                StopCue(cue);
            }
        }

        public bool IsPlayingCue(string cueName)
        {
            var cue = PlayedCues.SingleOrDefault(c => c.Name == cueName);
            return cue != null && cue.IsPlaying;
        }

        public bool IsPlayingAnyCue()
        {
            return PlayedCues.Any(c => c.IsPlaying);
        }

        private Cue AddOrReplaceCue(Cue cue)
        {
            for (int i = 0; i < PlayedCues.Count; i++)
            {
                var current = PlayedCues[i];
                if (current.Name == cue.Name)
                {
                    // Replace existing cue
                    cue = Sounds.GetCue(cue.Name);
                    PlayedCues[i] = cue;
                    return cue;
                }
            }

            PlayedCues.Add(cue);
            return cue;
        }
    }
}
