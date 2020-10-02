using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;
using Microsoft.Xna.Framework.Audio;

namespace EmptyGame
{
    public static class Sounds
    {
        public static SoundItem knock, knock2, knock3;

        public static SoundItem clickButton => knock;

        public abstract class SoundItem
        {
            public int lastPlayedFrame = -1;

            public float volume = 0.5f;
            public float pitch;
            public float pan;
            
            public void Play(int cooldownFrames = 1)
            {
                if (Game1.updateFrame - lastPlayedFrame > cooldownFrames)
                {
                    PlayChild(volume, pitch, pan);
                    lastPlayedFrame = Game1.updateFrame;
                }
            }
            public void PlayFromContainer(float volume, float pitch, float pan)
            {
                PlayChild(volume, pitch, pan);
            }

            public void Reset()
            {
                volume = 0.5f;
                pitch = 0f;
                pan = 0f;
            }

            public abstract void PlayChild(float volume, float pitch, float pan);
        }
        public class Sound : SoundItem
        {
            public SoundEffect soundEffect;
            //public Sound(SoundEffect _soundEffect, float _volume, float _pan, float _pitch)
            //{
            //    soundEffect = _soundEffect;
            //}

            public override void PlayChild(float volume, float pitch, float pan)
            {
                if (soundEffect != null)
                {
                    soundEffect.PlayMe(volume, pitch, pan);

                    //Console.WriteLine("Sound played: " + this.soundEffect.Name);
                }
            }
        }

        public class SoundContainer : SoundItem
        {
            List<SoundItem> waiting = new List<SoundItem>();
            public int preventPlayingSameSound = 1;
            public List<SoundItem> sounds = new List<SoundItem>();

            public override void PlayChild(float volume, float pitch, float pan)
            {
                //int index = G.graphicsRand.Next(sounds.Count);
                int index = 0;
                sounds[index].PlayFromContainer(volume, pitch, pan);
                sounds.Add(sounds[index]);
                sounds.RemoveAt(index);
                //if (preventPlayingSameSound > 0)
                //{
                //    waiting.Add(sounds[index]);
                //    sounds.RemoveAt(index);

                //    if (waiting.Count > preventPlayingSameSound || sounds.Count <= 0)
                //    {
                //        sounds.Add(waiting[0]);
                //    }
                //}
            }
        }

        //static string noneSfx = "_";

        static Dictionary<string, SoundItem> soundDictionary = new Dictionary<string, SoundItem>();

        public static void RestartFrames()
        {
            foreach (var item in soundDictionary)
            {
                item.Value.lastPlayedFrame = -1;
            }
        }

        public static void Initialize()
        {
            soundDictionary = new Dictionary<string, SoundItem>();
            string[] sounds = ContentLoader.sounds.Keys.ToArray();
            foreach (string sound in sounds)
            {
                int slashIndex = sound.IndexOf('/');
                if (slashIndex != -1)
                {
                    // folder

                    string soundName = sound.Remove(slashIndex);
                    if (!soundDictionary.ContainsKey(soundName))
                        soundDictionary.Add(soundName, new SoundContainer());

                    SoundContainer container = soundDictionary[soundName] as SoundContainer;
                    container.sounds.Add(new Sound()
                    {
                        soundEffect = ContentLoader.sounds[sound]
                    });
                }
                else
                {
                    soundDictionary.Add(sound, new Sound()
                    {
                        soundEffect = ContentLoader.sounds[sound]
                    });
                }
                
            }

            //soundDictionary = new Dictionary<string, SoundItem>();
            //string soundPath = RunningContent.soundPath;
            //string[] dirs = Directory.GetDirectories(soundPath);
            //string[] files;
            //foreach (string dir in dirs)
            //{
            //    string name = dir.Substring(soundPath.Length, dir.Length - soundPath.Length);
            //    //name = name.Replace('\\', '/');

            //    SoundContainer container = new SoundContainer();
            //    files = Directory.GetFiles(dir, "*.wav", SearchOption.TopDirectoryOnly);
            //    foreach (string file in files)
            //    {
            //        container.sounds.Add(new Sound()
            //        {
            //            soundEffect = GetSoundEffect(file)
            //        });
            //    }

            //    soundDictionary.Add(name, container);
            //}

            //files = Directory.GetFiles(soundPath, "*.wav", SearchOption.TopDirectoryOnly);
            //foreach (string file in files)
            //{
            //    string name = file.Substring(soundPath.Length, file.Length - soundPath.Length - 4);
            //    if (!soundDictionary.ContainsKey(name))
            //    {
            //        soundDictionary.Add(name, new Sound()
            //        {
            //            soundEffect = GetSoundEffect(file)
            //        });
            //    }
            //}

            //SoundEffect GetSoundEffect(string file)
            //{
            //    using (FileStream stream = new FileStream(file, FileMode.Open))
            //        return SoundEffect.FromStream(stream);
            //}


            var fields = typeof(Sounds).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            // RELEASE: replace this with hard code
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType == typeof(SoundItem))
                {

                    string name = fields[i].Name;
                    if (soundDictionary.ContainsKey(name))
                    {
                        fields[i].SetValue(null, soundDictionary[name]);
                    }
                    else
                        fields[i].SetValue(null, new Sound());
                }
            }

            ReloadSettings();

            RestartFrames();
        }

        public static void ReloadSettings()
        {
            if (File.Exists(RunningContent.soundSettingsPath))
            {
                string[] keys = soundDictionary.Keys.ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    soundDictionary[keys[i]].Reset();
                }

                string[] lines = File.ReadAllLines(RunningContent.soundSettingsPath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("//"))
                        continue;

                    string[] split = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length > 1)
                    {
                        if (soundDictionary.ContainsKey(split[0]))
                        {
                            SoundItem sound = soundDictionary[split[0]];
                            for (int j = 1; j < split.Length; j++)
                            {
                                char c = split[j][0];
                                string rest = split[j].Substring(1);
                                float f;
                                switch (c)
                                {
                                    case 'p':
                                        if (float.TryParse(rest, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out f))
                                            sound.pitch = f * 2f - 1f;
                                        break;
                                    case 'v':
                                        if (float.TryParse(rest, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out f))
                                            sound.volume = f;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public static void PlayMe(this SoundEffect sound, float _volume, float _pitch, float _pan)
        {
            sound.Play(Rules.volume * _volume, _pitch/*(float)G.graphicsRand.NextDouble() * 2f - 1f*/, _pan);
        }

        //private static void PlayMe(this SoundEffect sound, float _volume, float _pitch, float _pan, int cooldownFrames)
        //{
        //    if (cooldownFrames <= 0)
        //    {
        //        PlayReal(sound, _volume, _pitch, _pan);
        //        return;
        //    }

        //    Cooldown c = cooldowns.Find(f => f.soundEffect == sound);

        //    if (c != null)
        //    {
        //        if (c.frameStarted + cooldownFrames <= ThreadGame.frame)
        //        {
        //            cooldowns.Remove(c);
        //            c = null;
        //        }
        //    }

        //    if (c == null)
        //    {
        //        cooldowns.Add(new Cooldown()
        //        {
        //            soundEffect = sound,
        //            frameStarted = ThreadGame.frame
        //        });

        //        PlayReal(sound, _volume, _pitch, _pan);
        //    }
        //}
    }
}
