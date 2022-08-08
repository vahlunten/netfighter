using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace AtariJetFighter.Messages
{
    public static class UpdateTransformMessage
    {
        /// <summary>
        /// This message is used to update transform of both Jets and Bullets
        /// </summary>
        /// <param name="netServer">NetServer instance.</param>
        /// <param name="objectId">Unique object identifier.</param>
        /// <param name="position">New position of updated object.</param>
        /// <param name="rotation">New rotation of updated object.</param>
        /// <returns></returns>
        public static NetOutgoingMessage CreateMessage(NetPeer netServer, byte objectId, Vector2 position, float rotation)
        {
            NetOutgoingMessage message = netServer.CreateMessage();
            message.Write((byte)UpdateMessageType.UpdateTransform);
            message.Write(objectId);
            message.Write(position.X);
            message.Write(position.Y);
            message.Write(rotation);
            return message;
        }
    }
}
