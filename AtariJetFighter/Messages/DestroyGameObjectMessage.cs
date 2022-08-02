using Lidgren.Network;

namespace AtariJetFighter.Messages
{
    public static class DestroyGameObjectMessage
    {
        /// <summary>
        /// This message is used to destroy Jets of disconnected players and bullets at the end of their lifespan. 
        /// </summary>
        /// <param name="netServer">NetServer instance.</param>
        /// <param name="objectType">Type of object: Either Jet or Bullet</param>
        /// <param name="objectId">Unique object identifier</param>
        /// <returns></returns>
        public static NetOutgoingMessage CreateMessage(NetServer netServer,UpdateMessageType objectType, byte objectId)
        {
            NetOutgoingMessage message = netServer.CreateMessage();
            message.Write((byte)objectType);
            message.Write(objectId);
            return message;
        }
    }
}
