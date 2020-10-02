using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;

namespace EmptyGame
{
    class U_Draw : U_Update
    {
        ActionRef<IUpdraw> update;
        Action draw, drawScreen;
        
        public U_Draw(Action _draw, ActionRef<IUpdraw> _update)
             : base(_update)
        {
            draw = _draw;
            update = _update;
        }
        public U_Draw(Action draw, Action _drawScreen, ActionRef<IUpdraw> _update)
             : base(_update)
        {
            this.draw = draw;
            drawScreen = _drawScreen;
            update = _update;
        }

        public override void Draw()
        {
            draw?.Invoke();
        }

        public override void DrawScreen()
        {
            drawScreen?.Invoke();
        }
    }
}
