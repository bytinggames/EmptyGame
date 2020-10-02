using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyGame
{
    public static class Paths
    {
        public static readonly string exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public static readonly string data = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GameIdentity.ProjectName);
        public static readonly string error_log = Path.Combine(data, "error.log");
        public static readonly string screenshots = Path.Combine(data, "screenshots");
        public static readonly string screenshotsRandom = Path.Combine(data, "screenshots-random");
        public static readonly string input = Path.Combine(data, "input-recordings");
        public static readonly string settings_txt = Path.Combine(data, "settings.txt");
        public static readonly string save_bin = Path.Combine(data, "save1.bin");
    }
}
