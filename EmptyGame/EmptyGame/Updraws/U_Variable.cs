using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyGame
{
    abstract class U_Output : IUpdraw
    {
        public abstract object GetOutput();
    }

    class U_Object : U_Output
    {
        public object input;
        public U_Object(object _input)
        {
            input = _input;
        }

        public override object GetOutput()
        {
            return input;
        }
    }

    class U_Variable<T> : U_Output
    {
        public T input;
        public U_Variable(T _input)
        {
            input = _input;
        }

        public override object GetOutput()
        {
            return input;
        }
    }
}
