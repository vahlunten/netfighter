using AtariJetFighter.Messages;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace AtariJetFighter.GameMachineObjects
{
    class Client : DrawableGameComponent
    {
        public NetClient netClient;
        private int port;
        private JetFighterGame game;
        public bool isHost;
        private float connectToServerTimeout = 5.0f;
        private bool wasConnected = false;

        public Client(JetFighterGame game, int port, bool isHost = false) : base((Game)game)
        {
            this.port = port;
            this.game = game;
            this.isHost = isHost;
            NetPeerConfiguration config = new NetPeerConfiguration("AtariNetFighter");
            config.AutoFlushSendQueue = true;
            config.PingInterval = 1f;
            config.ConnectionTimeout = 1.5f;
            netClient = new NetClient(config);
        }
        public override void Initialize()
        {
            netClient.Start();
            NetOutgoingMessage hail = netClient.CreateMessage("This is the hail message");
            netClient.Connect("localhost", this.port, hail);
            this.game.GameState = GameStateEnum.Connecting;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if(netClient.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                if (wasConnected == false)
                {
                    // update initial connection timeout
                    this.connectToServerTimeout -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (connectToServerTimeout < 0)
                    {
                        this.game.GameState = GameStateEnum.FailedToConnect;
                    }
                    return;
                } else
                {
                    this.game.GameState = GameStateEnum.Disconnected;
                }
            }
            else if (netClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                this.game.GameState = GameStateEnum.GameRunning;
                this.wasConnected = true;
            }
            ProcessMessages();
            SteerJet();

            base.Update(gameTime);
        }
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
        public void SendUserControlMessage(Controls command)
        {
            //Console.WriteLine("Sending user control message: " + command.ToString());
            NetOutgoingMessage userControMessage = UserControlMessage.CreateMessage(this.netClient, command);
            netClient.SendMessage(userControMessage, NetDeliveryMethod.UnreliableSequenced);
        }

        private void ProcessMessages()
        {
            NetIncomingMessage message;
            while ((message = netClient.ReadMessage()) != null)
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
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                            Console.WriteLine("Client: Connected to the server");

                        if (status == NetConnectionStatus.Disconnected)
                            Console.WriteLine("Client: Disconnected from the server");

                        string reason = message.ReadString();
                        Console.WriteLine(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        if (this.game.sceneInitialized)
                        {

                        }
                        this.ProcessMessage(message);
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + message.MessageType + " " + message.LengthBytes + " bytes");
                        break;
                }
                netClient.Recycle(message);
            }

        }
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

                    case UpdateMessageType.DestroyPlayer: 
                        {
                        ProcessDestroyObjectMessage(message, UpdateMessageType.DestroyPlayer);
                    }
                    break;
                    case UpdateMessageType.DestroyProjectile:
                    {
                        ProcessDestroyObjectMessage(message, UpdateMessageType.DestroyProjectile);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ProcessTransformMessage(NetIncomingMessage message)
        {
            byte objectId = message.ReadByte();
            float positionX = message.ReadFloat();
            float positionY = message.ReadFloat();
            float rotation = message.ReadFloat();
            this.game.scene.UpdateJet(objectId, new Vector2(positionX, positionY), rotation);
            this.game.scene.UpdateBullet(objectId, new Vector2(positionX, positionY), rotation);

        }

        private void ProcessNewPlayerMessage(NetIncomingMessage message)
        {
            Console.WriteLine("Process new player message ");
            byte objectId = message.ReadByte();
            long jetOwner = message.ReadInt64();
            float positionX = message.ReadFloat();
            float positionY = message.ReadFloat();
            float rotation = message.ReadFloat();

            this.game.scene.AddJet(objectId, jetOwner, new Vector2(positionX, positionY), rotation, Color.White);
        }

        private void ProcessDestroyObjectMessage(NetIncomingMessage message, UpdateMessageType type)
        {
            Console.WriteLine("Process destroy object message ");

            byte objectId = message.ReadByte();
            if (type == UpdateMessageType.DestroyPlayer)
            {
                this.game.scene.RemoveJet(objectId);
            } else
            {
                this.game.scene.RemoveBullet(objectId);

            }


        }

        private void ProcessNewProjectileMessage(NetIncomingMessage message)
        {
            Console.WriteLine("Process new projectile message ");
            byte objectId = message.ReadByte();
            float positionX = message.ReadFloat();
            float positionY = message.ReadFloat();
            float rotation = message.ReadFloat();

            this.game.scene.AddBullet(objectId, new Vector2(positionX, positionY), rotation, Color.White);
        }
    }

}
