using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace AtariJetFighter.Messages
{
    public static class SpawnPlayerMessage

    {
        public static NetOutgoingMessage CreateMessage( NetServer netServer, byte objectId,long jetOwner, Vector2 position, float rotation)
        {
            NetOutgoingMessage message = netServer.CreateMessage();
            message.Write((byte)UpdateMessageType.SpawnPlayer);
            message.Write(objectId);
            message.Write(jetOwner);
            message.Write(position.X);
            message.Write(position.Y);
            message.Write(rotation);
            return message;

        }
    }
}
