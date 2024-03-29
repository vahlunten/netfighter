﻿using Microsoft.Xna.Framework;

namespace AtariJetFighter.GameEngine.GameObjects
{
    /// <summary>
    /// Object representing the Jet. 
    /// This object lives on the server ONLY and it is used to store Jet information as well as the player's score. 
    /// Each player is assigned one jet. 
    /// </summary>
    class Jet : GameObject
    {
        /// <summary>
        /// Unique identifier of the player, based on netClientID
        /// </summary>
        public long PlayerId { get; set; }
        /// <summary>
        /// The amount of time it takes to reload the cannon and be able to shoot again.
        /// In seconds. 
        /// </summary>
        public float ShotCooldown { get; set; }
        /// <summary>
        /// Current time left to be able to shoot again.
        /// </summary>
        public float ShotCooldownLeft { get; set; }
        /// <summary>
        /// Property to define how fast should jet shot rotate around pivot. 
        /// </summary>
        public float SteeringVelocity { get; set; }
        /// <summary>
        /// Score of this jet.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Jet constructor is called upon connection of a user. 
        /// </summary>
        /// <param name="playerId">Netclient unique identifier.</param>
        /// <param name="objectId">Id of object in a scene.</param>
        /// <param name="spawnPosition">Initial position of the jet.</param>
        /// <param name="spawnRotation">Initial rotation of the jet.</param>
        public Jet(long playerId, byte objectId, Vector2 spawnPosition, float spawnRotation, byte color)
        {
            this.PlayerId = playerId;
            this.ObjectID = objectId;
            this.Position = spawnPosition;
            this.Rotation = spawnRotation;
            this.ShotCooldown = 0.5f;
            this.Velocity = 0.25f;
            this.ShotCooldownLeft = 0f;
            this.SteeringVelocity = 0.0025f;
            this.Color = color;
            this.Score = 0;
            this.ColliderRadius = 22.5f;
        }
        /// <summary>
        /// Steer method is called upon receiving UserControlMessage.
        /// It calculates rotation direction and amount based on gametime.
        /// </summary>
        /// <param name="gameTime">Current gametime.</param>
        /// <param name="direction">Command, in wich direction the Jet should steer.</param>
        public void Steer(GameTime gameTime, Controls direction)
        {
            var elapsed = gameTime.ElapsedGameTime.TotalMilliseconds;
            var rotationAmount = elapsed * SteeringVelocity;

            if (direction == Controls.Left)
            {
                this.Rotation -= (float)rotationAmount ;
            }
            else if (direction == Controls.Right)
            {
                this.Rotation += (float)rotationAmount;
            }
        }

        /// <summary>
        /// Updates bullet position and lifespan left.
        /// </summary>
        /// <param name="gameTime"></param>
        public new void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            ShotCooldownLeft -= (float)elapsed;

        }
    }
}
