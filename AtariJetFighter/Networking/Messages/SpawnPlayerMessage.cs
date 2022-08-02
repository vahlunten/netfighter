using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.Networking.Messages
{
    public static class SpawnPlayerMessage

    {
        public static void FillMessage(NetOutgoingMessage message, byte objectId,long jetOwner, Vector2 position, float rotation)
        {
            message.Write((byte)UpdateMessageType.SpawnPlayer);
            message.Write(objectId);
            message.Write(jetOwner);
            message.Write(position.X);
            message.Write(position.Y);
            message.Write(rotation);

        }
    }
}
