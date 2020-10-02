using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JuliHelper;
using JuliHelper.Camera;
using JuliHelper.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EmptyGame
{
    public class Ingame
    {
        Camera camera;
        bool extended = false;

        public Ingame()
        {
            camera = new Camera()
            {
                middleMouseMoveControl = true,
                zoomControl = true,
                zoomStep = 2,
                targetZoom = 2,
            };
        }

        public void Update()
        {
            camera.UpdateBegin();

            if (Input.tab.pressed)
                extended = !extended;

            camera.UpdateEnd(G.res.X, G.res.Y);
        }

        public void Draw()
        {
            G.batch.BeginWithDepth(typeof(Depth), SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, camera.matrix);

            DrawM.gDevice.Clear(Colors.background);

            DrawIngame();

            G.batch.End();


            G.batch.Begin(samplerState:SamplerState.PointClamp);

            DrawOnScreen();

            G.batch.End();
        }

        private void DrawIngame()
        {
            Depth.cursor.Set(() =>
            {
                Tex.Placeholder.book_of_no_limits.Draw(Vector2.Zero);
                Tex.Placeholder.cursor.Draw(camera.mousePos.FloorVector() + new Vector2(-16));
            });

            Depth.one.Set(() =>
            {
                DrawM.Sprite.DrawRectangle(G.batch, new M_Rectangle(-250, -100, 100, 100), Color.Red, Drawer.depth);
            });

            //Depth.zero.Set(() =>
            //{
                DrawM.Sprite.DrawRectangle(G.batch, new M_Rectangle(-350, -200, 100, 100), Color.Green, Drawer.depth);
                DrawM.Sprite.DrawRectangle(G.batch, new M_Rectangle(-300, -150, 100, 100), Color.Blue, Drawer.depth);
            //});
        }

        private void DrawOnScreen()
        {
            string normal = "[Esc] Exit\n[F11] Toggle Fullscreen\n[R] Restart\n[Tab] Toggle Extended\n";

            if (extended)
                normal += @"[F12] Take Screenshot
[Ctrl+C] Copy Screenshot to Clipboard
[Left | Right] Swap Screen
[Ctrl+F5] Play replay
[Up] double update speed
[Down] halve update speed
<Left Shift> 10x update speed
<Left Shift + Left Control> 100x update speed
<Right Control> pause game
<Left Alt> 0.2 update speed
<Left Alt + Left Control> 0.04 update speed";

            Font.big.Draw(normal, new Vector2(16), Color.Black, new Vector2(2f));
        }

        public void Dispose()
        {

        }
    }
}
