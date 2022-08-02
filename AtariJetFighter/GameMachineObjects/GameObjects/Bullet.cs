using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.GameEngine.GameObjects
{
    class Bullet : GameObject
    {
        public float lifespan = 5.0f;
        public float lifespanLeft = 5.0f;
        public long ShotByID;

        public Bullet(long shotById, byte objectId, Vector2 spawnPosition, float spawnRotation)
        {
            this.ShotByID = shotById;
            this.ObjectID = objectId;
            this.Position = spawnPosition;
            this.Rotation = spawnRotation;
            this.Velocity = 0.4f;
        }
        
       
    }
}
