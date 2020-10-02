using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Reflection;
using EmptyGame.Shared;

namespace EmptyGame
{

    public class Game1 : Game
    {
        const bool UPDATERUNNINGCONTENT = true;

        public static Game1 I;

        static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<string> cmds = new List<string>();

        Ingame ingame;

        #region window

        public static GameWindow window;
        static bool changeResolution = false;
        public static bool isFullScreen;

        private static int windowResX, windowResY;

        public static int resX
        {
            get
            { return graphics.PreferredBackBufferWidth; }
        }
        public static int resY
        {
            get
            { return graphics.PreferredBackBufferHeight; }
        }

        #endregion

        public static RenderTarget2D renderTarget;

        public static int samplingZoom = 1;

        public static InputRecorderManager inputRecorderManager;


        public static List<Action<Game1>> doInThisThread;

        public static double updatesPerFrame = 1;

        public Game1()
            : base(true)
        {
            if (I != null)
                throw new Exception();

            I = this;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            window = Window;

#if DEBUG
            IsFixedTimeStep = false;
#else
            IsFixedTimeStep = true;
#endif
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        int inited = -2;

        void InitializeLater()
        {
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new System.EventHandler<System.EventArgs>(Window_ClientSizeChanged);

            renderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080);

            windowResX = renderTarget.Width / 2;
            windowResY = renderTarget.Height / 2;

            ContentLoader.Initialize(Content, GraphicsDevice, new Random());
            DrawM.Initialize(GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            G.batch = spriteBatch;
            Drawer.Initialize(spriteBatch);
            Input.Initialize();
            Rules.Initialize();
            Save.Initialize();
            RunningContent.LoadAll(GraphicsDevice);


            G.Initialize(new Int2(renderTarget.Width / samplingZoom, renderTarget.Height / samplingZoom), GraphicsDevice);


            //InitPatcher();

            inputRecorderManager = new InputRecorderManager();

            Restart(null);

            ToggleFullscreen();

        }

        public void Restart(Action _playRecord)
        {
            doInThisThread = new List<Action<Game1>>();

            ingame?.Dispose();

            _playRecord?.Invoke();

            int seed = Rules.seed ?? new Random().Next(); //1707520875
            if (!inputRecorderManager.playNow)
            {
                inputRecorderManager.Restart(seed);
            }
            else
            {
                inputRecorderManager.playNow = false;
                inputRecorderManager.playing = true;
                if (inputRecorderManager.seed.HasValue)
                    seed = inputRecorderManager.seed.Value;
            }


            G.Restart(seed);

            ingame = new Ingame();
        }

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
            ingame.Dispose();

            inputRecorderManager?.Stop();
        }

        bool updateGame = true;
        bool isApplicationActive;
        
        public static bool restart = false;

        public static bool exit = false;
        protected override void Update(GameTime gameTime)
        {
            updateFps.NewFrame();

            if (inited < 0)
            {
                inited++;
                if (inited == 0)
                {
                    InitializeLater();
                }
                return;
            }

            Input.Update();

            bool newIsApplicationActive = ApplicationIsActivated();
            if (isApplicationActive != newIsApplicationActive)
            {
                isApplicationActive = newIsApplicationActive;
                if (isApplicationActive)
                    frameFps.Continue();
                else
                    frameFps.Pause();
            }


            if (updateGame)
            {
                bool doUpdate = true;
#if DEBUG
                if (Input.up.pressed)
                {
                    updatesPerFrame *= 2;
                }
                else if (Input.down.pressed)
                {
                    updatesPerFrame /= 2d;
                }
                if (updatesPerFrame > 0.75d && updatesPerFrame < 1.5d)
                {
                    updatesPerFrame = 1d;
                }

                double cUpdate = updatesPerFrame;
                if (Input.leftShift.down)
                {
                    cUpdate *= 10d;

                    if (Input.leftControl.down)
                        cUpdate *= 10d;
                }

                if (Input.rightControl.down)
                    doUpdate = false;

                if (Input.leftAlt.down)
                {
                    cUpdate *= 0.2d;

                    if (Input.leftControl.down)
                        cUpdate *= 0.2d;
                }

                InceptionUpdatesPerFrame = cUpdate;
#endif

                //if (iterations > 0 && inputRecorderManager.playing)
                //    iterations = 100;

                if (doUpdate)
                {
#region adapt mouse pos

                    float scale = CalculateRenderTargetScaling();

                    Vector2 rtPos = CalculateRenderTargetPos(scale);

                    Input.mbPos -= rtPos;

                    if (scale != 1)
                        Input.mbPos /= scale;

                    Input.mbPos /= samplingZoom;

#endregion



#if DEBUG
                    if (Input.r.released && !inputRecorderManager.playing)
                    {
                        restart = true;
                    }
                    if (Input.esc.pressed)
                    {
                        exit = true;
                    }
#endif

                    if (restart)
                    {
                        restart = false;
                        Restart(null);
                    }

                    if (exit)
                    {
                        exit = false;
                        ExitToSurvey();
                    }

#if DEBUG
                    if (Input.esc.pressed && Input.leftShift.down)
                    {
                        if (!inputRecorderManager.playing)
                            Exit();
                        return;
                    }
#endif
                    if (Input.f11.pressed)
                    {
                        ToggleFullscreen();
                    }

#if DEBUG
                    if (Input.right.pressed)
                    {
                        SwapScreen(true);
                    }
                    if (Input.left.pressed)
                    {
                        SwapScreen(false);
                    }
#endif

                    if (Input.leftControl.down && Input.p.pressed)
                    {
                        patcher.StartDownload();
                    }


                    //if (Input.tab.pressed)
                    //{
                    //    Program.ShowWindow(window.Handle, 6);
                    //}

                    if (cmds.Count > 0)
                    {
                        while (cmds.Count > 0)
                        {
                            string cmd = cmds[cmds.Count - 1];
                            cmds.RemoveAt(cmds.Count - 1);


                            // execute cmd
                            string[] cmdSplit = cmd.Split(new char[] { ' ' });
                            string cmd0 = cmdSplit[0];
                            switch (cmd0)
                            {
                                case "exit":
                                    Exit();
                                    break;
                            }
                        }

                        //Program.ShowWindow(window.Handle, 9);
                    }

                    Action playRecord;
                    if ((playRecord = inputRecorderManager.Update()) != null)
                    {
                        Restart(playRecord);
                        return;
                    }

                    ingame.Update();

                    if (setTitleAction != null)
                    {
                        setTitleAction();
                        setTitleAction = null;
                    }


                    while (doInThisThread.Count > 0)
                    {
                        doInThisThread[0].Invoke(this);
                        if (doInThisThread.Count > 0)
                        {
                            doInThisThread.RemoveAt(0);
                        }
                    }
                }
            }


            if (isApplicationActive != updateGame)
            {
                if (isApplicationActive)
                {
                    if (Input.mbLeft.none || inputRecorderManager.playing)
                    {
                        updateGame = true;
                        if (UPDATERUNNINGCONTENT)
                        {
                            RunningContent.Update(GraphicsDevice);
                        }
                    }
                }
                else
                {
                    updateGame = false;
                }
            }

            updateFrame++;

            base.Update(gameTime);
        }

        public static FPS updateFps = new FPS(), frameFps = new FPS();

        bool firstDraw = true;

        public static int updateFrame;

        protected override void Draw(GameTime gameTime)
        {
            if (firstDraw)
            {
                firstDraw = false;
                GraphicsDevice.Clear(Color.Black);
            }

            if (inited < 0)
                return;

            if (isApplicationActive)
            {
                GraphicsDevice.SetRenderTarget(renderTarget);

                GraphicsDevice.Clear(Color.Black);
                ingame.Draw();

                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);

                if (Input.f12.down && !inputRecorderManager.playing)
                {
                    // interrupt drawing, to give the user feedback, that he made a screenshot
                    if (Input.f12.pressed)
                        Screenshot(Input.f12.pressed);
                }
                else if (Input.leftControl.down && Input.c.down && !inputRecorderManager.playing)
                {
                    if (Input.c.pressed)
                    {
                        CopyScreenshot();
                    }
                }
                else
                {
                    float scale = CalculateRenderTargetScaling();

                    spriteBatch.Begin(SpriteSortMode.Deferred, null, scale % 1 == 0 ? SamplerState.PointClamp : SamplerState.LinearClamp);

                    spriteBatch.Draw(renderTarget, CalculateRenderTargetPos(scale), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                    frameFps.NewFrame();

#if DEBUG
                    //if (frameFps.CurrentFPS < 50)
                    //    Font.small.Draw("fps: " + frameFps.CurrentFPS, Anchor.TopLeft(0, 0));
                    //if (updateFps.CurrentFPS < 50)
                    //    Font.small.Draw("ups: " + updateFps.CurrentFPS, Anchor.TopLeft(0, 32));
#endif

                    spriteBatch.End();
                }

                if (Input.mbPos != Input.mbPosPast && !inputRecorderManager.playing)
                {
                    if (Rules.shootRandomScreenshots)
                    {
                        if (G.graphicsRand.Next(60 * 60) == 0) // take random pic around every minute of mouse movement, just for fun^^
                            Screenshot(false);
                    }
                }

            }
            base.Draw(gameTime);
        }

        private void Screenshot(bool shotByUser)
        {
            string path = shotByUser ? Paths.screenshots : Paths.screenshotsRandom;

            // screenshot
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            using (FileStream fs = new FileStream(Path.Combine(path, "screenie_" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss.fff") + ".png"), FileMode.Create))
            {
                renderTarget.SaveAsPng(fs, renderTarget.Width, renderTarget.Height);

            }
        }

        private void CopyScreenshot()
        {
#if WINDOWS
            using (MemoryStream ms = new MemoryStream())
            {
                renderTarget.SaveAsPng(ms, renderTarget.Width, renderTarget.Height);

                using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
                {
                    System.Windows.Forms.Clipboard.SetImage(img);
                }
            }
#endif
        }

        private Vector2 CalculateRenderTargetPos(float scale)
        {
            return (graphics.PreferredBackBufferSize() / 2f - renderTarget.GetSize() / 2f * scale).RoundVector();
        }

        private float CalculateRenderTargetScaling()
        {
            //Window
            float scale = Math.Min((float)graphics.PreferredBackBufferWidth / renderTarget.Width, (float)graphics.PreferredBackBufferHeight / renderTarget.Height);
            if (scale > 1)
            {
                if (scale >= 1.5f && scale < 2f)
                    scale = 1.5f;
                else
                    scale = (float)Math.Floor(scale);
            }
            else if (scale < 1)
            {
                if (scale > 0.5)
                    scale = 0.5f;
            }
            return scale;
        }

        public void GetCMD(string cmd)
        {
            lock (cmds)
            {
                cmds.Add(cmd);
            }
        }


        private void ExitToSurvey()
        {
            if (!inputRecorderManager.playing)
                Exit();
        }



        public void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (!changeResolution)
            {
                changeResolution = true;

                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphics.ApplyChanges();

                if (!isFullScreen)
                {
                    windowResX = resX;
                    windowResY = resY;
                }

                changeResolution = false;
            }

            DrawM.Initialize(graphics.GraphicsDevice);
        }
        public void ToggleFullscreen()
        {
            isFullScreen = !isFullScreen;

            changeResolution = true;

            if (isFullScreen)
            {

                Rectangle screenRect = GetCurrentScreenRectangle();

                window.Position = screenRect.Location;
                //window.Position = new Point((((int)window.Position.X + window.ClientBounds.Width / 2) / 1920) * 1920, 0);

                Thread.Sleep(50); // testing (maybe prevent fullscreen not working??)

                graphics.PreferredBackBufferWidth = screenRect.Width;//GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;//System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                graphics.PreferredBackBufferHeight = screenRect.Height;// GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;//System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                graphics.ApplyChanges();
#if DIRECTX
                window.IsBorderless = true;
#else
                graphics.ToggleFullScreen();
#endif
            }
            else
            {
#if DIRECTX
                window.IsBorderless = false;
#else
                graphics.ToggleFullScreen();
#endif
                graphics.PreferredBackBufferWidth = windowResX;
                graphics.PreferredBackBufferHeight = windowResY;
                graphics.ApplyChanges();

                CenterWindow();
            }

            changeResolution = false;

            DrawM.ResizeWindow();
        }
        public static void ToggleFullscreenStatic()
        {
            doInThisThread.Add(g => g.ToggleFullscreen());
        }

        private void SwapScreen(bool toNext)
        {
            if (!isFullScreen)
                return;

#if WINDOWS

            var screen = GetCurrentScreen();
            var allScreens = System.Windows.Forms.Screen.AllScreens;

            if (allScreens.Length == 0)
                return;

            int index = Array.IndexOf(allScreens, screen);
            index += toNext ? 1 : -1;
            if (index >= allScreens.Length)
                index = 0;
            else if (index < 0)
                index = allScreens.Length - 1;

            ToggleFullscreen();

            CenterWindowToScreen(allScreens[index]);

            ToggleFullscreen();
            
#endif
        }

        public static void CenterWindow()
        {
            if (!isFullScreen)
            {
                Rectangle screenRect = GetCurrentScreenRectangle();

                window.Position = screenRect.Location + new Point((screenRect.Size.X - graphics.PreferredBackBufferWidth) / 2, (screenRect.Size.Y - graphics.PreferredBackBufferHeight - 80) / 2);

                //window.Position = new Point(window.Position.X + (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - graphics.PreferredBackBufferWidth) / 2, (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 80 - graphics.PreferredBackBufferHeight) / 2);
            }
        }

        private static Rectangle GetCurrentScreenRectangle()
        {
#if WINDOWS
            var screen = GetCurrentScreen();
            return new Rectangle(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);
#else

            return new Rectangle(0, 0, 1920, 1080); // TODO
#endif

        }

#if WINDOWS
        private static System.Windows.Forms.Screen GetCurrentScreen()
        {
            return System.Windows.Forms.Screen.FromRectangle(new System.Drawing.Rectangle(window.ClientBounds.X, window.ClientBounds.Y, window.ClientBounds.Width, window.ClientBounds.Height));
        }

        private static void CenterWindowToScreen(System.Windows.Forms.Screen screen)
        {
            if (!isFullScreen)
            {
                Rectangle screenRect = new Rectangle(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);

                window.Position = screenRect.Location + new Point((screenRect.Size.X - graphics.PreferredBackBufferWidth) / 2, (screenRect.Size.Y - graphics.PreferredBackBufferHeight - 80) / 2);
            }
        }
#endif


#if WINDOWS
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
#endif



        public static bool ApplicationIsActivated()
        {

#if WINDOWS
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = System.Diagnostics.Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
#else
            return true;
#endif
        }

#if WINDOWS
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
#endif





        ToBePatched patcher;
        Version version;
#if !DEBUG
        Thread patcherThread;
#endif

        Action setTitleAction;

        private void PatcherExitGameEvent()
        {
            Exit();
        }

        private void InitPatcher()
        {
#if WINDOWS
            version = Assembly.GetExecutingAssembly().GetName().Version;
            Window.Title = GameIdentity.ProjectName + " v" + version;
            string projectName = GameIdentity.ProjectName;
            string url = "https://bytinggames.com/patcher/";
            patcher = new ToBePatched(version, projectName, url, EmptyGame.DX.Properties.Resources.Patcher, "patched", "", true);
            patcher.evCheckedVersion += CheckedVersion;
            patcher.evClose += PatcherExitGameEvent;

#if !DEBUG
            patcherThread = new System.Threading.Thread(patcher.Start);
            patcherThread.Start();
#endif
#endif

        }
        private void CheckedVersion(bool newPatchAvailable)
        {
            if (newPatchAvailable)
            {
                setTitleAction = () => Window.Title += " - update available: v" + patcher.serverVersion;
#if WINDOWS // TODO
                if (System.Windows.Forms.MessageBox.Show("Do you want to update to v" + patcher.serverVersion + "?", "Update Available", System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                    patcher.StartDownload();
#endif
            }
            else
                setTitleAction = () => Window.Title += " - newest version";
        }

        public void PauseFastForwardOn(Func<bool> _predicate)
        {
            if (_predicate())
            {
                if (updatesPerFrame > 1)
                    updatesPerFrame = 1;
                if (InceptionUpdatesPerFrame > 1)
                    InceptionUpdatesPerFrame = 1;
            }
        }
        public void PauseFastForwardOn(int _frame) => PauseFastForwardOn(() => inputRecorderManager.frame == _frame);
    }
}
