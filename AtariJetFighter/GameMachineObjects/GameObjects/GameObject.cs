using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.GameEngine.GameObjects
{
    abstract class GameObject
    {
        public byte ObjectID { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Velocity { get; set; }

        public void Update(GameTime gametime)
        {
            var elapsed = gametime.ElapsedGameTime.TotalMilliseconds;
            var movementAmount = (float)elapsed * Velocity;

            var X = (float)Math.Cos((double)this.Rotation);
            var Y = (float)Math.Sin((double)this.Rotation);

            this.Position += movementAmount * (new Vector2(X, Y));

            float newX = this.Position.X;
            float newY = this.Position.Y;
            if (this.Position.X > Constants.ScreenWidth)
            {
                newX = this.Position.X - Constants.ScreenWidth;
            }
            if (this.Position.X < 0)
            {
                newX = this.Position.X + Constants.ScreenWidth;
            }

            if (this.Position.Y > Constants.ScreenWidth)
            {
                newY = this.Position.Y - Constants.ScreenWidth;
            }
            if (this.Position.Y < 0)
            {
                newY = this.Position.Y + Constants.ScreenWidth;
            }

            this.Position = new Vector2(newX, newY);
            // move in the direction of rotation
            //this.Position = new Vector2(Position.X + (float)elapsed * Velocity, Position.Y);

        }
    }
    
}
