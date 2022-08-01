using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter
{
    public class Scene : DrawableGameComponent
    {
        public bool isInitialized = false;
        private JetFighterGame game;
        private Vector2 position = new Vector2(200f, 200f);
        private float rotation = 0f;

        public Scene(JetFighterGame game) : base((Game) game)
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
            GraphicsDevice.Clear(Color.Black);
            this.game.spriteBatch.Begin();
            DrawPlayers();
            this.game.spriteBatch.End();
            base.Draw(gameTime);
            
        }

        public void DrawPlayers()
        {
            //Console.WriteLine("Coordinates: " + this.position);
            this.game.spriteBatch.Draw(this.game.jet, this.position, Color.White);
        }

        public void DrawScores()
        {

        }
        public void DrawBullets()
        {

        }

        public void AddPlayer()
        {

        }
        public void AddBullet()
        {

        }

        public void UpdateJet(Vector2 position, float rotation)
        {
            this.position = position;
            if (position.X > 800)
            {
                position.X = 0f;
            }
            this.rotation = rotation;
        }


    }
}
