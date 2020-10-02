using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyGame
{
    class U_WaitUntil : UpdrawIn<Func<U_WaitUntil, bool>>
    {
        public U_WaitUntil(Func<U_WaitUntil, bool> _until, Func<IUpdraw> _output) : base(_until, _output)
        {
        }
        protected override void Update(ref IUpdraw _return)
        {
            if (input(this))
            {
                _return = output();
            }
        }
    }
}
