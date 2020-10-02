using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyGame
{
    class U_Anim : UpdrawIn<(int frames, Func<float, float> curve, Action<float> update)>
    {
        int f = 0;

        public U_Anim(int frames, Func<float, float> curve, Action<float> update, Func<IUpdraw> _output) : base((frames, curve, update), _output)
        {
        }

        protected override void Update(ref IUpdraw _return)
        {
            f++;

            float x = input.curve((float)f / Math.Max(1, input.frames));

            input.update(x);

            if (f >= input.frames)
            {
                _return = output();
            }
        }
    }
}
