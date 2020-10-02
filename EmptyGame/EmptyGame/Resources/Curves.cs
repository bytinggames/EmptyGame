using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace EmptyGame
{
    public static class Curves
    {
        public static float Sin(float x) => (float)Math.Sin(x * MathHelper.Pi - MathHelper.Pi / 2f) / 2f + 0.5f;
        public static float SinAccelerate(float x) => (float)Math.Sin(x * MathHelper.Pi / 2f - MathHelper.Pi / 2f) + 1f;
        public static float SinDecelerate(float x) => (float)Math.Sin(x * MathHelper.Pi / 2f);
        public static float Linear(float x) => x;
    }
}
