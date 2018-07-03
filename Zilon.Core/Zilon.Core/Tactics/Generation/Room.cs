﻿using System.Collections.Generic;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Generation
{
    public class Room
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<HexNode> Nodes { get; set; }

        public Room() {
            Nodes = new List<HexNode>();
        }
    }
}