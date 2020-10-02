using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyGame
{
    public interface UpdrawStored
    {
        IUpdraw GetUpdraw();
    }

    public class UpdrawStoredFunc : UpdrawStored
    {
        Func<IUpdraw> func;

        public UpdrawStoredFunc(Func<IUpdraw> func)
        {
            this.func = func;
        }

        public IUpdraw GetUpdraw()
        {
            return func();
        }
    }
    public class UpdrawStoredInstance : UpdrawStored
    {
        IUpdraw updraw;

        public UpdrawStoredInstance(IUpdraw updraw)
        {
            this.updraw = updraw;
        }

        public IUpdraw GetUpdraw()
        {
            return updraw;
        }
    }

}
