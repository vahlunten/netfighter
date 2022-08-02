using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.Scene.SceneObjects
{
    class SceneJet : SceneGameObject
    {
        public int Score { get; set; }

        public SceneJet(byte objectId,long ownerId, Vector2 position, float rotation, Color color, int score = 0)
        {
            this.ObjectId = objectId;
            this.OwnerId = ownerId;
            this.Position = position;
            this.Rotation = rotation;
            this.color = color;
            this.Score = score;
        }
    }
}


