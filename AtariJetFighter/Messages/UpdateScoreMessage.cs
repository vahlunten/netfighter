﻿using Lidgren.Network;

namespace AtariJetFighter.Messages
{
    static class UpdateScoreMessage
    {
        /// <summary>
        /// This message is sent when player's bullet collides with another jet or upon round restart.
        /// </summary>
        /// <param name="netServer">NetServer instance.</param>
        /// <param name="playerId">Unique object identifier.</param>
        /// <param name="score">New score of a player.</param>
        /// <returns></returns>
        public static NetOutgoingMessage CreateMessage(NetPeer netServer, long playerId, int score)
        {
            NetOutgoingMessage message = netServer.CreateMessage();
            message.Write((byte)UpdateMessageType.UpdateScore);
            message.Write(playerId);
            message.Write(score);
            return message;
        }
    }
}
