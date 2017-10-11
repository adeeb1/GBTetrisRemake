using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public static class LoadGraphics
    {
        public static Texture2D BoardGraphic;
        public static Texture2D BlockGraphic;
        public static Texture2D GameOverBlock;
        public static Texture2D ClearBlock;

        public static SpriteFont TetrisFont;

        public static void LoadContent(ContentManager Content)
        {
            BoardGraphic = Content.Load<Texture2D>("BoardGraphics\\BoardGraphic");
            BlockGraphic = Content.Load<Texture2D>("BlockGraphics\\BlockGraphic");
            GameOverBlock = Content.Load<Texture2D>("BlockGraphics\\GameOverBlock");
            ClearBlock = Content.Load<Texture2D>("BlockGraphics\\ClearBlock");

            TetrisFont = Content.Load<SpriteFont>("TetrisFont");
        }
    }
}
