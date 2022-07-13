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
        private Vector2 position = new Vector2(400f, 400f);
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
            this.game.spriteBatch.Begin();
            DrawPlayers();
            this.game.spriteBatch.End();
            base.Draw(gameTime);
            
        }

        public void DrawPlayers()
        {
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

        public void UpdateJet()
        {
            this.position += new Vector2(10f, 0f) ;
            if (position.X > 800)
            {
                position.X = 0f;
            }
        }


    }
}
