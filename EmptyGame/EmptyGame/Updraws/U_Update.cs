using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;

namespace EmptyGame
{
    class U_Update : IUpdraw
    {
        ActionRef<IUpdraw> update;

        public U_Update(ActionRef<IUpdraw> _update)
        {
            update = _update;
        }
        protected override void Update(ref IUpdraw _return)
        {
            update(ref _return);
        }
    }
}
