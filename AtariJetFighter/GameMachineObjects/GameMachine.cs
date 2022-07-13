using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtariJetFighter.GameMachineObjects
{
    internal class GameMachine : DrawableGameComponent
    {

        private NetServer serverPeer;
		public bool isInitialized = false;
        public GameMachine(Game game, int port): base(game)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("chat");
            config.MaximumConnections = 100;
            config.Port = port;

            this.serverPeer = new NetServer(config);

        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

       
        public override void Initialize()
        {
			serverPeer.Start();
			isInitialized = true;
            base.Initialize();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void Update(GameTime gameTime)
        {
			NetIncomingMessage im;
			while ((im = serverPeer.ReadMessage()) != null)
			{
				// handle incoming message
				switch (im.MessageType)
				{
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.ErrorMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.VerboseDebugMessage:
						string text = im.ReadString();
						//Output(text);
						break;

					case NetIncomingMessageType.StatusChanged:
						NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

						string reason = im.ReadString();
						//Output(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

						if (status == NetConnectionStatus.Connected)
							Console.WriteLine("Remote hail: " + im.SenderConnection.RemoteHailMessage.ReadString());

						//UpdateConnectionsList();
						break;
					case NetIncomingMessageType.Data:
						// incoming chat message from a client
						string chat = im.ReadString();
						Console.WriteLine("Sending message: " + chat);

						//Output("Broadcasting '" + chat + "'");

						// broadcast this to all connections
						NetOutgoingMessage om = serverPeer.CreateMessage();
						om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
						serverPeer.SendMessage(om, serverPeer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
						//List<NetConnection> all = serverPeer.Connections; // get copy
						//all.Remove(im.SenderConnection);

						//if (all.Count > 0)
						//{
						//	NetOutgoingMessage om = serverPeer.CreateMessage();
						//	om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
						//	serverPeer.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
						//}
						break;
					default:
						Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
						break;
				}
				serverPeer.Recycle(im);
			}
			base.Update(gameTime);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
    }
}
