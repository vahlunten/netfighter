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
        /// <summary>
        /// Property to keep the track of keyboard from previous tick. 
        /// </summary>
        private static KeyboardState oldKeybordState;
        /// <summary>
        /// Property to keep track of current state of the keyboard.
        /// </summary>
        private static KeyboardState newKeyBoardState;

        /// <summary>
        /// Function saves the state of keyboard from previous tick and gets current state. This method is called once in Update() function of JetfighterGame instance.
        /// </summary>
        public static void UpdateKeyboardState()
        {
            oldKeybordState = newKeyBoardState;
            newKeyBoardState = Keyboard.GetState();
        }

        /// <summary>
        /// Function returns bool if key has been pressed in previous tick and realeased in this tick.
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
        /// <summary>
        /// Function returns bool wether key is pressed in current tick.
        /// </summary>
        /// <param name="key">Key to be checked, if it is pressed.</param>
        /// <returns></returns>
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
