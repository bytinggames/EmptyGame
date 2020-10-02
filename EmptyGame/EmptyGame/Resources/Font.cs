using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;
using Microsoft.Xna.Framework.Graphics;

namespace EmptyGame
{
    public static class Font
    {
        public static SpriteFont small, big, bigSpace, reallyBig;

        public static void Initialize()
        {
            small = ContentLoader.fonts["lato-thin-mod_10"];
            big = ContentLoader.fonts["lato-thin-mod_14_b"];
            bigSpace = ContentLoader.fonts["lato-thin-mod_14_b_space"];
            bigSpace.Spacing = 1f;
            reallyBig = ContentLoader.fonts["lato_28"];
            small.LineSpacing = 18;
            big.LineSpacing = 23;

            SpriteFont[] fonts = new SpriteFont[] { small, big };

            for (int i = 0; i < fonts.Length; i++)
            {
                int index = fonts[i].Characters.IndexOf('r');
                fonts[i].Glyphs[index].RightSideBearing = 1f;

                index = fonts[i].Characters.IndexOf('T');
                fonts[i].Glyphs[index].RightSideBearing = 1f;
            }
        }
    }
}