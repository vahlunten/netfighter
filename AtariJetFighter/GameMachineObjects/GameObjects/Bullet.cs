using Microsoft.Xna.Framework;

namespace AtariJetFighter.GameEngine.GameObjects
{
    /// <summary>
    /// Object representing the Jet. 
    /// It's instances only live on the Server's gameMachine.
    /// </summary>
    class Bullet : GameObject
    {
        /// <summary>
        /// Amount of time bullet lives.
        /// </summary>
        public float Lifespan { get; set; } =  3.0f;
        /// <summary>
        /// Amount of time of bullets life left. This value is reduced each tick. 
        /// </summary>
        public float LifespanLeft { get; set; }  = 3.0f;
        /// <summary>
        /// ID of player, who shot the bullet. 
        /// </summary>
        public long ShotByID { get; set; }

        /// <summary>
        /// Bullet constructor is called when gameMachine receives UserControlMessage with shoot control.
        /// </summary>
        /// <param name="shotById">Id of player who shot the bullet.</param>
        /// <param name="objectId">Id of bullet inside scene.</param>
        /// <param name="spawnPosition">Initial position based on shooting jet position.</param>
        /// <param name="spawnRotation">Rotation based on shooting jet rotation.</param>
        public Bullet(long shotById, byte objectId, Vector2 spawnPosition, float spawnRotation, byte color)
        {
            this.ShotByID = shotById;
            this.ObjectID = objectId;
            this.Position = spawnPosition;
            this.Rotation = spawnRotation;
            this.Velocity = 0.5f;
            this.Color = color;
            this.ColliderRadius = 7.5f;

        }
        /// <summary>
        /// Updates bullet position and lifespan left.
        /// </summary>
        /// <param name="gameTime"></param>
        public new void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            LifespanLeft -= (float)elapsed;

        }
        
       
    }
}
