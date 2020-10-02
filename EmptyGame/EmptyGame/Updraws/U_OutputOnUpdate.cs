using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyGame
{
    class U_OutputOnUpdate : Updraw
    {
        public U_OutputOnUpdate(Func<IUpdraw> _output) : base(_output)
        {
        }

        protected override void Update(ref IUpdraw _return)
        {
            _return = output();
        }
    }
}
