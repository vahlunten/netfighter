using AtariJetFighter.GameEngine.GameObjects;
using AtariJetFighter.Networking;
using AtariJetFighter.Networking.Messages;
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
		private List<NetConnection> connectedPlayers;
		private Random RandomNumberGenerator = new Random();


		private List<Jet> Jets;
		private List<Bullet> Bullets;

        public GameMachine(Game game, int port): base(game)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("AtariNetFighter");
            config.MaximumConnections = 10;
            config.Port = port;

			connectedPlayers = new List<NetConnection>();
			this.Jets = new List<Jet>();
			//var testJet = new Jet((long)0, (byte)23, new Vector2(400f, 400f), 0f);
			this.Bullets = new List<Bullet>();

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
			processMessages(gameTime);
			CheckForCollisions();

			base.Update(gameTime);
        }

        private void CheckForCollisions()
        {
            // check for collisions
			// 
        }

        private void processMessages(GameTime gameTime)
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
						connectedPlayers.Add(im.SenderConnection);
						this.SpawnJet(im.SenderConnection.RemoteUniqueIdentifier);

						//UpdateConnectionsList();
						break;
					case NetIncomingMessageType.Data:
						// incoming chat message from a client
						//string chat = im.ReadString();
						//Console.WriteLine("Sending message: " + chat);

						//Output("Broadcasting '" + chat + "'");

						// broadcast this to all connections
						//NetOutgoingMessage om = serverPeer.CreateMessage();
						//om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier));
						//serverPeer.SendMessage(om, serverPeer.Connections, NetDeliveryMethod.ReliableOrdered, 0);



						ProcessMessage(im, gameTime);


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
		}

		private void ProcessMessage(NetIncomingMessage message, GameTime gameTime)
		{
			byte messageType = message.ReadByte();
			switch (messageType)
			{
				case (byte)UpdateMessageType.UserControl:
					{
						this.ProcessSteeringMessage(message, gameTime);
						break;
					}

				default:
					break;
			}
		}

		private void ProcessSteeringMessage(NetIncomingMessage message, GameTime gameTime)
        {
			//message.Write((byte)UpdateMessageType.UserControl);
			//message.Write((byte)command);
			byte direction = message.ReadByte();

			this.Jets[0].Update(gameTime, (Controls)direction);
			SendUpdateTransformMessage();


		}

		private void SendUpdateTransformMessage()
        {
			NetOutgoingMessage om = serverPeer.CreateMessage();
			UpdateTransformMessage.FillMessage(om, 23, this.Jets[0].Position, 23f);
			serverPeer.SendMessage(om, serverPeer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
		}

		private void SpawnJet(long playerID)
        {
			this.Jets.Add(new Jet(
				playerID,
				1,
				new Vector2(RandomNumberGenerator.Next(0, Constants.ScreenWidth), RandomNumberGenerator.Next(0, Constants.ScreenWidth)),
				(float)RandomNumberGenerator.NextDouble() * 2.0f


				)) ;
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
    }
}
