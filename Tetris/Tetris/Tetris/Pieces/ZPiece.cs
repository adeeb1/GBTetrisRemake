using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tetris
{
    public sealed class ZPiece : Piece
    {
        public ZPiece()
        {
            Rectangle spawnloc = SpawnPiece();

            Blocks = new Rectangle[4] { spawnloc, CreateBlock(spawnloc.X + Piece.BlockSize, spawnloc.Y), CreateBlock(spawnloc.X + Piece.BlockSize, spawnloc.Y + Piece.BlockSize), 
                                        CreateBlock(spawnloc.X + (Piece.BlockSize * 2), spawnloc.Y + Piece.BlockSize) };
        }

        //Rotate around the second block
        protected override void RotatePiece(bool clockwise)
        {
            base.RotatePiece(clockwise);

            int Change = (Rotation % 2 != 0) ? 2 : -2;
            Change *= BlockSize;

            Blocks[2].Y -= Change;
            Blocks[3].X -= Change;
        }
    }
}
