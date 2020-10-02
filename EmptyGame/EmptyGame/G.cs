using System;
using System.IO;
using System.Text;
using JuliHelper;
using JuliHelper.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EmptyGame
{
    public static class G
    {
        public static Random graphicsRand;
        public static Random rand;

        public static GraphicsDevice gDevice;
        public static SpriteBatch batch;

        public static Int2 res;
        public static Vector2 resV; // res Vector
        public static Vector2 res2; // res Halved

        public static bool replayRun; // if set to true once, it stays that way until game is restartet (prevents save file from being overridden)

        public static void Initialize(Int2 _res, GraphicsDevice graphicsDevice)
        {
            gDevice = graphicsDevice;
            res = _res;
            resV = _res.ToVector2();
            res2 = resV / 2f;

            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-us");

            L.Initialize();

            DepthManager.Initialize(typeof(Depth), typeof(DepthScreen));

            graphicsRand = new Random();
        }

        public static void Restart(int seed)
        {
            rand = new Random(seed);
            Console.WriteLine("seed:" + seed);
        }

        public static string GetNameFromType(Type type)
        {
            string name = type.Name;
            name = name.Substring(name.LastIndexOf("_") + 1);
            StringBuilder str = new StringBuilder(name);
            StringBuilder str2 = new StringBuilder(name);
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (char.IsUpper(str[i]))
                {
                    str[i] = char.ToLower(str[i]);
                    if (i > 0)
                    {
                        if (char.IsLower(str[i - 1]))
                        {
                            str.Insert(i, "-");
                            str2.Insert(i, " ");
                        }
                    }
                }
            }
            return str2.ToString();
        }
        public static string GetTextureStringFromType(Type type)
        {
            string name = type.Name;
            name = name.Substring(name.LastIndexOf("_") + 1);
            return GetTextureStringFromString(name);
        }
        public static string GetTextureStringFromString(string name)
        {
            StringBuilder str = new StringBuilder(name);
            StringBuilder str2 = new StringBuilder(name);
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (char.IsUpper(str[i]) || char.IsNumber(str[i]))
                {
                    str[i] = char.ToLower(str[i]);
                    if (i > 0)
                    {
                        if (char.IsLower(str[i - 1]) || char.IsNumber(str[i - 1]))
                        {
                            str.Insert(i, "-");
                            str2.Insert(i, " ");
                        }
                    }
                }
            }
            return str.ToString();
        }
    }
}
