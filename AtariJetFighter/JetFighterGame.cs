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
        public Client client;
        public SpriteFont font;
        public Texture2D jet;
        public Texture2D bullet;
        public Texture2D collider;
        public GameScene scene;
        public bool sceneInitialized = false;

        public JetFighterGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        /// <summary>
        /// Initialize Monogane graphics.
        /// </summary>
        protected override void Initialize()
        {
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = Constants.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Constants.ScreenHeight;
            this.IsMouseVisible = false;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        /// <summary>
        /// Load sprites and spritefonts.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            jet = Content.Load<Texture2D>("jet");
            bullet = Content.Load<Texture2D>("bullet");
            collider = Content.Load<Texture2D>("collider");
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

                case GameStateEnum.Discovering:
                    { 
                        if (InputController.hasBeenPressed(Keys.NumPad0))
                        {
                            this.client.Connect(this.client.DiscoveredGames[0].Address.ToString(), this.client.DiscoveredGames[0].Port);
                        }
                        if (InputController.hasBeenPressed(Keys.NumPad1))
                        {
                            this.client.Connect(this.client.DiscoveredGames[1].Address.ToString(), this.client.DiscoveredGames[1].Port);
                        }

                        if (InputController.hasBeenPressed(Keys.NumPad2))
                        {
                            this.client.Connect(this.client.DiscoveredGames[2].Address.ToString(), this.client.DiscoveredGames[2].Port);
                        }
                        if (InputController.hasBeenPressed(Keys.NumPad3))
                        {
                            this.client.Connect(this.client.DiscoveredGames[3].Address.ToString(), this.client.DiscoveredGames[3].Port);
                        }
                        if (InputController.hasBeenPressed(Keys.NumPad4))
                        {
                            this.client.Connect(this.client.DiscoveredGames[4].Address.ToString(), this.client.DiscoveredGames[4].Port);
                        }

                        if (InputController.hasBeenPressed(Keys.Escape))
                        {
                            ReturntoMainMenu();
                        }
                    }
                    break;
                case GameStateEnum.GameRunning:
                case GameStateEnum.Disconnected:
                    if (InputController.hasBeenPressed(Keys.Escape))
                    {
                        ReturntoMainMenu();
                    }
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
        /// <summary>
        /// Disable Game machine, client and return to Main menu.
        /// </summary>
        private void ReturntoMainMenu()
        {
            this.Components.Remove(scene);
            this.scene = null;
            this.Components.Remove(client);

            client.NetClientInstance.Disconnect("bye");
            if (client.IsHost)
            {
                this.gameMachine.Stop();
                this.Components.Remove(this.gameMachine);
                this.gameMachine = null;
            }
            this.client = null;
            this.GameState = GameStateEnum.MainMenu;
        }

        /// <summary>
        /// Start a game machaine locally and connect to it.
        /// </summary>
        private void HostGame()
        {
            this.gameMachine = new GameMachine(this, 14242);
            this.Components.Add(this.gameMachine);
            JoinGame(true);
        }
        /// <summary>
        /// Connect to hosts gameMachine.
        /// </summary>
        /// <param name="isHost"></param>
        private void JoinGame(bool isHost = false)
        {
            this.scene = new GameScene(this);
            this.Components.Add(this.scene);
            this.client = new Client(this, 14242, this.scene, isHost);
            this.Components.Add(this.client);

            if (isHost)
            {
                this.client.Connect("localhost", this.gameMachine.Port);
            }
            else
            {
                this.client.DiscoverGames();
                this.GameState = GameStateEnum.Discovering;
            }

            sceneInitialized = true;
        }
    }
}
