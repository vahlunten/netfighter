using Microsoft.Xna.Framework;

namespace AtariJetFighter.Scene.SceneObjects
{
    /// <summary>
    /// Representation of a Jet game object living on Client. 
    /// </summary>
    class SceneJet : SceneGameObject
    {
        public int Score { get; set; }
        public bool IsLocal { get; set; }

        public SceneJet(byte objectId,long ownerId, Vector2 position, float rotation, Color color, bool isLocal, int score = 0)
        {
            this.ObjectId = objectId;
            this.OwnerId = ownerId;
            this.Position = position;
            this.Rotation = rotation;
            this.color = color;
            this.Score = score;
            this.IsLocal = isLocal;
        }
    }
}


