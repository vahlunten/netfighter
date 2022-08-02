using Lidgren.Network;

namespace AtariJetFighter.Messages
{
    public static class UserControlMessage
    {
        /// <summary>
        /// This message is sent when player presses control keys.
        /// </summary>
        /// <param name="netClient">NetClient instance.</param>
        /// <param name="command">Control command sent by client. Either: Left, Right or Shoot.</param>
        /// <returns></returns>
        public static NetOutgoingMessage CreateMessage(NetClient netClient, Controls command)
        {
            NetOutgoingMessage message = netClient.CreateMessage();
            message.Write((byte)UpdateMessageType.UserControl);
            message.Write((byte)command);
            return message;
        }
    }
}
