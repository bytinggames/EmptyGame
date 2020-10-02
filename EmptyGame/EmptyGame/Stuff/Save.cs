using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyGame
{
    public static class Save
    {
        const int version = 1;

        public enum Code : byte
        {
            End = 1,
            SaveVersion = 2,
        }

        public static void Initialize()
        {
            LoadData();
        }

        private static void SaveData()
        {
            using (Stream s = File.Create(Paths.save_bin))
            using (BinaryWriter bw = new BinaryWriter(s))
            {
                SaveTo(bw);
            }
        }

        public static void SaveTo(BinaryWriter bw)
        {
            bw.Write((byte)Code.SaveVersion);
            bw.Write(version);

            bw.Write((byte)Code.End);
        }

        private static void LoadData()
        {
            if (File.Exists(Paths.save_bin))
            {
                using (Stream s = File.OpenRead(Paths.save_bin))
                using (BinaryReader br = new BinaryReader(s))
                {
                    ReadFrom(br);
                }
            }
            else
            {
                // no save init
            }
        }

        public static void ReadFrom(BinaryReader br)
        {
            Code code;
            while ((code = (Code)br.ReadByte()) != Code.End)
            {
                switch (code)
                {
                    case Code.SaveVersion:
                        {
                            int v = br.ReadInt32();
                            if (v != version)
                                throw new Exception();
                        }
                        break;
                    default:
                        throw new Exception("unknown code in savestate");
                }
            }
        }
    }
}
