using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Tetris
{
    public static class Input
    {
        public static bool ButtonPressed(KeyboardState playerinput, Keys keypressed)
        {
            return (playerinput.IsKeyUp(keypressed) == true && Keyboard.GetState().IsKeyDown(keypressed) == true);
        }

        public static bool ButtonHeld(Keys keypressed)
        {
            return (Keyboard.GetState().IsKeyDown(keypressed) == true);
        }
    }
}
