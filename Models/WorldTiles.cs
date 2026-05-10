using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Space_RPG.Models
{
    public class ShipTile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public FacilityType Type { get; set; }
        public bool IsWalkable { get; set; }
        public ShipTile() { IsWalkable = true; }
    }
}
