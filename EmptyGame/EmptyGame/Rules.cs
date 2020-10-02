using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;

namespace EmptyGame
{
    public static class Rules
    {
        public static readonly int? seed = null;

        public static float volume = 0.5f;
        public static bool shootRandomScreenshots = true;

        public static void Initialize()
        {
        }

        static Rules()
        {
            string settingsPath = Paths.settings;

            if (File.Exists(settingsPath))
            {
                string[] lines = File.ReadAllLines(settingsPath);

                bool comment = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("/*"))
                    {
                        comment = true;
                        continue;
                    }

                    if (lines[i].StartsWith("*/"))
                    {
                        comment = false;
                        continue;
                    }

                    if (comment)
                        continue;

                    if (lines[i].StartsWith("//"))
                        continue;


                    string[] split = lines[i].Split(new char[] { '=' });
                    if (split.Length < 2)
                        continue;

                    var field = typeof(Rules).GetField(split[0]);
                    if (field == null)
                        throw new Exception("settings.txt: field not recognized: " + split[0]);

                    var converter = System.ComponentModel.TypeDescriptor.GetConverter(field.FieldType);
                    var result = converter.ConvertFrom(split[1]);
                    if (result != null)
                        field.SetValue(null, result);
                    else
                        throw new Exception();
                }
            }
        }

        internal static bool WPressed()
        {
#if DEBUG
            return Input.w.pressed;
#else
            return false;
#endif
        }
        internal static bool KPressed()
        {
#if DEBUG
            return Input.k.pressed;
#else
            return false;
#endif
        }

        internal static bool EnterPressed()
        {
#if DEBUG
            return Input.enter.pressed;
#else
            return false;
#endif
        }

        internal static bool LeftShiftDown()
        {
#if DEBUG
            return Input.leftShift.down;
#else
            return false;
#endif
        }
    }
}
