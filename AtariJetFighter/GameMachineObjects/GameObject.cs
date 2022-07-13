using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtariJetFighter.GameMachineObjects
{
    internal abstract class GameObject
    {
        public byte Id
        {
            get;
            set;
        }
        public Vector2 Position
        {
            get;
            set;
        }

        public float Rotation
        {
            get;
            set;
        }

        public float Radius
        {
            get;
            set;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public virtual void OnCollision(GameObject collisionObject)
        {

        }

    }
}
