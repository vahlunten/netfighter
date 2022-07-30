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
        public byte Id { get; set; }
        public Vector2 Position;
        public float Rotation;
        public float Velocity { get; set; }

    }
}
