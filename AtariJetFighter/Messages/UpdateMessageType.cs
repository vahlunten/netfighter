using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.Messages
{
    public enum UpdateMessageType : byte
    {
        UserControl,
        UpdateTransform,
        SpawnPlayer,
        SpawnProjectile,
        DestroyPlayer,
        DestroyProjectile,
        UpdateScore,
        RestartGame
    }
}
