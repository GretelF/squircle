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

        public AudioManager(Game game)
        {
            Game = game;
        }

        public void Initialize(ConfigSection section)
        {
            GlobalSettingsFileName = section["settings"];
            WaveBankFileName = section["waveBank"];
            SoundBankFileName = section["soundBank"];

            Game.EventSystem.getEvent("endLevel").addListener(OnEndLevel);
        }

        private void OnEndLevel(string data)
        {
            Sounds.Dispose();
            Waves.Dispose();
            Engine.Dispose();
        }

        public void LoadContent(ContentManager content)
        {
            if (GlobalSettingsFileName == null) return;

            Engine = new AudioEngine(GlobalSettingsFileName);
            Waves = new WaveBank(Engine, WaveBankFileName);
            Sounds = new SoundBank(Engine, SoundBankFileName);
        }

        public void Update(GameTime gameTime)
        {
            if (Engine == null) return;

            Engine.Update();
        }
    }
}
