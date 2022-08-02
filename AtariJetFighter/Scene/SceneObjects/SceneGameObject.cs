using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.Scene.SceneObjects
{
    /// <summary>
    /// Abstract class of game object living on CLIENT inside GameScene.
    /// No logic is done is this class, it's purpose is to hold data to be displayed on the player's screen.
    /// </summary>
    abstract class SceneGameObject
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Color color { get; set; }
        public byte ObjectId { get; set; }
        public long OwnerId { get; set; }
    }
}
