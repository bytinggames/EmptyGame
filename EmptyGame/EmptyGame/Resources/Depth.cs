using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;

namespace EmptyGame
{
    public static class DepthScreen
    {
        [Order] public static DepthLayer all;
    }

    public static class Depth
    {
        [Order] public static DepthLayer zero;
        [Order] public static DepthLayer cursor;
        [Order] public static DepthLayer one;
    }
}
