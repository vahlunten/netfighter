using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.Networking.Messages
{
    public static class UpdateTransformMessage
    {
        public static void FillMessage(NetOutgoingMessage message,byte objectId, Vector2 position, float rotation)
        {
            message.Write((byte)UpdateMessageType.UpdateTransform);
            message.Write(objectId);
            message.Write(position.X);
            message.Write(position.Y);
            message.Write(rotation);

        }
    }
}
