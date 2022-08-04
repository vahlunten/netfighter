using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.Messages
{
    
    public static class RoundUpdateMessage
    {
        /// <summary>
        /// This message container current game time and if the round is currently paused it also sends the winnerId.
        /// </summary>
        /// <param name="netServer">NetServer instance.</param>
        /// <param name="roundInProgress">If the round is in progress.</param>
        /// <param name="time"> Current game time.</param>
        /// <returns></returns>
        public static NetOutgoingMessage CreateMessage(NetServer netServer, bool roundInProgress, float time, long winnerId = 0)
        {
            NetOutgoingMessage message = netServer.CreateMessage();
            message.Write((byte)UpdateMessageType.RoundUpdate);
            message.Write(roundInProgress);
            message.Write(time);
            return message;
        }
    }
}
