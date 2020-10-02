using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyGame
{
    public class U_Multiple : UpdrawInOut<IUpdraw[], object[]>
    {
        object[] outputParameters;

        int waitForUpdraws;

        public U_Multiple(IUpdraw[] _input, Func<object[], IUpdraw> _output) : base(_input, _output)
        {
            waitForUpdraws = _input.Length;
            outputParameters = new object[_input.Length];
        }
        public U_Multiple(int _waitForUpdraws, IUpdraw[] _input, Func<object[], IUpdraw> _output) : base(_input, _output)
        {
            waitForUpdraws = _waitForUpdraws;
            outputParameters = new object[_input.Length];
        }

        bool done = false;

        protected override void Update(ref IUpdraw _return)
        {
            int i;
            for (i = 0; i < input.Length; i++)
            {
                if (input[i] != null)
                {
                    input[i].UpdateUpdraw(ref input[i]);

                    if (input[i] is U_Output output)
                    {
                        outputParameters[i] = output.GetOutput();
                        input[i] = null;
                    }
                }
            }
            
            for (i = 0; i < waitForUpdraws; i++)
            {
                if (input[i] != null)
                    break;
            }

            if (i == waitForUpdraws)
            {
                done = true;
                // if done
                _return = output(outputParameters);
            }
        }

        public override void Draw()
        {
            if (!done)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    input[i]?.Draw();
                }
            }
        }
    }
}
