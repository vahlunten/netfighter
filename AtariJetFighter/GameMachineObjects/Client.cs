﻿using AtariJetFighter.Messages;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace AtariJetFighter.GameMachineObjects
{
    class Client : DrawableGameComponent
    {
        private NetClient netClient;
        private int port;
        private JetFighterGame game;

        public Client(JetFighterGame game, int port) : base((Game)game)
        {
            this.port = port;
            this.game = game;
            NetPeerConfiguration config = new NetPeerConfiguration("AtariNetFighter");
            config.AutoFlushSendQueue = true;
            netClient = new NetClient(config);
        }
        public override void Initialize()
        {
            netClient.Start();
            NetOutgoingMessage hail = netClient.CreateMessage("This is the hail message");
            netClient.Connect("localhost", this.port, hail);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
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
