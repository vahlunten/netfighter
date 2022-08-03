using AtariJetFighter.Scene.SceneObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtariJetFighter.Scene
{
    /// <summary>
    /// Scene object holds all the necessary data to by displayed on player's screen while the game is running.
    /// Scene object is controller by Client which handles all the incomming messages from gameMachine.
    /// </summary>
    public class GameScene : DrawableGameComponent
    {
        /// <summary>
        /// Jetfighter game instance
        /// </summary>
        private JetFighterGame game;

        /// <summary>
        /// Collection of jets of currently connected players.
        /// </summary>
        private List<SceneJet> sceneJets = new List<SceneJet>();
        /// <summary>
        /// Collection of jets of currently active bullets.
        /// </summary>
        private List<SceneBullet> sceneBullets = new List<SceneBullet>();
        public GameScene(JetFighterGame game) : base((Game) game)
        {
            this.game = game;
        }
        public void LoadContent(ContentManager content)
        {

        }
        public override void Initialize()
        {
            base.Initialize();
        }
        
        /// <summary>
        /// Draw all scene objects, scores and time left in current round. 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
           if(game.GameState == GameMachineObjects.GameStateEnum.GameRunning)
            {
                GraphicsDevice.Clear(Color.Black);
                this.game.spriteBatch.Begin();
                DrawPlayers();
                DrawBullets();
                DrawScores();
                this.game.spriteBatch.End();
                base.Draw(gameTime);
            }
            
        }
        
        public void DrawPlayers()
        {
            foreach (var sceneJet in sceneJets)
            {
                this.game.spriteBatch.Draw(this.game.jet, sceneJet.Position, null, sceneJet.color, sceneJet.Rotation, new Vector2(32), new Vector2(1), SpriteEffects.None, 0.50f);

                // draw collider
                this.game.spriteBatch.Draw(this.game.collider, sceneJet.Position, null, Color.White, 0f, new Vector2(22.5f), new Vector2(1), SpriteEffects.None, 0.50f);

            }
        }
        
        public void DrawBullets()
        {
            foreach (var bullet in sceneBullets)
            {
                this.game.spriteBatch.Draw(this.game.bullet, bullet.Position, null, bullet.color, bullet.Rotation, new Vector2(7.5f), new Vector2(1), SpriteEffects.None, 0.50f);
                this.game.spriteBatch.Draw(this.game.bullet, bullet.Position, null, Color.White, 0f, new Vector2(0), new Vector2(0.5f), SpriteEffects.None, 0.50f);
            }
        }

        public void DrawScores()
        {
            for (int i = 0; i < sceneJets.Count; i++)
            {
                var jet = sceneJets[i];
                DrawStringScore(jet.Score, jet.IsLocal, i * 150f, 50f, jet.color);
            }
        }

        public void DrawStringScore(int score, bool isLocal, float horizontalOffset, float verticalOffset, Color color, float scale = 0.5f)
        {
            this.game.spriteBatch.DrawString(
                this.game.font,
                isLocal? $"{score} (YOU) " : $" {score}",
                new Vector2(50 + horizontalOffset, verticalOffset),
                color,
                0.0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.5f);
        }
        public void AddJet(byte objectId, long ownerId,  Vector2 position, float rotation, Color color, bool isLocal, int score = 0)
        {
            if(sceneJets.Find(jet => jet.ObjectId == objectId) == null)
            {
                this.sceneJets.Add(new SceneJet(objectId, ownerId, position, rotation, color, isLocal));
            }
        }
        public void AddBullet(byte objectId, Vector2 position, float rotation, Color color)
        {
            if (sceneBullets.Find(bullet => bullet.ObjectId == objectId) == null)
            {
                this.sceneBullets.Add(new SceneBullet(objectId, position, rotation, color));
            }
        }

        public void RemoveJet(byte objectId)
        {
            var removedJet = sceneJets.Find(jet => jet.ObjectId == objectId);
            if (removedJet != null)
            {
                this.sceneJets.Remove(removedJet);
            }
        }

        public void RemoveBullet(byte objectId)
        {
            var removedBullet = sceneBullets.Find(jet => jet.ObjectId == objectId);
            if (removedBullet != null)
            {
                this.sceneBullets.Remove(removedBullet);
            }
        }

        public void UpdateJet(byte objectId, Vector2 position, float rotation)
        {
            var jet = this.sceneJets.Find(jet => jet.ObjectId == objectId);
            if(jet == null)
            {
                return;
            } else
            {
                jet.Position = position;
                jet.Rotation = rotation;
            }
        }

        public void UpdateBullet(byte objectId, Vector2 position, float rotation)
        {
            var bullet = this.sceneBullets.Find(bullet => bullet.ObjectId == objectId);
            if (bullet == null)
            {
                return;
            }
            else
            {
                bullet.Position = position;
                bullet.Rotation = rotation;
            }
        }

        public void UpdateScore(long playerId, int score)
        {
            var jet = this.sceneJets.Find(jet => jet.OwnerId == playerId);
            jet.Score = score;
            Console.WriteLine("Score: " + jet.Score);
        }


    }
}
