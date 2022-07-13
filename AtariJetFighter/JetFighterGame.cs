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
            jet = Content.Load<Texture2D>("player knife");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            InputController.UpdateKeyboardState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            

            switch (GameState)
            {
                case GameStateEnum.MainMenu:

                    // start as server 
                    if (InputController.hasBeenPressed(Keys.N))
                    {
                        Console.WriteLine("Key N has been Pressed, starting gamemachine");
                        // start new game 
                        this.gameMachine = new GameMachine(this, 14242);
                        this.Components.Add(this.gameMachine);

                        //this.client = new Client(this, 14242);
                        //this.Components.Add(this.client);
                        //GameState = GameStateEnum.GameRunning;
                    }

                    if (InputController.hasBeenPressed(Keys.J))
                    {
                        this.scene = new Scene(this);
                        this.Components.Add(this.scene);

                        this.client = new Client(this, 14242);
                        this.Components.Add(this.client);
                        sceneInitialized = true;

                        

                        GameState = GameStateEnum.GameRunning;
                    }

                        //start as client
                        break;
                case GameStateEnum.GameRunning:
                    if (Keyboard.GetState().IsKeyDown(Keys.X))
                    {
                        this.client.sendMessage("pes");
                    }
                        break;
                case GameStateEnum.Disconnected:
                    break;
                default:
                    break;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "SCORE: 235", Vector2.Zero, Color.Black, 0.0f, Vector2.Zero,1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
