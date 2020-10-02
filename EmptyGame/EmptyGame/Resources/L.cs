using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;
using Microsoft.Xna.Framework;

namespace EmptyGame
{
    public static class L // Localization
    {
        public static readonly int language = 0;

        public static readonly Loca
            hello = "hi"
            ;

        public static void Initialize()
        {

        }


        static Func<object> F(Func<object> func)
        {
            return func;
        }
    }
}
