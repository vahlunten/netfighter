using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtariJetFighter.GameEngine.GameObjects
{
    class Jet : GameObject
    {
        public float Velocity { get; set; }
        public long PlayerId { get; set; }

    }
}
