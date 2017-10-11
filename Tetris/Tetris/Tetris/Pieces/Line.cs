using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tetris
{
    public sealed class Line : Piece
    {
        public Line()
        {
            Rectangle spawnloc = SpawnPiece();

            Blocks = new Rectangle[4] { spawnloc, CreateBlock(spawnloc.X + Piece.BlockSize, spawnloc.Y), CreateBlock(spawnloc.X + (Piece.BlockSize * 2), spawnloc.Y), 
                                        CreateBlock(spawnloc.X + (Piece.BlockSize * 3), spawnloc.Y) };
        }

        //Rotates around the second piece horizontally
        protected override void RotatePiece(bool clockwise)
        {
            base.RotatePiece(clockwise);

            Vector2 secondblockpos = new Vector2(Blocks[1].X, Blocks[1].Y);
            
            //Rotate vertically
            if ((Rotation % 2) != 0)
            {
                for (int i = 0; i < Blocks.Length; i++)
                {
                    if (i != 1)
                    {
                        int xdifference = (int)secondblockpos.X - Blocks[i].X;

                        Blocks[i].X = (int)secondblockpos.X;
                        Blocks[i].Y -= xdifference;
                    }
                }
            }
            //Rotate horizontally
            else
            {
                for (int i = 0; i < Blocks.Length; i++)
                {
                    if (i != 1)
                    {
                        int ydifference = (int)secondblockpos.Y - Blocks[i].Y;

                        Blocks[i].X -= ydifference;
                        Blocks[i].Y = (int)secondblockpos.Y;
                    }
                }
            }
        }
    }
}
