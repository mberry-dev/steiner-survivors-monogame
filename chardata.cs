using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteinerSurvivors
{
    internal class CharData
    {
        public int Id { get; set; } // ASCII code of the character
        public int X { get; set; }  // X position of the character in the texture
        public int Y { get; set; }  // Y position of the character in the texture
        public int Width { get; set; }  // Width of the character in the texture
        public int Height { get; set; } // Height of the character in the texture
        public int XOffset { get; set; } // X offset for drawing the character
        public int YOffset { get; set; } // Y offset for drawing the character
    }
}