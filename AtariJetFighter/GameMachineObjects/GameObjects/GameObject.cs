using Microsoft.Xna.Framework;
using System;

namespace AtariJetFighter.GameEngine.GameObjects
{
    /// <summary>
    /// Abstract class to represent the base class for both Jet and Bullet object. 
    /// These then inherit from this class to provide some additional properties and funcionality.
    /// </summary>
    abstract class GameObject
    {
        /// <summary>
        /// Unique identifier of the object used in GameMachine to control object behavior.
        /// </summary>
        public byte ObjectID { get; set; }
        /// <summary>
        /// Current position of the object inside gaming screen. 
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// Current rotation of the object inside gaming scene in Radians.
        /// </summary>
        public float Rotation { get; set; }
        /// <summary>
        /// Traveling speed of gameObject.
        /// </summary>
        public float Velocity { get; set; }

        /// <summary>
        /// Color code of the object.
        /// </summary>
        public byte Color { get; set; }

        /// <summary>
        /// This method is responsible for moving GameObject on the screen. It's the same for Jet and Bullet since they move in the same way.
        /// Direction vector is calculated based on Rotation property. This vector is then multiplied by Velocity and gameTime.Elapsed to 
        /// ensure the same performance on every machine.
        /// </summary>
        /// <param name="gametime">Gametime parameter is used to calculate correct amount to move base on tick rate.</param>
        public void Update(GameTime gametime)
        {
            var elapsed = gametime.ElapsedGameTime.TotalMilliseconds;
            var movementAmount = (float)elapsed * Velocity;
            
            // calculate direction vector for movement
            var X = (float)Math.Cos((double)this.Rotation);
            var Y = (float)Math.Sin((double)this.Rotation);

            var possibleNewPosition = this.Position + movementAmount * (new Vector2(X, Y));

            float newX = possibleNewPosition.X;
            float newY = possibleNewPosition.Y;
            //check if the object did not fly out of bounds of the screen 
            if (possibleNewPosition.X > Constants.ScreenWidth)
            {
                newX = possibleNewPosition.X - Constants.ScreenWidth;
            }
            if (this.Position.X < 0)
            {
                newX = possibleNewPosition.X + Constants.ScreenWidth;
            }

            if (this.Position.Y > Constants.ScreenWidth)
            {
                newY = possibleNewPosition.Y - Constants.ScreenWidth;
            }
            if (this.Position.Y < 0)
            {
                newY = possibleNewPosition.Y + Constants.ScreenWidth;
            }

            this.Position = new Vector2(newX, newY);
        }
    }    
}
