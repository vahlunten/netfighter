using AtariJetFighter.GameEngine.GameObjects;
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
		private NetServer NetServerInstance;
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
		/// Signals, whether simulated jet is enabled.
		/// </summary>
		private bool isSimulatingJet;
		/// <summary>
		/// Instance of simulated jet.
		/// </summary>
		private SimulatedJet simulatedJet;
		private bool RoundInProgress = false;
		private readonly float RoundDuration = 50.0f;
		private float RoundDurationRemaining = 50.0f;
		private readonly float PauseDuration = 5.0f;
		private float PauseDurationRemaining = 5.0f;

		/// <summary>
		/// GameMachine object constructor.
		/// </summary>
		/// <param name="game">Inctance of a game where GameMachine is running.</param>
		/// <param name="port">Port on wich the netclient listens for game related communication. Running on localhost.</param>
		public GameMachine(Game game, int port) : base(game)
		{

			this.Jets = new List<Jet>();
			this.Bullets = new List<Bullet>();
			this.dyingBullets = new List<Bullet>();
			simulatedJet = new SimulatedJet();

			this.initializePeer(port);
		}
		/// <summary>
		/// Starts the NetServer. 
		/// </summary>
		public override void Initialize()
		{
			// start listening for connections
			try
			{
				NetServerInstance.Start();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine("Game alrrady running on this port, client will join that one");
			}
		}
		/// <summary>
		/// Initialize NetServer instance on provided port.
		/// </summary>
		/// <param name="port">Port to start NetServer on.</param>
		private void initializePeer(int port)
		{
			NetPeerConfiguration config = new NetPeerConfiguration("AtariNetFighter");
			config.MaximumConnections = 10;
			config.Port = port;

			config.PingInterval = 1f;
			config.ConnectionTimeout = 1.5f;
			this.NetServerInstance = new NetServer(config);

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

			if (isSimulatingJet)
			{
				ControlSimulatedJet(gameTime);
			}

			if (Jets.Count > 0)
			{
				this.UpdateRoundState(gameTime);
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// Check current state of simulated jet and perform it's actions.
		/// </summary>
		/// <param name="gameTime"></param>
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

			if (this.simulatedJet.SecondsSinceLastShot > 2f)
			{
				this.simulatedJet.SecondsSinceLastShot = 0;
				Shoot(this.simulatedJet.Id);
			}
		}

		/// <summary>
		/// Stop the NetServer.
		/// </summary>
		public void Stop()
		{
			this.NetServerInstance.Shutdown("Host said bye.");
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
		/// <summary>
		/// Check for collisions. Both Jets and Bullets have property called Collision radius.
		/// Collision happens, whe (jet.ollisionRadius + bullet.CollisionRadius) is less than their distance.
		/// </summary>
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
                                if (RoundInProgress)
                                {
									UpdateJetScore(bullet.ShotByID, clientJet.Score + 1);
								}
							}
						}
					}


				}
			}
		}
		/// <summary>
		/// Destroy jets or bullets.
		/// </summary>
		/// <param name="gameObject">Object</param>
		private void DestroyObject(GameObject gameObject)
		{
			if (gameObject is Jet)
			{
				SendObjectDestroyMessage(UpdateMessageType.DestroyPlayer, gameObject.ObjectID);
				this.Jets.Remove((Jet)gameObject);

				if (this.Jets.Count == 1)
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
		/// <summary>
		/// Sends message to all connected clients about destroyed object.
		/// </summary>
		/// <param name="objType"></param>
		/// <param name="objectId"></param>
		private void SendObjectDestroyMessage(UpdateMessageType objType, byte objectId)
		{
			var message = DestroyGameObjectMessage.CreateMessage(NetServerInstance, objType, objectId);
			this.SendMessagetoEverybody(message);
		}

		/// <summary>
		/// Read connection and user control messages.
		/// </summary>
		/// <param name="gameTime"></param>
		private void processMessages(GameTime gameTime)
		{
			NetIncomingMessage message;
			while ((message = NetServerInstance.ReadMessage()) != null)
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
						ProcessUserControlMessage(message, gameTime);
						break;
					default:
						Console.WriteLine("Unhandled message type: " + message.MessageType + " " + message.LengthBytes + " bytes " + message.DeliveryMethod + "|" + message.SequenceChannel);
						break;
				}
				NetServerInstance.Recycle(message);
			}
		}
		/// <summary>
		/// Handle users connecting and disconnecting.
		/// </summary>
		/// <param name="message">Connection related message.</param>
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
		/// <summary>
		/// Process user control message and control his jet accordingly. 
		/// </summary>
		/// <param name="message">user control message.</param>
		/// <param name="gameTime">Current game time</param>
		private void ProcessUserControlMessage(NetIncomingMessage message, GameTime gameTime)
		{
			byte messageType = message.ReadByte();
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

		/// <summary>
		/// Update score of a player. This method is called when bullet hits new jet or when the game restarts.
		/// </summary>
		/// <param name="playerId">Players identifier.</param>
		/// <param name="score">New score</param>
		private void UpdateJetScore(long playerId, int score)
		{
			var updatedJet = GetClientJet(playerId);
			updatedJet.Score = score;

			var scoreUpdateMessage = UpdateScoreMessage.CreateMessage(this.NetServerInstance, playerId, score);
			SendMessagetoEverybody(scoreUpdateMessage, NetDeliveryMethod.Unreliable);

		}

		/// <summary>
		/// Adjust jets angle based on used input.
		/// </summary>
		/// <param name="playerId"></param>
		/// <param name="gameTime"></param>
		/// <param name="direction"></param>
		private void SteerJet(long playerId, GameTime gameTime, Controls direction)
		{
			var playerJet = this.GetClientJet(playerId);
			playerJet.Steer(gameTime, direction);
		}
		/// <summary>
		/// Shoot bulet form players jet.
		/// </summary>
		/// <param name="playerId">Identifier of a player who shot the bullet.</param>
		private void Shoot(long playerId)
		{
			// do not allow shooting in between rounds
			if(RoundInProgress)
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

		}
		/// <summary>
		/// Sends message about new bullet in scene when some of the player shoots.
		/// </summary>
		/// <param name="bullet">Spawned bullet.</param>
		private void SendSpawnNewBulletMessage(Bullet bullet)
		{
			var spawnBulletMessage = SpawnNewBulletMessage.CreateMessage(NetServerInstance, bullet.ObjectID, bullet.Position, bullet.Rotation, bullet.Color);
			this.SendMessagetoEverybody(spawnBulletMessage);

		}
		/// <summary>
		/// Sends message to spawn new players jet as well as simulated jet.
		/// </summary>
		/// <param name="jet">Spawned jet.</param>
		private void SendSpawnNewJetMessage(Jet jet)
		{
			NetOutgoingMessage spawnPlayerMessage = SpawnPlayerMessage.CreateMessage(this.NetServerInstance, jet.ObjectID, jet.PlayerId, jet.Position, jet.Rotation, jet.Color);
			this.SendMessagetoEverybody(spawnPlayerMessage);
		}
		/// <summary>
		/// Sends message to update transform of a game object. It is called every tick for every object.
		/// </summary>
		/// <param name="gameObject">Game object identifier.</param>
		private void SendUpdateTransformMessage(GameObject gameObject)
		{
			NetOutgoingMessage updateTransformMessage = UpdateTransformMessage.CreateMessage(this.NetServerInstance, gameObject.ObjectID, gameObject.Position, gameObject.Rotation);
			this.SendMessagetoEverybody(updateTransformMessage);
		}

		/// <summary>
		/// Every message is sent using this method to deliver it to all connected players.
		/// </summary>
		/// <param name="message">Message to send.</param>
		/// <param name="method">Message delivery method.</param>
		private void SendMessagetoEverybody(NetOutgoingMessage message, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
		{
			NetServerInstance.SendMessage(message, NetServerInstance.Connections, method, 0);
		}

		/// <summary>
		/// Spawns net player jet or simulated jet and notifies all players.
		/// </summary>
		/// <param name="playerID"></param>
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
			if (Jets.Count == 1)
			{
				EnableSimulatedJet();

				// disable simulated jet when second player spawns
			} else if (Jets.Count == 3)
			{
				DisableSimulatedJet();
			}

			//this.SendSpawnNewJetMessage(newJet);
			// when new player connects to the game, we have to notify him about all clients that have connected before hi
			this.UpdateRemoteJetList();
			this.ResetRound();
		}
		/// <summary>
		/// When player connects, this method is called to update his list about previously connected players.
		/// </summary>
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

		/// <summary>
		/// Get new identifier for newly spawned object.
		/// </summary>
		/// <returns></returns>
		private byte GetObjectId()
		{
			for (byte newId = 0; newId < byte.MaxValue; newId++)
			{
				if (!Jets.Any(jet => jet.ObjectID == newId) && !Bullets.Any(bullet => bullet.ObjectID == newId))
				{
					return newId;
				}
			}
			throw new Exception("Developer of this game is an idiot and he should have used int for objectID");
		}
		/// <summary>
		/// Returns color of jet for new player. 
		/// </summary>
		/// <returns></returns>
		private byte GetRandomFreeColor()
		{
			byte randomColorCode = (byte)RandomNumberGenerator.Next(0, 10);
			while (Jets.Any(jet => jet.Color == randomColorCode))
			{
				randomColorCode = (byte)RandomNumberGenerator.Next(0, 10);
			}
			return randomColorCode;

		}
		/// <summary>
		/// Create instance of simulated jet.
		/// </summary>
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
		/// <summary>
		/// Destroy simulated jet.
		/// </summary>
		private void DisableSimulatedJet()
		{
			var disconnectedPlayerJet = this.Jets.Find(jet => jet.PlayerId == this.simulatedJet.Id);
			this.DestroyObject(disconnectedPlayerJet);
			this.isSimulatingJet = false;
		}
		
		/// <summary>
		/// Update timers for round. Change state if the time has run out.
		/// </summary>
		/// <param name="gameTime"></param>
		private void UpdateRoundState(GameTime gameTime)
        {
			var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (RoundInProgress)
            {
				this.SendRoundUpdate(RoundDurationRemaining);
				this.RoundDurationRemaining -= elapsed;
				
				if(this.RoundDurationRemaining < 0 )
                {
					Console.WriteLine("Round ended");
					this.RoundInProgress = false;
					this.PauseDurationRemaining = this.PauseDuration;
				} 
            } else
            {
				SendRoundUpdate(PauseDurationRemaining);
				this.PauseDurationRemaining -= elapsed;
				if(this.PauseDurationRemaining <= 0)
                {					
					ResetRound();
				}
			}
        }
		/// <summary>
		/// Send current round time left to players.
		/// </summary>
		/// <param name="time"></param>
		private void SendRoundUpdate(float time)
        {
			var roundUpdateMessage = RoundUpdateMessage.CreateMessage(this.NetServerInstance, this.RoundInProgress, time);
			SendMessagetoEverybody(roundUpdateMessage, NetDeliveryMethod.Unreliable);
		}

		/// <summary>
		/// Reset scores, reset round and start new round.
		/// </summary>
		private void ResetRound()
        {
			Console.WriteLine("Round started");
			this.RoundInProgress = true;
			this.RoundDurationRemaining = this.RoundDuration;

			foreach (var jet in this.Jets)
            {
				jet.Score = 0;
				var scoreUpdateMessage = UpdateScoreMessage.CreateMessage(this.NetServerInstance, jet.PlayerId, 0);
				SendMessagetoEverybody(scoreUpdateMessage, NetDeliveryMethod.Unreliable);
			}
        }
	}
}
