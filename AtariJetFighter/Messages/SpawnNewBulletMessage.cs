﻿using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace AtariJetFighter.Messages
{
    public static class SpawnNewBulletMessage

    {
        public static NetOutgoingMessage CreateMessage(NetServer netServer, byte objectId, Vector2 position, float rotation)
        {
            NetOutgoingMessage message = netServer.CreateMessage();
            message.Write((byte)UpdateMessageType.SpawnProjectile);
            message.Write(objectId);
            message.Write(position.X);
            message.Write(position.Y);
            message.Write(rotation);
            return message;
        }
    }
}