using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter
{
    internal class TextDrawer : DrawableGameComponent
    {

        private SpriteFont font;
        private SpriteBatch spriteBatch;
        private JetFighterGame jfGame;

        private string gameNameString = "ATARI .NET FIGHTER";
        private Vector2 gameNameStringLength;
        private string newGameString = "Press [ N ] to host a new game";
        private Vector2 newGameStringLength;
        private string joinGameString = "Press [ J ] to join a running game";
        private Vector2 joinGameStringLength;




        public TextDrawer(Game game, SpriteBatch spriteBatch, SpriteFont font) : base(game)
        {
            this.jfGame = (JetFighterGame)game;
            this.spriteBatch = spriteBatch;
            this.font = font;

            gameNameStringLength = font.MeasureString(gameNameString);
            Console.WriteLine(gameNameStringLength);
            newGameStringLength = font.MeasureString(newGameString) * Constants.inctructionsScale;
            joinGameStringLength = font.MeasureString(joinGameString) * Constants.inctructionsScale; ;
        }

        public override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Begin();
            if (jfGame.GameState == GameMachineObjects.GameStateEnum.MainMenu)
            {
                
                this.jfGame.GraphicsDevice.Clear(Color.Black);
                spriteBatch.DrawString(
                    font,
                    gameNameString,
                    new Vector2(((float)Constants.ScreenWidth - gameNameStringLength.X)/ 2f, 150),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    1.0f);

                this.jfGame.GraphicsDevice.Clear(Color.Black);
                spriteBatch.DrawString(
                    font,
                    newGameString,
                    new Vector2(((float)Constants.ScreenWidth - newGameStringLength.X) / 2f, 380),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    Constants.inctructionsScale,
                    SpriteEffects.None,
                    1.0f);

                this.jfGame.GraphicsDevice.Clear(Color.Black);
                spriteBatch.DrawString(
                    font,
                    joinGameString,
                    new Vector2(((float)Constants.ScreenWidth - joinGameStringLength.X) / 2f, 450),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    Constants.inctructionsScale,
                    SpriteEffects.None,
                    1.0f);
            }
            this.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
