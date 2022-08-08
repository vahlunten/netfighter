using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter
{
    /// <summary>
    /// Text Drawer is responsible for drawing "UI" elements while game is not running.
    /// </summary>
    internal class TextDrawer : DrawableGameComponent
    {

        private SpriteFont font;
        private SpriteBatch spriteBatch;
        private JetFighterGame jfGame;
        
        private string gameName = "ATARI .NET FIGHTER";
        private Vector2 gameNameDimensions;
        private string newGame = "Press [ N ] to host a new game";
        private Vector2 newGameDimensions;
        private string joinGame = "Press [ J ] to join a running game";
        private Vector2 joinGameDimensions;
        private string connecting = "Trying to connect to the server.";
        private Vector2 connectingDimensions;
        private string disconnected = "You had been disconnected from the server.";
        private Vector2 disconnectedDimensions;
        private string failedToConnect = "Failed to connect to the server after 5 second.";
        private Vector2 failedToConnectDimensions;
        private string returnToMenu = "Press [ ESC ] to return to main menu";
        private Vector2 returnToMenuDimensions;
        private string exitGame = "Press [ ESC ] to quit game";
        private Vector2 exitGameDimensions;

        /// <summary>
        /// Constructor calculates dimensions of all the strings. 
        /// </summary>
        /// <param name="game">Instance of the game.</param>
        /// <param name="spriteBatch">Instance of game's SpriteBatch.</param>
        /// <param name="font">Font sprite.</param>
        public TextDrawer(Game game, SpriteBatch spriteBatch, SpriteFont font) : base(game)
        {
            this.jfGame = (JetFighterGame)game;
            this.spriteBatch = spriteBatch;
            this.font = font;

            gameNameDimensions = font.MeasureString(gameName);
            Console.WriteLine(gameNameDimensions);
            newGameDimensions = font.MeasureString(newGame);
            joinGameDimensions = font.MeasureString(joinGame);
            connectingDimensions = font.MeasureString(connecting);
            disconnectedDimensions = font.MeasureString(disconnected);
            failedToConnectDimensions = font.MeasureString(failedToConnect);
            returnToMenuDimensions = font.MeasureString(returnToMenu);
            exitGameDimensions = font.MeasureString(exitGame);
        }

        /// <summary>
        /// Draws UI on the screen. If the game is not Running.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            this.spriteBatch.Begin();
            switch (jfGame.GameState)
            {
                case GameMachineObjects.GameStateEnum.MainMenu:
                    {
                        DrawStringCenter(gameName, gameNameDimensions, 1.0f, -200f);
                        DrawStringCenter(newGame, newGameDimensions, 0.5f);
                        DrawStringCenter(joinGame, joinGameDimensions, 0.5f, 50);
                        DrawStringCenter(exitGame, exitGameDimensions, 0.5f, 350);
                    }
                    break;
                case GameMachineObjects.GameStateEnum.GameRunning:
                    break;
                case GameMachineObjects.GameStateEnum.Disconnected:
                    {
                        DrawStringCenter(disconnected, disconnectedDimensions, 0.4f);
                        DrawStringCenter(returnToMenu, returnToMenuDimensions, 0.5f, 350f);
                    }                    
                    break;
                case GameMachineObjects.GameStateEnum.Discovering:
                    DrawDiscoveredGames();
                    break;
                default:
                    break;
            }
            this.spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draw string horizontally and vertically in the middle of the canvas. 
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="stringMeasurement">Pre-calculated string dimensions.</param>
        /// <param name="scale">Scale of displayed text.</param>
        /// <param name="verticalOffset">Vertical offset to shift string up and down if needed.</param>
        private void DrawStringCenter(string text, Vector2 stringMeasurement, float scale = 0.5f, float verticalOffset = 0)
        {
            Vector2 scaledMeasurement = stringMeasurement * scale;
            this.jfGame.GraphicsDevice.Clear(Color.Black);
            spriteBatch.DrawString(
                font,
                text,
                new Vector2(((float)Constants.ScreenWidth - scaledMeasurement.X) / 2f, ((float)Constants.ScreenHeight - scaledMeasurement.Y) / 2f + verticalOffset),
                Color.White,
                0.0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                1.0f);
        }

        private void DrawDiscoveredGames()
        {
            string chooseGameString = "Press numerical key with [index] oy game";

            this.DrawStringCenter(chooseGameString, font.MeasureString(chooseGameString), 0.7f, -200f);
            for (int i = 0; i < this.jfGame.client.DiscoveredGames.Count; i++)
            {
                var game = this.jfGame.client.DiscoveredGames[i];
                this.DrawStringCenter($"[{i}]" + game.ToString(), font.MeasureString(game.ToString()), 0.5f, i * 50f);
            }
        }
    }
}
