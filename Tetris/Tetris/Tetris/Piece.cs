using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace Tetris
{
    public abstract class Piece
    {
        //The piece pool to select from
        public static Piece[] AllPieces;

        //Block size and fall/move speed
        public const int BlockSize = 16;

        //The image to draw
        protected Texture2D BlockImage;

        //The rectangle blocks
        protected Rectangle[] Blocks;

        // Timer for manual, keyboard input
        protected const float KeyboardInputTime = 50f;
        protected float KeyboardTimer;

        // Timer for moving down
        protected float PrevMove;

        //The rotation of the piece
        protected int Rotation;

        //How many spaces the piece dropped while being hard dropped
        public int PointsAwarded;

        public Piece()
        {
            PrevMove = 0f;
            KeyboardTimer = 0f;
            BlockImage = LoadGraphics.BlockGraphic;

            PointsAwarded = 0;
        }

        static Piece()
        {
            AllPieces = new Piece[] { new Square(), new Line(), new ZPiece(), new SPiece(), new JPiece(), new LPiece(), new TPiece() };
        }

        public static Piece RandomPiece()
        {
            switch (new Random().Next(0, AllPieces.Length))
            {
                case 0: return new Square();
                case 1: return new Line();
                case 2: return new ZPiece();
                case 3: return new JPiece();
                case 4: return new LPiece();
                case 5: return new TPiece();
                default: return new SPiece();
            }

            //return AllPieces[new Random().Next(0, AllPieces.Length)];
        }

        public Texture2D GetBlockImage
        {
            get { return BlockImage; }
        }

        public Rectangle[] GetBlocks
        {
            get { return Blocks; }
        }

        public int GetPieceLeft
        {
            get
            {
                int leftpos = 9999;

                //Find the leftmost block in the piece
                for (int i = 0; i < Blocks.Length; i++)
                {
                    if (Blocks[i].X < leftpos)
                    {
                        leftpos = Blocks[i].X;
                    }
                }

                return leftpos;
            }
        }

        public int GetPieceRight
        {
            get
            {
                int rightpos = -9999;

                //Find the rightmost block in the piece
                for (int i = 0; i < Blocks.Length; i++)
                {
                    if (Blocks[i].Right > rightpos)
                    {
                        rightpos = Blocks[i].Right;
                    }
                }

                return rightpos;
            }
        }

        public int GetPieceTop
        {
            get
            {
                int toppos = 9999;

                //Find the bottom-most block in the piece
                for (int i = 0; i < Blocks.Length; i++)
                {
                    if (Blocks[i].Y < toppos)
                    {
                        toppos = Blocks[i].Y;
                    }
                }

                return toppos;
            }
        }

        public int GetPieceBottom
        {
            get
            {
                int bottompos = -9999;

                //Find the bottom-most block in the piece
                for (int i = 0; i < Blocks.Length; i++)
                {
                    if (Blocks[i].Bottom > bottompos)
                    {
                        bottompos = Blocks[i].Bottom;
                    }
                }

                return bottompos;
            }
        }

        protected Rectangle SpawnPiece()
        {
            return (new Rectangle((Board.LeftLimit / 2) + ((int)Board.BoardSize.X / 2), Piece.BlockSize, Piece.BlockSize, Piece.BlockSize));
        }

        protected Rectangle CreateBlock(int x, int y)
        {
            return (new Rectangle(x, y, BlockSize, BlockSize));
        }

        protected bool CheckMovePieceTimer(Board gameboard)
        {
            return (Main.GetActiveTime - PrevMove >= gameboard.MoveDownTimer);
        }

        protected virtual void RotatePiece(bool clockwise)
        {
            //Play sound
            if (clockwise == true) Rotation = (Rotation + 1) % 4;
            else
            {
                Rotation -= 1;

                if (Rotation < 0) Rotation = 3;
            }
        }

        protected void MovePiece(Board gameboard)
        {
            int NewX = 0, NewY = 0;

            //Control how fast you can move left or right
            if ((Main.GetActiveTime - KeyboardTimer) >= KeyboardInputTime)
            {
                // Check if player is moving left
                if (Input.ButtonHeld(Keys.Left) && gameboard.PieceCollidedX(-1) == false && gameboard.TouchedLeftSide(-1) == false)
                {
                    NewX = -BlockSize;
                    KeyboardTimer = (float)Main.GetActiveTime;
                }

                // Checking if player is moving right
                if (Input.ButtonHeld(Keys.Right) && gameboard.PieceCollidedX(1) == false && gameboard.TouchedRightSide(1) == false)
                {
                    NewX = BlockSize;
                    KeyboardTimer = (float)Main.GetActiveTime;
                }
            }

            // Check if the piece should be moved down
            if (((Main.GetActiveTime - KeyboardTimer) >= KeyboardInputTime && Input.ButtonHeld(Keys.Down)) || CheckMovePieceTimer(gameboard) == true)
            {
                //Increase the points awarded when hard dropping a piece
                if (Input.ButtonHeld(Keys.Down) == true) PointsAwarded++;

                NewY = BlockSize;

                PrevMove = KeyboardTimer = (float)Main.GetActiveTime;
            }

            // Check if the piece should be moved
            if (NewX != 0 || NewY != 0)
            {
                // Loop through each block
                for (int i = 0; i < Blocks.Length; i++)
                {
                    Blocks[i].X += NewX;

                    // Move the block down
                    Blocks[i].Y += NewY;
                }
            }

            //Reset the points awarded when hard dropping a piece
            if (Input.ButtonHeld(Keys.Down) == false) PointsAwarded = 0;
        }

        public void Update(Board gameboard, KeyboardState playerinput)
        {
            MovePiece(gameboard);

            if (Input.ButtonPressed(playerinput, Keys.Z) == true)
            {
                RotatePiece(true);

                if ((gameboard.PieceCollidedX(0) == true || gameboard.TouchedEitherSide() == true) || gameboard.PieceCollidedY() == true)
                {
                    //Rotate 3 times to get back to the previous position
                    for (int i = 0; i < 3; i++)
                        RotatePiece(true);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw all the pieces
            for (int i = 0; i < Blocks.Length; i++)
            {
                spriteBatch.Draw(BlockImage, Blocks[i], Color.White);
            }
        }

        public void DrawNext(SpriteBatch spriteBatch)
        {
            //Draw all the pieces
            for (int i = 0; i < Blocks.Length; i++)
            {
                spriteBatch.Draw(BlockImage, new Vector2(Board.NextPieceRect.X + Blocks[i].X - (int)(Board.NextPieceRect.Width * 1.4f), Board.NextPieceRect.Y + Blocks[i].Y + (Board.NextPieceRect.Height / 8) - 7), Color.White);
            }
        }
    }
}
