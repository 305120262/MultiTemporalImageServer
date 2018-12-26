﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalCacheLibrary
{
    public class TileInfo
    {
        public int Level { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int Size { get; set; }
        public byte[] TileData { get; set; }
    }
}
