using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AtariJetFighter
{
    internal static class Constants
    {
        /// <summary>
        /// Canvas resolution width.
        /// </summary>
        public const int ScreenWidth = 800;
        /// <summary>
        /// Canvas resolution height.
        /// </summary>
        public const int ScreenHeight = 800;

        /// <summary>
        /// Collection of possible colors for jets.
        /// </summary>
        public static Dictionary<byte, Color> Colors = new Dictionary<byte, Color>
        {
            { 0, Color.Fuchsia },
            { 1, Color.Green },
            { 2, Color.Yellow },
            { 3, Color.Chocolate},
            { 4, Color.Red},
            { 5, Color.Blue},
            { 6, Color.Cyan},
            { 7, Color.PeachPuff},
            { 8, Color.Purple},
            { 9, Color.Khaki},

        };

        /// <summary>
        /// Names of colors
        /// </summary>
        public static Dictionary<Color, string> ColorNames = new Dictionary<Color, string>
        {
            { Color.Fuchsia, "fuchsia" },
            { Color.Green, "green"},
            { Color.Yellow, "yellow" },
            { Color.Chocolate, "chocolate"},
            { Color.Red, "red"},
            { Color.Blue, "blue"},
            { Color.Cyan, "cyan"},
            { Color.PeachPuff, "peach puff"},
            { Color.Purple, "purple"},
            { Color.Khaki, "khaki"},

        };
    }
}
