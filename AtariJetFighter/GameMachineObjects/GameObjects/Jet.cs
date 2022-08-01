using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.GameEngine.GameObjects
{
    class Jet : GameObject
    {

        // this number is retrieved from first connection message  
        public long PlayerId { get; }
        public float AttackSpeed = 0.25f;
        public float NextShotCooldown = 0f;
        public float SteeringVelocity = 1.0f;

        public Jet(long playerId, byte objectId, Vector2 spawnPosition, float spawnRotation)
        {
            this.PlayerId = playerId;
            this.ObjectID = objectId;
            this.Position = spawnPosition;
            this.Rotation = spawnRotation;
            this.Velocity = 1f;
        }

        public void Update(GameTime gametime, Controls direction)
        {
            var elapsed = gametime.ElapsedGameTime.TotalMilliseconds;
            var rotationAmount = elapsed * SteeringVelocity;

            if (direction == Controls.Left)
            {
                this.Position = new Vector2(Position.X + (float)elapsed * Velocity, Position.Y);
                this.Rotation += (float)rotationAmount;
            } else if (direction == Controls.Right)
            {
                this.Position = new Vector2(Position.X - (float)elapsed * Velocity, Position.Y);

                this.Rotation -= (float)rotationAmount;
            }

            // move in the direction of rotation
            //this.Position = new Vector2(Position.X + (float)elapsed * Velocity, Position.Y);
            
        }

    }
}
