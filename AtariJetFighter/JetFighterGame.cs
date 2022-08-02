﻿using AtariJetFighter.GameMachineObjects;
using AtariJetFighter.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AtariJetFighter
{
    /// <summary>
    /// Jet fighter game object. This object holds references to all components of the game, sprites. 
    /// </summary>
    public class JetFighterGame : Game
    {
        private GraphicsDeviceManager _graphics;
        public SpriteBatch spriteBatch;
        public GameStateEnum GameState;

        private GameMachine gameMachine;
        private Client client;
        public SpriteFont font;
        public Texture2D jet;
        public Texture2D bullet;
        public GameScene scene;
        public bool sceneInitialized = false;

        public JetFighterGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
           
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            this.IsMouseVisible = false;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            jet = Content.Load<Texture2D>("jet");
            bullet = Content.Load<Texture2D>("bullet");
            Components.Add(new TextDrawer(this, spriteBatch, font));
        }

        /// <summary>
        /// Update methods handles keypressed that are related to changing state of the game. 
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            InputController.UpdateKeyboardState();

            switch (GameState)
            {
                case GameStateEnum.MainMenu:

                    if (InputController.hasBeenPressed(Keys.Escape))
                        Exit();

                    // start as server 
                    if (InputController.hasBeenPressed(Keys.N))
                    {
                        Console.WriteLine("Key N has been Pressed, starting gamemachine");
                        this.HostGame();
                    }
                    // join as a client
                    if (InputController.hasBeenPressed(Keys.J))
                    {
                        Console.WriteLine("Key J has been Pressed, connecting as a client");
                        this.JoinGame();
                    }
                        break;
                case GameStateEnum.GameRunning:
                    if (InputController.hasBeenPressed(Keys.Escape))
                        ReturntoMainMenu();
                    break;
                default:
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        private void ReturntoMainMenu()
        {
            this.GameState = GameStateEnum.MainMenu;
            // TODO: Properly stop server peer, notify all players about server disconnected
        }

        private void HostGame()
        {
            this.gameMachine = new GameMachine(this, 14242);
            this.Components.Add(this.gameMachine);
            JoinGame();
        }
        private void JoinGame()
        {
            this.scene = new GameScene(this);
            this.Components.Add(this.scene);

            this.client = new Client(this, 14242);
            this.Components.Add(this.client);

            sceneInitialized = true;
            GameState = GameStateEnum.GameRunning;
        }
    }
}
