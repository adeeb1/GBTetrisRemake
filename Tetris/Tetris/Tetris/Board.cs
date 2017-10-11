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
    //The game board
    public sealed class Board
    {
        //The board size where the pieces move
        public static readonly Vector2 BoardSize;

        //The place on the game screen where the next piece is drawn
        public static readonly Rectangle NextPieceRect;

        //How many points you get for clearing 1, 2, 3, and 4 lines, respectively
        private static readonly int[] LineScores;

        //The board limits
        public const int LeftLimit = 32;
        public const int RightLimit = 192;

        //The lines on the board
        private Texture2D[][] Lines; //Not Null = filled, Null = Not Filled

        //Keeps track of the lines cleared and moves the lines above them down by one
        private List<int> LinesCleared;
        private int TotalLinesCleared;
        private float PrevCleared;

        private int TimesPlayed;

        //The piece the player rotates and moves
        private Piece ActivePiece;

        //The piece that comes up next
        private Piece NextPiece;

        private float PrevGameOver;
        private float PrevGameOverFill;
        private int GameOverLine;

        private int LevelNum;
        private int Score;

        public Board()
        {
            //Initialize the board
            Lines = new Texture2D[(int)BoardSize.X / Piece.BlockSize][];
            for (int i = 0; i < Lines.Length; i++)
                Lines[i] = new Texture2D[(int)BoardSize.Y / Piece.BlockSize];

            LinesCleared = new List<int>();
            TotalLinesCleared = 0;
            TimesPlayed = 0;

            //Debug
            //for (int x = 0; x < Lines.Length - 1; x++)
            //{
            //    Lines[x][9] = LoadGraphics.BlockGraphic;
            //    Lines[x][10] = LoadGraphics.BlockGraphic;
            //    Lines[x][11] = LoadGraphics.BlockGraphic;
            //    Lines[x][12] = LoadGraphics.BlockGraphic;
            //    Lines[x][13] = LoadGraphics.BlockGraphic;
            //    Lines[x][14] = LoadGraphics.BlockGraphic;
            //    Lines[x][15] = LoadGraphics.BlockGraphic;
            //    Lines[x][16] = LoadGraphics.BlockGraphic;
            //    Lines[x][17] = LoadGraphics.BlockGraphic;
            //}

            ActivePiece = Piece.RandomPiece();
            NextPiece = Piece.RandomPiece();

            PrevCleared = 0f;
            PrevGameOver = 0f;
            PrevGameOverFill = 0f;
            GameOverLine = Lines[0].Length - 1;

            LevelNum = Score = 0;
        }

        static Board()
        {
            BoardSize = new Vector2(160, 288);
            NextPieceRect = new Rectangle(238, 206, 68, 68);

            LineScores = new int[] { 40, 100, 300, 1200 };
        }

        public Texture2D[][] BoardLines
        {
            get { return Lines; }
        }

        public Piece GetActivePiece
        {
            get { return ActivePiece; }
        }

        //Gets the rectangle of a particular block on the board
        public Rectangle GetBlockRect(int x, int y)
        {
            return (new Rectangle((x + (LeftLimit / Piece.BlockSize)) * Piece.BlockSize, y * Piece.BlockSize, Piece.BlockSize, Piece.BlockSize));
        }

        //Gets a particular block on the board from a rectangle
        public Vector2 GetBlockFromRect(Rectangle block)
        {
            return (new Vector2((block.X - LeftLimit) / Piece.BlockSize, block.Y / Piece.BlockSize));
        }

        public float MoveDownTimer
        {
            get { return 600f - (LevelNum * 48f); }
        }

        //Collision
        public bool TouchedLeftSide(int XChange)
        {
            return ((ActivePiece.GetPieceLeft + (XChange * Piece.BlockSize)) < GetBlockRect(0, 0).X);
        }

        public bool TouchedRightSide(int XChange)
        {
            return ((ActivePiece.GetPieceRight + (XChange * Piece.BlockSize)) > GetBlockRect(BoardLines.Length - 1, 0).Right);
        }

        public bool TouchedEitherSide()
        {
            return (TouchedLeftSide(0) || TouchedRightSide(0));
        }

        private bool TouchedBottom()
        {
            return ((ActivePiece.GetPieceBottom + Piece.BlockSize) > Board.BoardSize.Y);
        }

        // Collision with another block on the board (X check)
        public bool PieceCollidedX(int XChange)
        {
            // Loop through all of the blocks in the active piece
            for (int i = 0; i < ActivePiece.GetBlocks.Length; i++)
            {
                // Get the X and Y position on the game board for the piece's particular block
                Vector2 block = GetBlockFromRect(ActivePiece.GetBlocks[i]);

                // Check if the block is within the X boundaries
                if ((block.X + XChange) >= 0 && (block.X + XChange) < Lines.Length)
                {
                    // Check if the block to the side of the current block contains a block
                    if (Lines[((int)block.X) + XChange][(int)block.Y] != null)
                    {
                        // Return true
                        return true;
                    }
                }
            }
            
            // The piece didn't collide with any other piece, so return false
            return false;
        }

        // Collision with another block on the board (Y check)
        public bool PieceCollidedY()
        {
            // Loop through all of the blocks in the active piece
            for (int i = 0; i < ActivePiece.GetBlocks.Length; i++)
            {
                // Get the X and Y position on the game board for the piece's particular block
                Vector2 block = GetBlockFromRect(ActivePiece.GetBlocks[i]);

                // Check if the block is within the Y boundaries
                if (block.Y < (Lines[(int)block.X].Length - 1))
                {
                    // Check if the block under the current block contains a block
                    if (Lines[(int)block.X][((int)(block.Y)) + 1] != null)
                    {
                        // Return true
                        return true;
                    }
                }
            }

            // The piece didn't collide with any other piece, so check if it touched the bottom of the game board
            return TouchedBottom();
        }

        //Clear lines after placing a piece
        private void CascadeLines()
        {
            int TopLine = ActivePiece.GetPieceTop / Piece.BlockSize;
            int BottomLine = (ActivePiece.GetPieceBottom / Piece.BlockSize) - 1;

            for (int y = BottomLine; y >= TopLine; y--)
            {
                int filledcount = 0;

                for (int x = 0; x < Lines.Length; x++)
                {
                    if (Lines[x][y] != null) filledcount++;
                    else break;
                }

                //Clear a line if there is a block in each line
                if (filledcount == Lines.Length)
                {
                    //FillLine(y);
                    LinesCleared.Add(y);
                }
            }

            //If at least one line was cleared then move the other lines above it down
            if (LinesCleared.Count > 0)
            {
                PrevCleared = (float)Main.GetActiveTime;
                //MoveLinesDown(LinesCleared);
                //AddToScore(LinesCleared.Count);
            }
        }

        private void AddToScore(int linescleared)
        {
            //Score calculation obtained from: http://tetris.wikia.com/wiki/Scoring
            if (linescleared > 0 && linescleared <= LineScores.Length)
            {
                Score += (LineScores[linescleared - 1]) * (LevelNum + 1);
            }
        }

        //Moves the lines above a cascade down
        private void MoveLinesDown(List<int> LinesCleared)
        {
            // Loop through all of the lines cleared
            for (int i = 0; i < LinesCleared.Count; i++)
            {
                // All cleared lines after the first one should be moved down to accurately represent their position on the board
                LinesCleared[i] += i;

                // Loop through the cleared line and all lines above it
                for (int y = LinesCleared[i]; y > 0; y--)
                {
                    // Loop through each block on the current line
                    for (int x = 0; x < Lines.Length; x++)
                    {
                        // Set the block equal to the block on the line above it, essentially moving the blocks down
                        Lines[x][y] = Lines[x][y - 1];
                    }
                }
            }
        }

        //Clear or fill a line
        private void FillLine(int y, Texture2D blockgraphic = null)
        {
            for (int x = 0; x < Lines.Length; x++)
            {
                Lines[x][y] = blockgraphic;
            }
        }

        public void PlacePieceOnBoard()
        {
            //Go through all the blocks the piece has and fill them onto the board
            for (int i = 0; i < ActivePiece.GetBlocks.Length; i++)
            {
                Vector2 xy = GetBlockFromRect(ActivePiece.GetBlocks[i]);

                Lines[(int)xy.X][(int)xy.Y] = ActivePiece.GetBlockImage;
            }

            //Check if the player cleared any lines
            CascadeLines();

            //Add to the score
            Score += ActivePiece.PointsAwarded;

            ActivePiece = NextPiece;
            NextPiece = Piece.RandomPiece();

            //Check if it's a game over
            if (PieceCollidedX(0) == true)
            {
                Main.GameOver = true;
                PrevGameOver = (float)Main.GetActiveTime;
            }
        }

        public void Update(KeyboardState playerinput)
        {
            if (Main.GamePaused == false && Main.GameOver == false && ActivePiece != null && LinesCleared.Count == 0)
            {
                ActivePiece.Update(this, playerinput);

                //Check if the active piece collided with another piece or touched the bottom of the game board
                if (PieceCollidedY() == true)
                {
                    // Place the piece on the board
                    PlacePieceOnBoard();
                }
            }
            //Play the cleared animation
            else if (LinesCleared.Count > 0 && TimesPlayed < 3)
            {
                float timer = (float)(Main.GetActiveTime - PrevCleared);

                //Draw the cleared block graphic
                if (timer <= 300f)
                {
                    //Make sure the graphic isn't already set
                    if (Lines[0][LinesCleared[0]] != LoadGraphics.ClearBlock)
                    {
                        for (int i = 0; i < LinesCleared.Count; i++)
                        {
                            FillLine(LinesCleared[i], LoadGraphics.ClearBlock);
                        }
                    }
                }
                //Draw the normal block graphic
                else if (timer > 300f && timer < 600f)
                {
                    //Make sure the graphic isn't already set
                    if (Lines[0][LinesCleared[0]] != LoadGraphics.BlockGraphic)
                    {
                        for (int i = 0; i < LinesCleared.Count; i++)
                        {
                            FillLine(LinesCleared[i], LoadGraphics.BlockGraphic);
                        }
                    }
                }
                //Finish the animation
                else if (timer >= 600f)
                {
                    //Increment the number of times the animation plays - it plays 3 times
                    TimesPlayed++;

                    //When the animation has played 3 times, clear the line, move the ones above down, and give the player score
                    if (TimesPlayed >= 3)
                    {
                        //Clear all the lines
                        for (int i = 0; i < LinesCleared.Count; i++)
                            FillLine(LinesCleared[i]);

                        //Move the lines down and add to the player's score based on how many lines were cleared at once
                        MoveLinesDown(LinesCleared);
                        AddToScore(LinesCleared.Count);

                        //Reset variables and add to the total line count
                        TimesPlayed = 0;
                        TotalLinesCleared += LinesCleared.Count;
                        LinesCleared.Clear();

                        //Increase the level number based on how many total lines are cleared; Level 9 is the max
                        if (LevelNum < 9) LevelNum = (TotalLinesCleared / 10);
                    }
                    else PrevCleared = (float)Main.GetActiveTime;
                }
            }
            //Fill up the board with game over blocks
            else if (Main.GameOver == true && GameOverLine >= 0)
            {
                if ((Main.GetActiveTime - PrevGameOver) >= 700f && (Main.GetActiveTime - PrevGameOverFill) >= 75f)
                {
                    //Fill a line
                    FillLine(GameOverLine, LoadGraphics.GameOverBlock);

                    GameOverLine--;
                    PrevGameOverFill = (float)Main.GetActiveTime;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the game board
            spriteBatch.Draw(LoadGraphics.BoardGraphic, Vector2.Zero, Color.White);
            
            //Draw the active piece
            if (ActivePiece != null) ActivePiece.Draw(spriteBatch);
            if (NextPiece != null) NextPiece.DrawNext(spriteBatch);

            //Draw all the pieces on the board
            for (int x = 0; x < Lines.Length; x++)
            {
                for (int y = 0; y < Lines[x].Length; y++)
                {
                    if (Lines[x][y] != null)
                        spriteBatch.Draw(Lines[x][y], GetBlockRect(x, y), Color.White);
                }
            }

            //Draw the score
            Vector2 scoresize = LoadGraphics.TetrisFont.MeasureString(Score.ToString());
            Vector2 levelsize = LoadGraphics.TetrisFont.MeasureString(LevelNum.ToString());
            Vector2 linesize = LoadGraphics.TetrisFont.MeasureString(TotalLinesCleared.ToString());

            spriteBatch.DrawString(LoadGraphics.TetrisFont, Score.ToString(), new Vector2((int)((BoardSize.X * 1.66f) - (scoresize.X / 2)), 40), Color.Black, 0f, Vector2.Zero, .85f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(LoadGraphics.TetrisFont, LevelNum.ToString(), new Vector2((int)((BoardSize.X * 1.66f) - (levelsize.X / 2)), 105), Color.Black, 0f, Vector2.Zero, .8f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(LoadGraphics.TetrisFont, TotalLinesCleared.ToString(), new Vector2((int)((BoardSize.X * 1.66f) - (linesize.X / 2)), 153), Color.Black, 0f, Vector2.Zero, .8f, SpriteEffects.None, 1f);

            //Say "Game Over" if it's game over
            if (Main.GameOver == true && GameOverLine < 0)
            {
                Vector2 stringsize = LoadGraphics.TetrisFont.MeasureString("Game Over");

                spriteBatch.DrawString(LoadGraphics.TetrisFont, "Game Over", new Vector2((int)((BoardSize.X / 2) - (stringsize.X / 4)), (int)((BoardSize.Y / 2) - (stringsize.Y / 2))), Color.White);
            }
        }
    }
}
