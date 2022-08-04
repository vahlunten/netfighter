using Microsoft.Xna.Framework;
using System;

namespace AtariJetFighter.GameMachineObjects.GameObjects
{
    /// <summary>
    /// This object represents state of the simulated jet. GameMachine then controls Jet game object based on it.
    /// </summary>
    class SimulatedJet
    {
        public State CurerntState;
        public float StateChangeCooldown;
        public float SecondsSinceLastShot;
        public long Id = 23;
        private Random rng;

        /// <summary>
        /// Possible states of simulated jet
        /// </summary>
        public enum State : byte
        {
            SteeringLeft,
            SteeringRight,
            GoingStraight
        }
        public SimulatedJet()
        {
            CurerntState = State.GoingStraight;
            StateChangeCooldown = 3.0f;
            SecondsSinceLastShot = 2.0f;
            rng = new Random();
        
        }

        /// <summary>
        /// Update timers.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            StateChangeCooldown -= elapsed;
            SecondsSinceLastShot += elapsed;

            if (StateChangeCooldown < 0)
            {
                StateChangeCooldown = (float)rng.NextDouble() * 3f;
                CurerntState = (State)rng.Next(0, 3);
            }


        }
    }
}
