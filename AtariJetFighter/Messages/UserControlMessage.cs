using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.Messages
{
    public static class UserControlMessage
    {
        public static NetOutgoingMessage CreateMessage(NetClient netClient, Controls command)
        {
            NetOutgoingMessage message = netClient.CreateMessage();
            message.Write((byte)UpdateMessageType.UserControl);
            message.Write((byte)command);
            return message;
        }
    }
}
