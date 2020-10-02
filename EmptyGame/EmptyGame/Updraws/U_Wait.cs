using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyGame
{
    class U_Wait : UpdrawIn<int>
    {
        public U_Wait(int _frames, Func<IUpdraw> _output) : base(_frames, _output)
        {
        }
        protected override void Update(ref IUpdraw _return)
        {
            if (FramesAlive() >= input)
            {
                _return = output();
            }
        }
    }
}
