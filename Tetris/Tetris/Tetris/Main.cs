using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Tetris
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //The active time
        private static double activeTime;

        //The game board
        private Board GameBoard;

        //Keyboard input
        private KeyboardState PlayerInput;

        public static bool GamePaused;
        public static bool GameOver;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 320;
            graphics.PreferredBackBufferHeight = 288;

            Content.RootDirectory = "Content";

            PlayerInput = new KeyboardState(Keys.Enter, Keys.Z, Keys.X);
        }

        static Main()
        {
            activeTime = 0f;
            GamePaused = false;
            GameOver = false;
        }

        public static double GetActiveTime
        {
            get { return activeTime; }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadGraphics.LoadContent(Content);

            GameBoard = new Board();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Increase game time
            if (GamePaused == false)
                activeTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            
            //Update the game board
            GameBoard.Update(PlayerInput);

            //Pause the game
            if (GameOver == false && Input.ButtonPressed(PlayerInput, Keys.Enter) == true)
                GamePaused = !GamePaused;

            //Get current keyboard state
            PlayerInput = Keyboard.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            Matrix ScaleMatrix = Matrix.CreateScale(new Vector3((float)graphics.GraphicsDevice.PresentationParameters.BackBufferWidth / 320f, (float)graphics.GraphicsDevice.PresentationParameters.BackBufferHeight / 288f, 1));

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, ScaleMatrix);

            //Draw the game board
            GameBoard.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
