using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;

namespace EmptyGame
{
    public class InputRecorderManager
    {
        InputRecorder recorder;

        public bool playNow;
        public int? seed;
        public bool playing;

        public int frame => recorder.frame;

        //List<string> filesToPlay;
        
        public InputRecorderManager()
        {
        }

        public void Restart(int _seed)
        {
            Stop();
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            recorder = new InputRecorder(true, true, version, SaveSaveState, LoadSaveState);
            recorder.FinishPlaying += FinishPlaying;
            recorder.Record(GetNewFilePath(), _seed);
            playing = false;
        }

        private string GetNewFilePath() => Path.Combine(Paths.input, "input-" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss_fff") + ".inr");

        private void FinishPlaying(InputRecorder _recorder)
        {
            // swap from playing to recording

            string filePath = GetNewFilePath();
            File.Copy(recorder.GetFilePath(), filePath);
            recorder.RecordAfterPlay(filePath);
        }

        public Action Update()
        {
            if (recorder == null)
                return null;

            if (Input.leftControl.down)
            {
                if (Input.f5.released)
                {
                    string[] files = Directory.GetFiles(Paths.input, "*.inr");
                    if (files.Length > 1)
                    {
                        return () => Play(files[0]);
                    }
                }
                else if (Input.f4.released)
                {
                    //using (OpenFileDialog openFile = new OpenFileDialog())
                    //{
                    //    if (openFile.ShowDialog() == DialogResult.OK)
                    //    {
                    //        filesToPlay = openFile.FileNames.ToList();

                    //        return () => PlayFileToPlay();
                    //    }
                    //}
                }
                else if (Input.f6.released)
                {
#if WINDOWS
                    using (System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog())
                    {
                        open.InitialDirectory = Paths.input;
                        open.Filter = "inr files (*.inr)|*.inr|All files (*.*)|*.*";
                        System.Windows.Forms.DialogResult result = open.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            return () => Play(open.FileName);
                        }
                    }
#endif
                }
                else if (Input.f7.released)
                {
                    if (playing)
                    {
                        playing = false;
                        recorder.StopPlaying(true);
                    }
                }
            }

            recorder.Update();

            if (!recorder.playing)
                playing = false;

            return null;
        }

        private void Play(string file)
        {
            G.replayRun = true;

            seed = recorder.Play(file);
            playNow = true;

            Input.mbLeft.blocked = true;
            Input.enter.blocked = true;
            Input.mbRight.blocked = true;
        }

        //private void PlayFileToPlay()
        //{
        //    string file = filesToPlay[0];
        //    filesToPlay.RemoveAt(0);

        //    Play(file);
        //}

        ~InputRecorderManager()
        {
            //Console.WriteLine("dispose input recorder");
            Stop();
        }

        public void Stop()
        {
            recorder?.Dispose();
            recorder = null;
        }

        private void SaveSaveState(BinaryWriter writer)
        {
            Save.SaveTo(writer);
            
            bool settingsExists = File.Exists(Paths.settings_txt);
            writer.Write(settingsExists);

            if (settingsExists)
            {
                using (FileStream fs = File.OpenRead(Paths.settings_txt))
                {
                    writer.Write((long)fs.Length);
                    
                    while (fs.Position < fs.Length)
                    {
                        writer.Write((byte)fs.ReadByte());
                    }
                }
            }
        }

        private void LoadSaveState(BinaryReader reader)
        {
            Save.ReadFrom(reader);
            
            bool settingsExists = reader.ReadBoolean();

            if (settingsExists)
            {
                long size = reader.ReadInt64();

                for (long i = 0; i < size; i++)
                {
                    reader.ReadByte(); // TODO: save that (in ram) and load it into Rules.cs -> but for that I have to make the class non static
                }
            }
        }
    }
}
