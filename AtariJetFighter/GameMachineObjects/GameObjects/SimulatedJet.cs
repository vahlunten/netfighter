using Microsoft.Xna.Framework;
using System;

namespace AtariJetFighter.GameMachineObjects.GameObjects
{
    class SimulatedJet
    {
        public State CurerntState;
        public float StateChangeCooldown;
        public float SecondsSinceLastShot;
        public long Id = 23;
        private Random rng;

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
            SecondsSinceLastShot = 3.0f;
            rng = new Random();
        
        }

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
