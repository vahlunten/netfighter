using AtariJetFighter.GameEngine.GameObjects;
using AtariJetFighter.Messages;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtariJetFighter.GameMachineObjects
{
	/// <summary>
	/// This class exists on the server. It is responsible for holding information for every object in the game,
	/// updating it's transform, instantiating and creating them, notifying clients about changes. 
	/// Is also handles all the running game logic. It's instance is added to Monogames Game instance as Game component.
	/// Monogame handless calling the Update method. 
	/// </summary>
	internal class GameMachine : DrawableGameComponent
	{
		/// <summary>
		/// NetServer instance used for sending and receiving messages. 
		/// </summary>
		private NetServer serverPeer;
		private Random RandomNumberGenerator = new Random();

		/// <summary>
		/// Collection of currently playing players and their jets. 
		/// </summary>
		private List<Jet> Jets;
		/// <summary>
		/// Collection of currently spawned bullets. 
		/// </summary>
		private List<Bullet> Bullets;

		/// <summary>
		/// Bullets to be removed in next tick.
		/// </summary>
		private List<Bullet> dyingBullets;

		/// <summary>
		/// GameMachine object constructor.
		/// </summary>
		/// <param name="game">Inctance of a game where GameMachine is running.</param>
		/// <param name="port">Port on wich the netclient listens for game related communication. Running on localhost.</param>
		public GameMachine(Game game, int port) : base(game)
		{
			NetPeerConfiguration config = new NetPeerConfiguration("AtariNetFighter");
			config.MaximumConnections = 10;
			config.Port = port;
			
			config.PingInterval = 1f;
			config.ConnectionTimeout = 1.5f;
			this.Jets = new List<Jet>();
			this.Bullets = new List<Bullet>();
			this.dyingBullets = new List<Bullet>();
			this.serverPeer = new NetServer(config);
			

		}
		/// <summary>
		/// Starts the NetServer. 
		/// </summary>
		public override void Initialize()
		{
			// start listening for connections
			serverPeer.Start();
			base.Initialize();
		}

		/// <summary>
		/// Most of the logic related to game objects happen in here.
		/// GameMachine processes all the control messages from clients.
		/// Transform of each game object is updated here. All clients are notified about those changes 
		/// by corresponding messages.
		/// </summary>
		/// <param name="gameTime">Current game time.</param>
		public override void Update(GameTime gameTime)
		{
			processMessages(gameTime);
			CheckForCollisions();
			RemoveExpiredBullets();
			foreach (var jet in Jets)
			{
				jet.Update(gameTime);
				SendUpdateTransformMessage(jet);
			}

			foreach (var bullet in Bullets)
			{
				bullet.Update(gameTime);
				//Console.WriteLine(bullet.Position);
				if (bullet.LifespanLeft < 0)
				{
					this.dyingBullets.Add(bullet);
				}
				else
				{
					this.SendUpdateTransformMessage(bullet);
				}
			}
			base.Update(gameTime);
		}

		public void Stop()
        {
			this.serverPeer.Shutdown("Host said bye.");
        }
		private void RemoveExpiredBullets()
		{
			foreach (var bullet in dyingBullets)
			{
				this.Bullets.Remove(bullet);
				this.DestroyObject(bullet);
			}
			this.dyingBullets.Clear();

		}
		private void CheckForCollisions()
		{
			// check for collisions
			// 
		}
		private void DestroyObject(GameObject gameObject)
		{
			if (gameObject is Jet)
			{
				SendObjectDestroyMessage(UpdateMessageType.DestroyPlayer, gameObject.ObjectID);
				this.Jets.Remove((Jet)gameObject);
			}
			else if (gameObject is Bullet)
			{
				SendObjectDestroyMessage(UpdateMessageType.DestroyProjectile, gameObject.ObjectID);
				this.Bullets.Remove((Bullet)gameObject);

			}
		}
		private void SendObjectDestroyMessage(UpdateMessageType objType, byte objectId)
		{
			var message = DestroyGameObject.CreateMessage(serverPeer, objType, objectId);
			this.SendMessagetoEverybody(message);
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
				this.SpawnJet(message.SenderConnection.RemoteUniqueIdentifier);
			}
			// User disconnected, we will destroy his jet and notify other players.
			else if (status == NetConnectionStatus.Disconnected)
			{
				var disconnectedPlayerJet = this.Jets.Find(jet => jet.PlayerId == message.SenderConnection.RemoteUniqueIdentifier);
				this.DestroyObject(disconnectedPlayerJet);
				Console.WriteLine("Player disconnected: " + message.SenderConnection.RemoteUniqueIdentifier);
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
			}
			else
			{
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
			var spawnBulletMessage = SpawnNewBulletMessage.CreateMessage(serverPeer, bullet.ObjectID, bullet.Position, bullet.Rotation);
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

		private void SendMessagetoEverybody(NetOutgoingMessage message, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
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
			throw new Exception("Developer of this application is an idiot");
		}
		protected override void UnloadContent()
		{
			base.UnloadContent();
		}
	}
}
