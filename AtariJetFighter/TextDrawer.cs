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

        private string gameName = "ATARI .NET FIGHTER";
        private Vector2 gameNameLength;
        private string newGame = "Press [ N ] to host a new game";
        private Vector2 newGameLength;
        private string joinGame = "Press [ J ] to join a running game";
        private Vector2 joinGameLength;
        private string connecting = "Trying to connect to the server.";
        private Vector2 connectingLength;
        private string disconnected = "You had been disconnected from the server.";
        private Vector2 disconnectedLength;
        private string failedToConnect = "Client had failed to connect to the server after 5 second.\n" +
            "               Press [ ESC ] to return to main menu";
        private Vector2 failedToConnectLenght;
        private float instructionsScale = 0.5f;

        public TextDrawer(Game game, SpriteBatch spriteBatch, SpriteFont font) : base(game)
        {
            this.jfGame = (JetFighterGame)game;
            this.spriteBatch = spriteBatch;
            this.font = font;

            gameNameLength = font.MeasureString(gameName);
            Console.WriteLine(gameNameLength);
            newGameLength = font.MeasureString(newGame) * instructionsScale;
            joinGameLength = font.MeasureString(joinGame) * instructionsScale;
            connectingLength = font.MeasureString(connecting);
            disconnectedLength = font.MeasureString(disconnected);
            failedToConnectLenght = font.MeasureString(failedToConnect);

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            this.spriteBatch.Begin();
            switch (jfGame.GameState)
            {
                case GameMachineObjects.GameStateEnum.MainMenu:
                    DrawMenu();
                    break;
                case GameMachineObjects.GameStateEnum.GameRunning:
                    break;
                case GameMachineObjects.GameStateEnum.Disconnected:
                    DrawStringCenter(disconnected, disconnectedLength, 0.4f);
                    break;
                case GameMachineObjects.GameStateEnum.Connecting:
                    DrawStringCenter(connecting, connectingLength);
                    break;
                case GameMachineObjects.GameStateEnum.FailedToConnect:
                    DrawStringCenter(failedToConnect, failedToConnectLenght, 0.35f);
                    break;
                default:
                    break;
            }
            this.spriteBatch.End();
            base.Draw(gameTime);
        }

        public void DrawMenu()
        {

            this.jfGame.GraphicsDevice.Clear(Color.Black);
            spriteBatch.DrawString(
                font,
                gameName,
                new Vector2(((float)Constants.ScreenWidth - gameNameLength.X) / 2f, 150),
                Color.White,
                0.0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                1.0f);

            this.jfGame.GraphicsDevice.Clear(Color.Black);
            spriteBatch.DrawString(
                font,
                newGame,
                new Vector2(((float)Constants.ScreenWidth - newGameLength.X) / 2f, 380),
                Color.White,
                0.0f,
                Vector2.Zero,
                instructionsScale,
                SpriteEffects.None,
                1.0f);

            this.jfGame.GraphicsDevice.Clear(Color.Black);
            spriteBatch.DrawString(
                font,
                joinGame,
                new Vector2(((float)Constants.ScreenWidth - joinGameLength.X) / 2f, 450),
                Color.White,
                0.0f,
                Vector2.Zero,
                instructionsScale,
                SpriteEffects.None,
                1.0f);
        }

        private void DrawStringCenter(string text, Vector2 stringMeasurement, float scale = 0.5f)
        {
            Vector2 scaledMeasurement = stringMeasurement * scale;
            this.jfGame.GraphicsDevice.Clear(Color.Black);
            spriteBatch.DrawString(
                font,
                text,
                new Vector2(((float)Constants.ScreenWidth - scaledMeasurement.X) / 2f, ((float)Constants.ScreenHeight - scaledMeasurement.Y)/2f),
                Color.White,
                0.0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                1.0f);
        }
    }
}
