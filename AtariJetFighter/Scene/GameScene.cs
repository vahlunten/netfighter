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

        public bool roundInProgress = true;
        public float timer = 0;
        private List<SceneJet> LeaderBoard;

        public GameScene(JetFighterGame game) : base((Game)game)
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
            if (game.GameState == GameMachineObjects.GameStateEnum.GameRunning)
            {
                GraphicsDevice.Clear(Color.Black);
                this.game.spriteBatch.Begin();
                DrawPlayers();
                DrawBullets();
                DrawScores();
                DrawTimer();
                if (!roundInProgress)
                {
                    DrawLeaderboard();
                }
                this.game.spriteBatch.End();
                base.Draw(gameTime);
            }

        }
        /// <summary>
        /// When the roudn is over, this method is used to draw leaderboard on the screen.
        /// </summary>
        private void DrawLeaderboard()
        {

            if (this.LeaderBoard == null)
            {
                this.ComputeLeaderBoard();
            }

            for (int i = 0; i < LeaderBoard.Count; i++)
            {
                var jetEntry = this.LeaderBoard[i];
                string entryString = jetEntry.IsLocal ? $"{i}. { Constants.ColorNames[jetEntry.color]} jet (YOU)" : $"{i}. {Constants.ColorNames[jetEntry.color]} jet";
                this.game.spriteBatch.DrawString(
                this.game.font,
                entryString,
                new Vector2(80f, 200f + i * 50f),
                jetEntry.color,
                0.0f,
                Vector2.Zero,
                0.8f,
                SpriteEffects.None,
                0.5f);
            }
            
        }
        /// <summary>
        /// Generate LeaderBoard
        /// </summary>
        private void ComputeLeaderBoard()
        {
            this.LeaderBoard = this.sceneJets.OrderByDescending(jet => jet.Score).ToList();
        }
        public void ResetLeaderBoard()
        {
            this.LeaderBoard = null;
        }
        /// <summary>
        /// Draw round/ next round start time left.
        /// </summary>
        private void DrawTimer()
        {
            this.game.spriteBatch.DrawString(
              this.game.font,
              timer.ToString(),
              new Vector2(300, 10),
              this.roundInProgress? Color.White : Color.Red,
              0.0f,
              Vector2.Zero,
              0.6f,
              SpriteEffects.None,
              0.5f);
        }
        /// <summary>
        /// Draw jets of all players.
        /// </summary>
        public void DrawPlayers()
        {
            foreach (var sceneJet in sceneJets)
            {
                this.game.spriteBatch.Draw(this.game.jet, sceneJet.Position, null, sceneJet.color, sceneJet.Rotation, new Vector2(32), new Vector2(1), SpriteEffects.None, 0.50f);

                // draw collider
                //this.game.spriteBatch.Draw(this.game.collider, sceneJet.Position, null, Color.White, 0f, new Vector2(22.5f), new Vector2(1), SpriteEffects.None, 0.50f);

            }
        }
        /// <summary>
        /// Draw all currently existing bullets.
        /// </summary>
        public void DrawBullets()
        {
            foreach (var bullet in sceneBullets)
            {
                this.game.spriteBatch.Draw(this.game.bullet, bullet.Position, null, bullet.color, bullet.Rotation, new Vector2(7.5f), new Vector2(1), SpriteEffects.None, 0.50f);
            }
        }
        /// <summary>
        /// Draw user's scores in the row on top side of the screen. 
        /// TODO: Make more than 5 fit into screen :'(
        /// </summary>
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
                isLocal ? $"{score} (YOU) " : $" {score}",
                new Vector2(50 + horizontalOffset, verticalOffset),
                color,
                0.0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.5f);
        }

        /// <summary>
        /// Add new jet to the scene.
        /// </summary>
        public void AddJet(byte objectId, long ownerId, Vector2 position, float rotation, Color color, bool isLocal, int score = 0)
        {
            if (FindJetByObjectId(objectId) == null)
            {
                this.sceneJets.Add(new SceneJet(objectId, ownerId, position, rotation, color, isLocal));
            }
        }
        /// <summary>
        /// Add new bullet to the scene
        /// </summary>
        public void AddBullet(byte objectId, Vector2 position, float rotation, Color color)
        {
            if (FindBulletByObjectId(objectId) == null)
            {
                this.sceneBullets.Add(new SceneBullet(objectId, position, rotation, color));
            }
        }
        /// <summary>
        /// Removes jet when player disconnects or when simulated jet is disabled.
        /// </summary>
        /// <param name="objectId"></param>
        public void RemoveJet(byte objectId)
        {
            var removedJet = FindJetByObjectId(objectId);
            if (removedJet != null)
            {
                this.sceneJets.Remove(removedJet);
            }
        }
        /// <summary>
        /// Remove expired bullets from scene. Bullets lifespan has expired or it hit some jet.
        /// </summary>
        /// <param name="objectId"></param>
        public void RemoveBullet(byte objectId)
        {
            var removedBullet = FindBulletByObjectId(objectId);
            if (removedBullet != null)
            {
                this.sceneBullets.Remove(removedBullet);
            }
        }
        /// <summary>
        /// Update jets positon and rotation
        /// </summary>
        /// <param name="objectId">Jet object identifier.</param>
        /// <param name="position">Jets new position.</param>
        /// <param name="rotation">Jets new rotation.</param>
        public void UpdateJet(byte objectId, Vector2 position, float rotation)
        {
            var jet = FindJetByObjectId(objectId);
            if (jet == null)
            {
                return;
            }
            else
            {
                jet.Position = position;
                jet.Rotation = rotation;
            }
        }
        /// <summary>
        /// Update bullets positon and rotation
        /// </summary>
        /// <param name="objectId">bullets object identifier.</param>
        /// <param name="position">bullets new position.</param>
        /// <param name="rotation">bullets new rotation.</param>
        public void UpdateBullet(byte objectId, Vector2 position, float rotation)
        {
            var bullet = FindBulletByObjectId(objectId);
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
        /// <summary>
        /// Update player score, occurs when player hits some other player or when game restarts.
        /// </summary>
        /// <param name="playerId">Player identifier.</param>
        /// <param name="score">New score</param>
        public void UpdateScore(long playerId, int score)
        {
            var jet = FindJetByPlayerId(playerId);
            jet.Score = score;
            Console.WriteLine("Score: " + jet.Score);
        }

        private SceneJet FindJetByObjectId(byte objectId)
        {
            return this.sceneJets.Find(jet => jet.ObjectId == objectId);
        }
        private SceneJet FindJetByPlayerId(long playerId)
        {
            return this.sceneJets.Find(jet => jet.OwnerId == playerId);
        }
        private SceneBullet FindBulletByObjectId(byte objectId)
        {
            return this.sceneBullets.Find(bullet => bullet.ObjectId == objectId);
        }
    }
}
