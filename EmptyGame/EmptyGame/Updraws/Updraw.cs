using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyGame
{
    public abstract class IUpdraw
    {
        //int startFrame = Game1.inputRecorderManager.frame;
        int frame = 0;

        public virtual void Draw() { }
        public virtual void DrawScreen() { }

        protected virtual void OnExit() { }

        public void UpdateUpdraw(ref IUpdraw _return)
        {
            if (_return != this)
                throw new Exception();

            frame++;

            Update(ref _return);

            if (_return != this)
            {
                OnExit();
            }
        }

        protected int FramesAlive() => frame;// Game1.inputRecorderManager.frame - startFrame; <- doesn't work, because Game1.inputRecorderManager.frame doesn't increase somehow on sleep...?
        protected void ResetFrame() => frame = 0;// startFrame = Game1.inputRecorderManager.frame;

        protected virtual void Update(ref IUpdraw _return) { }
    }

    public abstract class Updraw : IUpdraw
    {
        protected Func<IUpdraw> output;

        public Updraw(Func<IUpdraw> _output)
        {
            output = _output;
        }
    }

    public abstract class UpdrawOut<Output> : IUpdraw
    {
        protected Func<Output, IUpdraw> output;

        public UpdrawOut(Func<Output, IUpdraw> _output)
        {
            output = _output;
        }
    }

    public abstract class UpdrawIn<Input> : IUpdraw
    {
        protected Input input;

        protected Func<IUpdraw> output;

        public UpdrawIn(Input _input, Func<IUpdraw> _output)
        {
            input = _input;
            output = _output;
        }
    }

    public abstract class UpdrawInOut<Input, Output> : IUpdraw
    {
        protected Input input;

        protected Func<Output, IUpdraw> output;

        public UpdrawInOut(Input _input, Func<Output, IUpdraw> _output)
        {
            input = _input;
            output = _output;
        }
    }
}
