using AtariJetFighter.Messages;
using AtariJetFighter.Scene;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net;

namespace AtariJetFighter.GameMachineObjects
{
    public class Client : DrawableGameComponent
    {
        /// <summary>
        /// NetClient instance.
        /// </summary>
        public NetClient NetClientInstance;
        /// <summary>
        /// Port where the player will connect on localhost.
        /// </summary>
        private int Port;
        /// <summary>
        /// Reference to the jetfighter game.
        /// </summary>
        private JetFighterGame GameInstance;
        /// <summary>
        /// Indicates whether Client is also a host.
        /// </summary>
        public bool IsHost;
        /// <summary>
        /// Reference to the game scene.
        /// </summary>
        public GameScene Scene;
        private bool WasConnected = false;

        public List<IPEndPoint> DiscoveredGames = new List<IPEndPoint>();     
        public Client(JetFighterGame game, int port, GameScene scene, bool isHost = false) : base((Game)game)
        {

            this.GameInstance = game;
            this.Scene = scene;
            this.IsHost = isHost;
            this.Port = port;
            NetPeerConfiguration config = new NetPeerConfiguration("AtariNetFighter");
            config.AutoFlushSendQueue = true;
            config.PingInterval = 1f;
            config.ConnectionTimeout = 1.5f;
            config.AcceptIncomingConnections = true;
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);


            NetClientInstance = new NetClient(config);

        }
        public void DiscoverGames()
        {
            for (int i = 0; i < 10; i++)
            {
                NetClientInstance.DiscoverLocalPeers(this.Port + i);
            }
        }
        public override void Initialize()
        {
            NetClientInstance.Start();
            NetOutgoingMessage hail = NetClientInstance.CreateMessage("This is the hail message");
            base.Initialize();
        }
        public void Connect(string address, int port)
        {
            NetOutgoingMessage hail = NetClientInstance.CreateMessage("This is the hail message");
            NetClientInstance.Connect(address, port, hail);
        }

        /// <summary>
        /// Client's update method send user input to the server and processes all messages received from the server.
        /// This method manipulates SceneObject
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (NetClientInstance.ConnectionStatus == NetConnectionStatus.Disconnected && this.GameInstance.GameState != GameStateEnum.Discovering)
            {
                if (WasConnected == true)
                {
                    this.GameInstance.GameState = GameStateEnum.Disconnected;
                }
            }
            else if (NetClientInstance.ConnectionStatus == NetConnectionStatus.Connected)
            {
                this.GameInstance.GameState = GameStateEnum.GameRunning;
                this.WasConnected = true;
            }
            ProcessMessages();
            SteerJet();

            base.Update(gameTime);
        }
        /// <summary>
        /// Process user input and send messages to the GameMachine.
        /// </summary>
        public void SteerJet()
        {
            if (InputController.keyIsPressed(Keys.Left) && InputController.keyIsPressed(Keys.Right))
            {
                // if both steering keys are pressed, dont do anything
                return;
            }
            if (InputController.keyIsPressed(Keys.Left))
            {
                SendUserControlMessage(Controls.Left);

            }
            else if (InputController.keyIsPressed(Keys.Right))
            {
                SendUserControlMessage(Controls.Right);
            }

            if (InputController.hasBeenPressed(Keys.Space))
            {
                SendUserControlMessage(Controls.Shoot);
            }
        }
        /// <summary>
        /// Sends user control messages to the server. 
        /// </summary>
        /// <param name="command"></param>
        public void SendUserControlMessage(Controls command)
        {
            //Console.WriteLine("Sending user control message: " + command.ToString());
            NetOutgoingMessage userControMessage = UserControlMessage.CreateMessage(this.NetClientInstance, command);
            NetClientInstance.SendMessage(userControMessage, NetDeliveryMethod.UnreliableSequenced);
        }

        /// <summary>
        /// Reads all the incomming messages from Server - GameMachine.
        /// </summary>
        private void ProcessMessages()
        {
            NetIncomingMessage message;
            while ((message = NetClientInstance.ReadMessage()) != null)
            {
                // handle incoming message
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = message.ReadString();
                        Console.Write(text);
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        bool isServer = message.ReadBoolean();
                        Console.WriteLine("Client: Discovery Response sender: " + message.SenderEndPoint);
                        if (isServer)
                        {
                            this.DiscoveredGames.Add(message.SenderEndPoint);
                        }
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        this.NetClientInstance.Connect(message.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                            Console.WriteLine("Client: Connected to the server");

                        if (status == NetConnectionStatus.Disconnected)
                            Console.WriteLine("Client: Disconnected from the server");
                        if (status == NetConnectionStatus.RespondedConnect)
                            Console.WriteLine("Client: Processing status message while responded connect");

                        string reason = message.ReadString();
                        Console.WriteLine(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        this.ProcessMessage(message);
                        break;
                    default:
                        Console.WriteLine("Client: Unhandled type: " + message.MessageType + " " + message.LengthBytes + " bytes");
                        break;
                }
                NetClientInstance.Recycle(message);
            }

        }
        /// <summary>
        /// Calls proper method for manipulationg GameScene
        /// </summary>
        /// <param name="message"></param>
        private void ProcessMessage(NetIncomingMessage message)
        {
            byte messageType = message.ReadByte();
            switch ((UpdateMessageType)messageType)
            {
                case UpdateMessageType.UpdateTransform:
                    {
                        this.ProcessTransformMessage(message);
                    }
                    break;

                case UpdateMessageType.SpawnPlayer:
                    {
                        this.ProcessNewPlayerMessage(message);

                    }
                    break;
                case UpdateMessageType.SpawnProjectile:
                    {
                        this.ProcessNewProjectileMessage(message);
                    }
                    break;

                case UpdateMessageType.DestroyObject:
                    {
                        ProcessDestroyObjectMessage(message);
                    }
                    break;
                case UpdateMessageType.UpdateScore:
                    {
                        ProcessScoreUpdateMessage(message);
                    }
                    break;

                case UpdateMessageType.RoundUpdate:
                    {
                        ProcessRoundUpdateMessage(message);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ProcessRoundUpdateMessage(NetIncomingMessage message)
        {
            bool roundInProgress = message.ReadBoolean();
            float gameTime = message.ReadFloat();

            if (this.Scene.roundInProgress != roundInProgress)
            {
                if (roundInProgress == false)
                {

                }
                if (roundInProgress == true)
                {
                    this.Scene.ResetLeaderBoard();
                }
            }

            this.Scene.timer = gameTime;
            this.Scene.roundInProgress = roundInProgress;
        }

        private void ProcessTransformMessage(NetIncomingMessage message)
        {
            byte objectId = message.ReadByte();
            float positionX = message.ReadFloat();
            float positionY = message.ReadFloat();
            float rotation = message.ReadFloat();
            this.Scene.UpdateObject(objectId, new Vector2(positionX, positionY), rotation);

        }

        private void ProcessNewPlayerMessage(NetIncomingMessage message)
        {
            Console.WriteLine("Process new player message ");
            byte objectId = message.ReadByte();
            long jetOwner = message.ReadInt64();
            float positionX = message.ReadFloat();
            float positionY = message.ReadFloat();
            float rotation = message.ReadFloat();
            byte color = message.ReadByte();

            this.Scene.AddJet(objectId, jetOwner, new Vector2(positionX, positionY), rotation, Constants.Colors[color], NetClientInstance.UniqueIdentifier == jetOwner);
        }

        private void ProcessDestroyObjectMessage(NetIncomingMessage message)
        {
            DestroyGameObjectMessage.UpdateScene(message, this.Scene);

        }

        private void ProcessNewProjectileMessage(NetIncomingMessage message)
        {
            Console.WriteLine("Process new projectile message ");
            byte objectId = message.ReadByte();
            float positionX = message.ReadFloat();
            float positionY = message.ReadFloat();
            float rotation = message.ReadFloat();
            byte color = message.ReadByte();

            this.Scene.AddBullet(objectId, new Vector2(positionX, positionY), rotation, Constants.Colors[color]);
        }


        private void ProcessScoreUpdateMessage(NetIncomingMessage message)
        {
            Console.WriteLine("Process score update message.");
            long playerId = message.ReadInt64();
            int score = message.ReadInt32();

            this.Scene.UpdateScore(playerId, score);
        }
    }

}
