using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Models
{
    public class ShipDesign
    {
        public string Name { get; set; }
        public List<ShipRoomDesign> Rooms { get; set; }

        public ShipDesign()
        {
            Rooms = new List<ShipRoomDesign>();
        }
    }

    public class ShipRoomDesign
    {
        public string Name { get; set; }
        public FacilityType RoomType { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int WorldX { get; set; }
        public int WorldY { get; set; }

        public List<ShipObjectDesign> Objects { get; set; }

        public ShipRoomDesign()
        {
            Objects = new List<ShipObjectDesign>();
        }
    }

    public class ShipObjectDesign
    {
        public string Name { get; set; }

        public InteriorObjectType ObjectType { get; set; }
        public InteriorTileType TileType { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public bool AllowMultipleCrew { get; set; }

        public List<InteractionDirection> InteractionDirections { get; set; }

        public ShipObjectDesign()
        {
            Width = 1;
            Height = 1;
            InteractionDirections = new List<InteractionDirection>();
        }
    }
}
