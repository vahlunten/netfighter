using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter
{
    internal static class InputController
    {
        private static KeyboardState oldKeybordState;
        private static KeyboardState newKeyBoardState;

        public static void UpdateKeyboardState()
        {
            oldKeybordState = newKeyBoardState;
            newKeyBoardState = Keyboard.GetState();
        }

        /// <summary>
        /// Function returns bool if key has been pressed in this tick. Returns true after key is RELEASED. 
        /// </summary>
        /// <param name="key">Key to be checked, if it had been pressed. </param>
        /// <returns></returns>
        public static bool hasBeenPressed(Keys key)
        {
            if (oldKeybordState.IsKeyDown(key) && newKeyBoardState.IsKeyUp(key))    
            {
                return true;
            }
            return false;
        }
        public static bool keyIsPressed(Keys key)
        {
            if (newKeyBoardState.IsKeyDown(key))
            {
                return true;
            }
            return false;
        }

    }
}
