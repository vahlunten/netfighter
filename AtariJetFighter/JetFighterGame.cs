using AtariJetFighter.GameMachineObjects;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AtariJetFighter
{
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
        public Scene scene;
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
            this.Components.Add(new TextDrawer(this, spriteBatch, font));


            // TODO: use this.Content to load your game content here
        }

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

                    if (InputController.hasBeenPressed(Keys.J))
                    {
                        this.JoinGame();
                    }
                        break;
                case GameStateEnum.GameRunning:

                    if (InputController.hasBeenPressed(Keys.Escape))
                        ReturntoMainMenu();
                    if (Keyboard.GetState().IsKeyDown(Keys.X))
                    {
                        //this.client.sendMessage("pes");
                    }
                        break;
                case GameStateEnum.Disconnected:
                    GameState = GameStateEnum.Disconnected;
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
        }

        private void HostGame()
        {
            this.gameMachine = new GameMachine(this, 14242);
            this.Components.Add(this.gameMachine);

            this.scene = new Scene(this);
            this.Components.Add(this.scene);

            JoinGame();
        }
        private void JoinGame()
        {
            this.scene = new Scene(this);
            this.Components.Add(this.scene);

            this.client = new Client(this, 14242);
            this.Components.Add(this.client);

            sceneInitialized = true;
            GameState = GameStateEnum.GameRunning;
        }
    }
}
