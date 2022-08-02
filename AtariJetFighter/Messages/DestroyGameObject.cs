using Lidgren.Network;

namespace AtariJetFighter.Messages
{
    public static class DestroyGameObject
    {
        public static NetOutgoingMessage CreateMessage(NetServer netServer,UpdateMessageType objectType, byte objectId)
        {
            NetOutgoingMessage message = netServer.CreateMessage();
            message.Write((byte)objectType);
            message.Write(objectId);
            return message;
        }
    }
}
