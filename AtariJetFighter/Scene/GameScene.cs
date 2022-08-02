using AtariJetFighter;
using AtariJetFighter.Scene.SceneObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.Scene
{
    /// <summary>
    /// Scene object holds all the necessary data to by displayed on player's screen while game is running.
    /// </summary>
    public class GameScene : DrawableGameComponent
    {
        public bool isInitialized = false;
        private JetFighterGame game;

        private List<SceneJet> sceneJets = new List<SceneJet>();
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
            isInitialized = true;
            base.Initialize();
        }
        
        public override void Draw(GameTime gameTime)
        {
           if(game.GameState == GameMachineObjects.GameStateEnum.GameRunning)
            {
                GraphicsDevice.Clear(Color.Black);
                this.game.spriteBatch.Begin();
                DrawPlayers();
                DrawBullets();
                this.game.spriteBatch.End();
                base.Draw(gameTime);
            }
            
        }

        public void DrawPlayers()
        {
            //Console.WriteLine("Coordinates: " + this.position);
            //this.game.spriteBatch.Draw(this.game.jet, this.position, Color.White);

            foreach (var sceneJet in sceneJets)
            {
                //this.game.spriteBatch.Draw(this.game.jet, sceneJet.Position, Color.White);
                //this.game.spriteBatch.Draw(this.game.jet, sceneJet.Position, null, Color.White, sceneJet.Rotation, new Vector2(0), SpriteEffects.None, 1.0f);
                this.game.spriteBatch.Draw(this.game.jet, sceneJet.Position, null, Color.White, sceneJet.Rotation + (float)Math.PI/2f, new Vector2(32,32), new Vector2(1), SpriteEffects.None, 0.50f);



            }
        }

        public void DrawBullets()
        {
            foreach (var bullet in sceneBullets)
            {
                //this.game.spriteBatch.Draw(this.game.jet, sceneJet.Position, Color.White);
                //this.game.spriteBatch.Draw(this.game.jet, sceneJet.Position, null, Color.White, sceneJet.Rotation, new Vector2(0), SpriteEffects.None, 1.0f);
                this.game.spriteBatch.Draw(this.game.bullet, bullet.Position, null, Color.White, bullet.Rotation + (float)Math.PI / 2f, new Vector2(32, 32), new Vector2(1), SpriteEffects.None, 0.50f);



            }
        }

        public void DrawScores()
        {

        }

        public void AddJet(byte objectId, long ownerId,  Vector2 position, float rotation, Color color, int score = 0)
        {
            if(sceneJets.Find(jet => jet.ObjectId == objectId) == null)
            {
                this.sceneJets.Add(new SceneJet(objectId, ownerId, position, rotation, color));
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


    }
}
