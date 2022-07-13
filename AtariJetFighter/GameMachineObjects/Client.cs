using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.GameMachineObjects
{
    class Client: DrawableGameComponent
    {
        private NetClient netClient;
        private int port;
        private double elapsed = 500;
        private JetFighterGame game;

        public Client( JetFighterGame game, int port):base((Game)game)
        {
            this.port = port;
            this.game = game;
            NetPeerConfiguration config = new NetPeerConfiguration("chat");
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


            NetIncomingMessage im;
            while ((im = netClient.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        Console.Write(text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                            Console.WriteLine("Connected");

                        if (status == NetConnectionStatus.Disconnected)
                            Console.WriteLine("DisConnected");

                        string reason = im.ReadString();
                        Console.WriteLine(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        string chat = im.ReadString();
                        if (this.game.sceneInitialized)
                        {

                        }
                        this.game.scene.UpdateJet();
                        //Console.WriteLine("Client received this message: " + 
                        //    chat);
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                netClient.Recycle(im);
            }

            
            //elapsed = elapsed - gameTime.ElapsedGameTime.TotalMilliseconds;
            //if(elapsed < 0)
            //{
            //    Console.WriteLine("Sending Message ");
            //    NetOutgoingMessage message = netClient.CreateMessage("mama moja ");
            //    netClient.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            //    elapsed = 5000;
            //}
            base.Update(gameTime);
        }
        public void sendMessage(string messageText)
        {
            NetOutgoingMessage message = netClient.CreateMessage($"Message from client { messageText}");
            netClient.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }
    }
    
}
