using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace EmptyGame
{
#if WINDOWS || LINUX

    public static class Program
    {
        static Game1 game;

        static bool runConsoleLoop = false;
        
        [STAThread]
        static void Main()
        {
            //EncounterGenerator.GetDistribution(10);
            //return;

            Task t = null;
            if (runConsoleLoop)
            {
                t = new Task(ConsoleLoop);
                t.Start();
            }

            using (game = new Game1())
            {
#if DEBUG
                game.Run();
#else
                try
                {
                    game.Run();
                } catch (Exception e)
                {
                    StackTrace stackTrace = new StackTrace(e, true);
                    string frame = stackTrace.GetFrame(0).ToString();

                    LogError(e);

#if WINDOWS
                    System.Windows.Forms.MessageBox.Show("Message: " + e.Message +
                        (e.InnerException != null ? ("\nInnerException: " + e.InnerException) : "") +
                        "\nSource: " + e.Source +
                        (frame != "" ? "\nStackFrame:" + frame : "") +
                        "\nStackTrace: " + e.StackTrace, "Crash");
#endif
                }
#endif
                }

            if (runConsoleLoop)
            {
                runConsoleLoop = false;
                // Send Enter to application, to interrupt Console.ReadLine() in ConsoleLoop()
                PostMessage(Process.GetCurrentProcess().MainWindowHandle, WM_KEYDOWN, VK_RETURN, 0);

                t.Wait();
            }
        }

        static void ConsoleLoop()
        {
            while (runConsoleLoop)
            {
                string cmd = Console.ReadLine();
                if (runConsoleLoop)
                    game.GetCMD(cmd);
            }
        }

        public static void LogError(Exception e)
        {
            string dir = Path.GetDirectoryName(Paths.error_log);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
			
            File.AppendAllText(Paths.error_log, "\n\n" + DateTime.Now + ":\nMessage: " + e.Message + "\n" + (e.InnerException != null ? ("InnerException: " + e.InnerException + "\n") : "") + "Source: " + e.Source + "\nStackTrace: " + e.StackTrace);
        }

        public static void Log(string text)
        {
            string dir = Path.GetDirectoryName(Paths.error_log);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.AppendAllText(Paths.error_log, "\nLOG: " + text);
        }

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("User32.Dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int VK_RETURN = 0x0D;
        const int WM_KEYDOWN = 0x100;
    }
#endif
        }