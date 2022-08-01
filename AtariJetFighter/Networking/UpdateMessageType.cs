using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.Networking
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
