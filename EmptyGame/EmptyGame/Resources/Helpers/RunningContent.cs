using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JuliHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EmptyGame
{
    class RunningContent
    {
        public static string modPath, texturePath, soundPath;

        private static DateTime lastLoaded;

        static void Initialize()
        {
            string path = JuliHelper.G.exeDir;
            bool noModPath = false;
            for (int i = 0; i < 5; i++)
            {
                int index = path.LastIndexOf(Path.DirectorySeparatorChar);
                if (index == -1)
                {
                    noModPath = true;
                    break;
                }
                path = path.Remove(index);
            }

            if (noModPath)
            {
                path = JuliHelper.G.exeDir;
            }
            modPath = Path.Combine(path, GameIdentity.ProjectName, "Content");
            texturePath = Path.Combine(modPath, "Textures");
            soundPath = Path.Combine(modPath, "Sounds");
        }

        public static void LoadAll(GraphicsDevice gDevice)
        {
            Initialize();

#if DEBUG

            lastLoaded = DateTime.Now;

            if (Directory.Exists(modPath))
            {
                string[] files;
                
                if (Directory.Exists(texturePath))
                {
                    files = Directory.GetFiles(texturePath, "*.png", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        //string name = file.Substring(file.LastIndexOf('\\') + 1).ToLower();
                        //name = name.Remove(name.Length - 4);
                        string name = file.Substring(texturePath.Length + 1, file.Length - texturePath.Length - 5);
                        name = name.Replace('\\', '/');
                        if (!ContentLoader.textures.ContainsKey(name))
                        {
                            //try
                            //{
                            using (FileStream stream = new FileStream(file, FileMode.Open))
                            {
                                Texture2D modTex = Texture2D.FromStream(gDevice, stream);
                                ContentLoader.textures.Add(name, modTex);
                                modTex.Name = name;
                            }
                            //} catch (Exception e) { Program.LogError(e); }
                        }
                        else
                        {
                            //throw new Exception("png found with duplicate name!");
                        }
                    }
                }

                if (Directory.Exists(soundPath))
                {
                //    files = Directory.GetFiles(soundPath, "*.wav", SearchOption.AllDirectories);
                //    foreach (string file in files)
                //    {
                //        try
                //        {
                //            //string name = file.Substring(file.LastIndexOf('\\') + 1);
                //            //name = name.Remove(name.Length - 4);

                //            string name = file.Substring(soundPath.Length, file.Length - soundPath.Length - 4);
                //            name = name.Replace('\\', '/');

                //            if (!ContentLoader.sounds.ContainsKey(name))
                //            {
                //                using (FileStream stream = new FileStream(file, FileMode.Open))
                //                    ContentLoader.sounds.Add(name, SoundEffect.FromStream(stream));
                //            }
                //        } catch (Exception e) { Program.LogError(e); }
                //    }

                }

            }
#endif

            OnReloadTextures(gDevice);
            OnReloadFonts();
            OnReloadSounds();
        }

        private static void OnReloadTextures(GraphicsDevice gDevice)
        {
            Tex.Initialize(gDevice, ContentLoader.content);
        }

        private static void OnReloadFonts()
        {
            Font.Initialize();
        }


        private static Texture2D ResizeCanvas(Texture2D tex, int w, int h)
        {
            Color[] color1 = tex.ToColor();
            Color[] color2 = new Color[w * h];

            int yEnd = Math.Min(h, tex.Height);
            int xEnd = Math.Min(w, tex.Width);

            for (int y = 0; y < yEnd; y++)
            {
                for (int x = 0; x < xEnd; x++)
                {
                    color2[y * w + x] = color1[y * tex.Width + x];
                }
            }

            return color2.ToTexture(w, tex.GraphicsDevice);
        }

        private static Texture2D FlipDiagonalThenHorizontal(Texture2D tex)
        {
            tex = ResizeCanvas(tex, 960, 540);

            Color[] colors = tex.ToColor();

            int wHalf = tex.Width / 2;

            for (int y = 0; y < tex.Height; y++)
            {
                for (int x = 0; x < wHalf; x++)
                {
                    Set(wHalf + x, y, wHalf - x - 1, y);
                }
            }

            for (int y = 1; y < tex.Height; y++)
            {
                for (int x = 0; x < y; x++)
                {
                    Set(x, y, y, x);
                }
            }

            for (int y = 1; y < tex.Height; y++)
            {
                for (int x = 0; x < y; x++)
                {
                    Set(tex.Width - 1 - x, y, (tex.Width - 1 - y), x);
                }
            }

            void Set(int x, int y, int x2, int y2)
            {
                int i1 = y * tex.Width + x;
                if (colors[i1].A == 0)
                {
                    int i2 = y2 * tex.Width + x2;
                    colors[i1] = colors[i2];
                }
            }

            tex.SetData(colors);
            return tex;
            //return colors.ToTexture(tex.Width, tex.GraphicsDevice);
        }

        private static Texture2D FlipHorizontal(Texture2D tex)
        {
            Color[] colors = tex.ToColor();

            int wHalf = tex.Width / 2;

            for (int y = 0; y < tex.Height; y++)
            {
                for (int x = 0; x < wHalf; x++)
                {
                    Set(wHalf + x, y, wHalf - x - 1, y);
                }
            }

            void Set(int x, int y, int x2, int y2)
            {
                int i1 = y * tex.Width + x;
                if (colors[i1].A == 0)
                {
                    int i2 = y2 * tex.Width + x2;
                    colors[i1] = colors[i2];
                }
            }
            
            return colors.ToTexture(tex.Width, tex.GraphicsDevice);
        }

        private static void OnReloadSounds()
        {
            string soundSettingsPath;
#if DEBUG
            soundSettingsPath = Path.Combine(RunningContent.soundPath, "settings.txt");
#else
            soundSettingsPath = Path.Combine(Paths.exeDir, "Content", "Sounds", "settings.txt");
#endif
            Sounds.Initialize(soundSettingsPath);
        }

        private static void AddOrReplace(string key, Texture2D tex)
        {
            if (ContentLoader.textures.ContainsKey(key))
            {
                ContentLoader.textures[key].Dispose();
                ContentLoader.textures[key] = tex;
            }
            else
                ContentLoader.textures.Add(key, tex);
        }

        public static void Update(GraphicsDevice gDevice)
        {
#if DEBUG
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            if (Directory.Exists(modPath))
            {
                string[] files;

                if (Directory.Exists(texturePath))
                {
                    files = GetChangedFiles(lastLoaded, texturePath, "*.png", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        string name = file.Substring(texturePath.Length + 1, file.Length - texturePath.Length - 5);
                        name = name.Replace('\\', '/');
                        if (ContentLoader.textures.ContainsKey(name))
                        {
                            LoadTexture(file, name, gDevice);
                        }
                    }

                    OnReloadTextures(gDevice);
                }

                if (Directory.Exists(soundPath))
                {
                    //ContentLoader.sounds.Clear();
                    //files = Directory.GetFiles(modPath + "\\sounds\\", "*.wav", SearchOption.AllDirectories);
                    //foreach (string file in files)
                    //{
                    //    //try
                    //    //{
                    //        string name = file.Substring(file.LastIndexOf('\\') + 1);
                    //        name = name.Remove(name.Length - 4);
                    //        //if (ContentLoader.sounds.ContainsKey(name))
                    //        {
                    //            using (FileStream stream = new FileStream(file, FileMode.Open))
                    //                ContentLoader.sounds.Add(name, SoundEffect.FromStream(stream));
                    //        }
                    //    //} catch (Exception e) { Program.LogError(e); }
                    //}

                    //OnReloadSounds();
                }
            }
            sw.Stop();
            Console.WriteLine("running content update ms: " + sw.ElapsedMilliseconds);

            lastLoaded = DateTime.Now;
#endif
        }

        private static string[] GetChangedFiles(DateTime lastLoad, string texturePath, string v, SearchOption allDirectories)
        {
            string[] files = Directory.GetFiles(texturePath, "*.png", SearchOption.AllDirectories);
            List<string> changed = new List<string>();
            foreach (string f in files)
            {
                FileInfo info = new FileInfo(f);
                if (info.LastWriteTime > lastLoad)
                    changed.Add(f);
            }
            return changed.ToArray();
        }

        public static void LoadTexture(string file, string name, GraphicsDevice gDevice)
        {
            try
            {
                using (FileStream stream = new FileStream(file, FileMode.Open))
                {
                    Texture2D modTex = Texture2D.FromStream(gDevice, stream);
                    //if (ContentLoader.textures[name].Width == modTex.Width && ContentLoader.textures[name].Height == modTex.Height)
                    //ContentLoader.textures[name] = modTex;
                    Color[] data = new Color[modTex.Width * modTex.Height];
                    modTex.GetData(data);
                    ContentLoader.textures[name].SetData(data);
                }
            } catch (Exception e) { Program.LogError(e); }
        }
    }
}

