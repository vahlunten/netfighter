using AtariJetFighter.GameEngine.GameObjects;
using AtariJetFighter.Messages;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public override void Update(GameTime gameTime)
        {
			processMessages(gameTime);
			CheckForCollisions();

            foreach (var jet in Jets)
            {
				jet.Update(gameTime);
				SendUpdateTransformMessage(jet);
			}

			foreach (var bullet in Bullets)
			{
				bullet.Update(gameTime);
				//Console.WriteLine(bullet.Position);
				SendUpdateTransformMessage(bullet);
			}
			base.Update(gameTime);
        }

        private void CheckForCollisions()
        {
            // check for collisions
			// 
        }

        private void processMessages(GameTime gameTime)
        {
			NetIncomingMessage message;
			while ((message = serverPeer.ReadMessage()) != null)
			{
				// handle incoming message
				switch (message.MessageType)
				{
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.ErrorMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.VerboseDebugMessage:
						string text = message.ReadString();
						Console.WriteLine("Server has received: " + message.MessageType + "saying: " + text);
						break;

					case NetIncomingMessageType.StatusChanged:
						ProcessConnectionMessage(message);
						break;
					case NetIncomingMessageType.Data:
						ProcessMessage(message, gameTime);
						break;
					default:
						Console.WriteLine("Unhandled message type: " + message.MessageType + " " + message.LengthBytes + " bytes " + message.DeliveryMethod + "|" + message.SequenceChannel);
						break;
				}
				serverPeer.Recycle(message);
			}
		}

		private void ProcessConnectionMessage(NetIncomingMessage message)
        {
			NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
			Console.WriteLine("Status changed: " + status);

			if (status == NetConnectionStatus.Connected)
			{
				connectedPlayers.Add(message.SenderConnection);
				this.SpawnJet(message.SenderConnection.RemoteUniqueIdentifier);
			}
			// TODO: Handle disconnect
			else if (status == NetConnectionStatus.Connected)
			{
				Console.WriteLine("Remote hail: " + message.SenderConnection.RemoteHailMessage.ReadString());
			}

		}
		private void ProcessMessage(NetIncomingMessage message, GameTime gameTime)
		{
			byte messageType = message.ReadByte();
			switch (messageType)
			{
				case (byte)UpdateMessageType.UserControl:
					{
						this.ProcessUserControlMessage(message, gameTime);
						break;
					}

				default:
					break;
			}
		}

		private void ProcessUserControlMessage(NetIncomingMessage message, GameTime gameTime)
        {			
			Controls direction = (Controls)message.ReadByte();

            if (direction == Controls.Shoot)
            {
				Shoot(message);
            } else {
				SteerJet(message, gameTime, direction);
            }


		}

		private void SteerJet(NetIncomingMessage message, GameTime gameTime, Controls direction)
        {
			var playerJet = this.GetClientJet(message.SenderConnection.RemoteUniqueIdentifier);
			playerJet.Steer(gameTime, direction);
		}

		private void Shoot(NetIncomingMessage message)
        {
			var playerJet = this.GetClientJet(message.SenderConnection.RemoteUniqueIdentifier);
			var newBullet = new Bullet(playerJet.PlayerId, GetObjectId(), playerJet.Position, playerJet.Rotation);
			this.Bullets.Add(newBullet);
			SendSpawnNewBulletMessage(newBullet);    

		}

		private void SendSpawnNewBulletMessage(Bullet bullet)
        {
			 var spawnBulletMessage = SpawnNewBulletMessage.CreateMessage(serverPeer, bullet.ObjectID,bullet.Position,bullet.Rotation);
			this.SendMessagetoEverybody(spawnBulletMessage);

		}
		private void SendSpawnNewJetMessage(Jet jet)
        {
			NetOutgoingMessage spawnPlayerMessage = SpawnPlayerMessage.CreateMessage(this.serverPeer, jet.ObjectID, jet.PlayerId, jet.Position, jet.Rotation);
			this.SendMessagetoEverybody(spawnPlayerMessage);    
        }
		private void SendUpdateTransformMessage(GameObject gameObject)
        {
			NetOutgoingMessage updateTransformMessage = UpdateTransformMessage.CreateMessage(this.serverPeer, gameObject.ObjectID, gameObject.Position, gameObject.Rotation);
			this.SendMessagetoEverybody(updateTransformMessage);
		}

		private void SendMessagetoEverybody(NetOutgoingMessage message, NetDeliveryMethod  method = NetDeliveryMethod.Unreliable)
        {
			serverPeer.SendMessage(message, serverPeer.Connections, method, 0);
		}

		private void SpawnJet(long playerID)
        {
			Console.WriteLine("spawn jet in gamemachine");
			Jet newJet = new Jet(
				playerID,
				GetObjectId(),
				new Vector2(RandomNumberGenerator.Next(0, Constants.ScreenWidth), RandomNumberGenerator.Next(0, Constants.ScreenWidth)),
				(float)RandomNumberGenerator.NextDouble() * 2.0f
				);
			this.Jets.Add(newJet);

			this.SendSpawnNewJetMessage(newJet);
			// when new player connects to the game, we have to notify him about all clients that have connected before hi
			this.UpdateRemoteJetList(newJet);


        }

		private void UpdateRemoteJetList(Jet newJetD)
        {
            foreach (var jet in Jets)
            {
				this.SendSpawnNewJetMessage(jet);
            }
        }

		private Jet GetClientJet(long playerId)
        {
			return this.Jets.Find(jet => jet.PlayerId == playerId);
        }
		// TODO: I feel dumb for writing this lol 
		private byte GetObjectId()
        {
            for (byte newId = 0; newId < byte.MaxValue; newId++)
            {
                if (!Jets.Any(jet => jet.ObjectID == newId) && !Bullets.Any(bullet => bullet.ObjectID == newId))
                {
					return newId;
                }
            }
			throw new Exception("You are an idiot");
        }
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
    }
}
