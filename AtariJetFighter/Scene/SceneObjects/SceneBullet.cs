using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.Scene.SceneObjects
{
    class SceneBullet: SceneGameObject
    {
        public SceneBullet(byte objectId, Vector2 position, float rotation, Color color)
        {
            this.ObjectId = objectId;
            this.Position = position;
            this.Rotation = rotation;
            this.color = color;
        }

    }
}
