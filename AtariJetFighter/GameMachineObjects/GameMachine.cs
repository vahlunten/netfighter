﻿using AtariJetFighter.GameEngine.GameObjects;
using AtariJetFighter.GameMachineObjects.GameObjects;
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

		private bool isSimulatingJet;
		private SimulatedJet simulatedJet;
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

			simulatedJet = new SimulatedJet();
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
				this.SendUpdateTransformMessage(bullet);
			}

			if(isSimulatingJet)
            {
				ControlSimulatedJet(gameTime);
            }


			base.Update(gameTime);
		}

        private void ControlSimulatedJet(GameTime gameTime)
        {
			this.simulatedJet.Update(gameTime);
			if (this.simulatedJet.CurerntState == SimulatedJet.State.SteeringLeft)
			{
				this.SteerJet(this.simulatedJet.Id, gameTime, Controls.Left);
			}
			else if (this.simulatedJet.CurerntState == SimulatedJet.State.SteeringRight)
			{
				this.SteerJet(this.simulatedJet.Id, gameTime, Controls.Right);
			}

			if (this.simulatedJet.SecondsSinceLastShot > 3f)
			{
				this.simulatedJet.SecondsSinceLastShot = 0;
				Shoot(this.simulatedJet.Id);
			}
		}

        public void Stop()
		{
			this.serverPeer.Shutdown("Host said bye.");
		}
		private void RemoveExpiredBullets()
		{
			foreach (var bullet in Bullets)
			{
				if (bullet.LifespanLeft < 0)
				{
					this.dyingBullets.Add(bullet);
				}
			}

			foreach (var bullet in dyingBullets)
			{
				this.Bullets.Remove(bullet);
				this.DestroyObject(bullet);
			}
			this.dyingBullets.Clear();

		}    
		private void CheckForCollisions()
		{
			foreach (var jet in this.Jets)
			{
				foreach (var bullet in this.Bullets)
				{

					if (jet.PlayerId != bullet.ShotByID)
					{
						var distance = (bullet.Position - jet.Position).Length();
                        if (distance < jet.ColliderRadius + bullet.ColliderRadius)
                        {
							dyingBullets.Add(bullet);
							var clientJet = GetClientJet(bullet.ShotByID);
                            if (clientJet != null)
                            {
								UpdateJetScore(bullet.ShotByID, clientJet.Score + 1);
							}
                        }
					}
					

				}
			}
		}
		private void DestroyObject(GameObject gameObject)
		{
			if (gameObject is Jet)
			{
				SendObjectDestroyMessage(UpdateMessageType.DestroyPlayer, gameObject.ObjectID);
				this.Jets.Remove((Jet)gameObject);

				if(this.Jets.Count == 1)
                {
					EnableSimulatedJet();
					this.UpdateRemoteJetList();
				}
			}
			else if (gameObject is Bullet)
			{
				SendObjectDestroyMessage(UpdateMessageType.DestroyProjectile, gameObject.ObjectID);
				this.Bullets.Remove((Bullet)gameObject);

			}
		}
		private void SendObjectDestroyMessage(UpdateMessageType objType, byte objectId)
		{
			var message = DestroyGameObjectMessage.CreateMessage(serverPeer, objType, objectId);
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
				Shoot(message.SenderConnection.RemoteUniqueIdentifier);
			}
			else
			{
				SteerJet(message.SenderConnection.RemoteUniqueIdentifier, gameTime, direction);
			}


		}

		private void UpdateJetScore(long playerId, int score)
		{
			var updatedJet = GetClientJet(playerId);
			updatedJet.Score = score;

			var scoreUpdateMessage = UpdateScoreMessage.CreateMessage(this.serverPeer, playerId, score);
			SendMessagetoEverybody(scoreUpdateMessage, NetDeliveryMethod.Unreliable);

		}


		private void SteerJet(long playerId, GameTime gameTime, Controls direction)
		{
			var playerJet = this.GetClientJet(playerId);
			playerJet.Steer(gameTime, direction);
		}

		private void Shoot(long playerId)
		{
			var playerJet = this.GetClientJet(playerId);

			
			var newBullet = new Bullet(playerJet.PlayerId, GetObjectId(), playerJet.Position, playerJet.Rotation, playerJet.Color);

            if (playerJet.ShotCooldownLeft <= 0)
            {
				playerJet.ShotCooldownLeft = playerJet.ShotCooldown;
				this.Bullets.Add(newBullet);
				SendSpawnNewBulletMessage(newBullet);
			}

		}

		private void SendSpawnNewBulletMessage(Bullet bullet)
		{
			var spawnBulletMessage = SpawnNewBulletMessage.CreateMessage(serverPeer, bullet.ObjectID, bullet.Position, bullet.Rotation, bullet.Color);
			this.SendMessagetoEverybody(spawnBulletMessage);

		}
		private void SendSpawnNewJetMessage(Jet jet)
		{
			NetOutgoingMessage spawnPlayerMessage = SpawnPlayerMessage.CreateMessage(this.serverPeer, jet.ObjectID, jet.PlayerId, jet.Position, jet.Rotation, jet.Color);
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
                (float)RandomNumberGenerator.NextDouble() * 2.0f,
                GetRandomFreeColor()
                );
            this.Jets.Add(newJet);

			// spawn simulated jets when the fist player spawns
			if(Jets.Count == 1)
            {
				EnableSimulatedJet();
			
			// disable simulated jet when second player spawns
			} else if(Jets.Count == 3)
            {
				DisableSimulatedJet();
			}

			//this.SendSpawnNewJetMessage(newJet);
			// when new player connects to the game, we have to notify him about all clients that have connected before hi
			this.UpdateRemoteJetList();
			this.ResetScore();


		}

		private void UpdateRemoteJetList()
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
		private byte GetRandomFreeColor()
		{
			byte randomColorCode = (byte)RandomNumberGenerator.Next(0, 10);
			while (Jets.Any(jet => jet.Color == randomColorCode))
			{
				randomColorCode = (byte)RandomNumberGenerator.Next(0, 10);
			}
			return randomColorCode;

		}

		private void EnableSimulatedJet()
        {
			Jet simulatedJet = new Jet(
					this.simulatedJet.Id,
					GetObjectId(),
					new Vector2(RandomNumberGenerator.Next(0, Constants.ScreenWidth), RandomNumberGenerator.Next(0, Constants.ScreenWidth)),
					(float)RandomNumberGenerator.NextDouble() * 2.0f,
					GetRandomFreeColor()
					);
			this.isSimulatingJet = true;
			this.Jets.Add(simulatedJet);

		}

		private void DisableSimulatedJet()
        {
			var disconnectedPlayerJet = this.Jets.Find(jet => jet.PlayerId == this.simulatedJet.Id);
			this.DestroyObject(disconnectedPlayerJet);
			this.isSimulatingJet = false;
		}

		private void ResetScore()
        {
            foreach (var jet in this.Jets)
            {
				var scoreUpdateMessage = UpdateScoreMessage.CreateMessage(this.serverPeer, jet.PlayerId, 0);
				SendMessagetoEverybody(scoreUpdateMessage, NetDeliveryMethod.Unreliable);
			}
        }
	}
}
