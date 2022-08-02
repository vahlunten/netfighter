using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AtariJetFighter
{
    internal static class Constants
    {
        public const int ScreenWidth = 800;
        public const int ScreenHeight = 800;

        public static Dictionary<byte, Color> colors = new Dictionary<byte, Color>
        {
            { 0, Color.MediumTurquoise },
            { 1, Color.Green },
            { 2, Color.Yellow },
            { 3, Color.BlueViolet},
            { 4, Color.Red},
            { 5, Color.Aquamarine},
            { 6, Color.Cyan},
            { 7, Color.PeachPuff},
            { 8, Color.Purple},
            { 9, Color.Khaki},

        };
    }
}
