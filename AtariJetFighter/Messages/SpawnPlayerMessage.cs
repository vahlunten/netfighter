using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace AtariJetFighter.Messages
{
    public static class SpawnPlayerMessage

    {
        /// <summary>
        /// This message is used to spawn player's jet.
        /// </summary>
        /// <param name="netServer">NetServer instance</param>
        /// <param name="objectId">Unique object identifier.</param>
        /// <param name="jetOwner">NetClient ID - owner of a jet.</param>
        /// <param name="position">Initial jet position.</param>
        /// <param name="rotation">Initial jet rotation.</param>
        /// <param name="color"> Jet color.</param>
        /// <returns></returns>
        public static NetOutgoingMessage CreateMessage(NetPeer netServer, byte objectId,long jetOwner, Vector2 position, float rotation, byte color)
        {
            NetOutgoingMessage message = netServer.CreateMessage();
            message.Write((byte)UpdateMessageType.SpawnPlayer);
            message.Write(objectId);
            message.Write(jetOwner);
            message.Write(position.X);
            message.Write(position.Y);
            message.Write(rotation);
            message.Write(color);
            return message;

        }
    }
}
