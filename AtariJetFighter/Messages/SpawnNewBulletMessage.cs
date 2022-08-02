using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace AtariJetFighter.Messages
{
    public static class SpawnNewBulletMessage

    {
        /// <summary>
        /// This message is used to spawn bullets.
        /// </summary>
        /// <param name="netServer">NetServer instance.</param>
        /// <param name="objectId">Unique object identifier</param>
        /// <param name="position">Initial bullet position.</param>
        /// <param name="rotation">Bullet rotation </param>
        /// <param name="color">Color of the bullet based on the player's jet.</param>
        /// <returns></returns>
        public static NetOutgoingMessage CreateMessage(NetServer netServer, byte objectId, Vector2 position, float rotation, byte color)
        {
            NetOutgoingMessage message = netServer.CreateMessage();
            message.Write((byte)UpdateMessageType.SpawnProjectile);
            message.Write(objectId);
            message.Write(position.X);
            message.Write(position.Y);
            message.Write(rotation);
            message.Write(color);
            return message;
        }
    }
}
