using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tetris
{
    public sealed class LPiece : Piece
    {
        public LPiece()
        {
            Rectangle spawnloc = SpawnPiece();

            Blocks = new Rectangle[4] { spawnloc, CreateBlock(spawnloc.X + Piece.BlockSize, spawnloc.Y), CreateBlock(spawnloc.X + (Piece.BlockSize * 2), spawnloc.Y), 
                                        CreateBlock(spawnloc.X, spawnloc.Y + Piece.BlockSize) };
        }

        //Rotate around the middle piece
        protected override void RotatePiece(bool clockwise)
        {
            base.RotatePiece(clockwise);

            Vector2 secondblockpos = new Vector2(Blocks[1].X, Blocks[1].Y);

            //Rotate vertically
            if (Rotation % 2 != 0)
            {
                for (int i = 0; i < Blocks.Length; i++)
                {
                    //Rotate around the second block, so don't bother changing it
                    if (i != 1)
                    {
                        //Get the x difference
                        int xdifference = (int)secondblockpos.X - Blocks[i].X;

                        //The last block is the only one equal to the Y position of the middle block
                        if (i != Blocks.Length - 1)
                        {
                            Blocks[i].X += xdifference;
                            Blocks[i].Y -= xdifference;
                        }
                        //Rotation 1, Blocks[0].Y
                        else Blocks[i].Y = (int)Blocks[Rotation - 1].Y;
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
                        //Get the y difference
                        int ydifference = (int)secondblockpos.Y - Blocks[i].Y;

                        if (i != Blocks.Length - 1)
                        {
                            Blocks[i].X -= ydifference;
                            Blocks[i].Y += ydifference;
                        }
                        //The last block has to move up or down but has the same X position as the middle block
                        else
                        {
                            Blocks[i].X = Blocks[Rotation].X;
                        }
                    }
                }
            }
        }
    }
}