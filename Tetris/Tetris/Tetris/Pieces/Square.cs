using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tetris
{
    public sealed class Square : Piece
    {
        public Square()
        {
            Rectangle spawnloc = SpawnPiece();

            Blocks = new Rectangle[4] { spawnloc, CreateBlock(spawnloc.X + Piece.BlockSize, spawnloc.Y), CreateBlock(spawnloc.X, spawnloc.Y + Piece.BlockSize), 
                                        CreateBlock(spawnloc.X + Piece.BlockSize, spawnloc.Y + Piece.BlockSize) };
        }
    }
}
