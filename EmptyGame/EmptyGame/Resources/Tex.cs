using System;
using System.IO;
using JuliHelper;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EmptyGame
{
    public static class Tex
    {
        public class Placeholder
        {
            public static Texture2D book_of_no_limits, cursor;
        }

        public static void Initialize(GraphicsDevice gDevice, ContentManager content)
        {
#if DEBUG
            string contentPath = Path.Combine(Calculate.GetParentPath(Environment.CurrentDirectory, 5), GameIdentity.ProjectName, "Content");
            ContentFenja.LoadRaw(typeof(Tex), contentPath, "Textures", gDevice);
#else
            ContentFenja.LoadProcessed(typeof(Tex), "Textures", content);
#endif
        }
    }
}
