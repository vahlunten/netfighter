using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.Networking.Messages
{
    public static class UserControlMessage
    {
        public static void FillMessage(NetOutgoingMessage message, Controls command)
        {
            message.Write((byte)UpdateMessageType.UserControl);
            message.Write((byte)command);
        }
    }
}
