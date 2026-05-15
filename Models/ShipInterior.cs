using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Space_RPG.Models
{
    public enum FacilityType
    {
        MainDeck,
        CaptainsRoom,
        Cafeteria,
        Armoury,
        FemaleCrewQuarters,
        MaleCrewQuarters,
        Garden,
        Gym,
        MedicalBay,
        Cargo,
        Corridor,
        EngineRoom
    }
    public enum InteriorObjectType
    {
        None = 0,
        // Workstations
        Cockpit,
        WeaponsConsole,
        MedicalConsole,
        Workbench,

        Bed, 
        Table, 
        Stove, 
        Sofa
    }

    public enum InteriorTileType 
    { 
        Floor = 0, 
        Wall = 1, 
        Door = 2, 
        Bed = 3, 
        StorageBox = 4, 
        Workbench = 5, 
        KitchenCounter = 6, 
        Table = 7, 
        Sofa = 8
    }

    public class ShipInterior
    {
        public List<InteriorRoom> Rooms { get; set; }

        [JsonIgnore]
        public Dictionary<string, ShipMapTile> GlobalTileMap { get; set; }

        public ShipInterior()
        {
            Rooms = new List<InteriorRoom>();
            GlobalTileMap = new Dictionary<string, ShipMapTile>();
        }
    }
    public class InteriorRoom
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public FacilityType RoomType { get; set; }
        public List<InteriorObject> Objects { get; set; }
        public int WorldX { get; set; }
        public int WorldY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<InteriorTile> Tiles { get; set; }

        [JsonIgnore]
        public InteriorTile[,] TileGrid { get; private set; }

        public InteriorRoom()
        {
            Id = Guid.NewGuid();
            Objects = new List<InteriorObject>();
            Tiles = new List<InteriorTile>();
        }

        public void RebuildTileIndex()
        {
            if (Width <= 0 || Height <= 0)
            {
                TileGrid = null;
                return;
            }

            TileGrid = new InteriorTile[Width, Height];
            if (Tiles == null)
                return;

            foreach (InteriorTile tile in Tiles)
            {
                if (tile == null)
                    continue;

                if (tile.X < 0 || tile.Y < 0 || tile.X >= Width || tile.Y >= Height)
                    continue;

                TileGrid[tile.X, tile.Y] = tile;
            }
        }

        public InteriorTile GetTileFast(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return null;

            if (TileGrid == null)
                RebuildTileIndex();

            return TileGrid[x, y];
        }
    }

    public class InteriorTile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public InteriorTileType TileType { get; set; }
        public bool IsWalkable { get; set; }
        public Guid? ObjectId { get; set; }

        public InteriorTile()
        {
            IsWalkable = true;
        }
    }

    public class InteriorObject
    {
        public Guid Id { get; set; }
        public InteriorObjectType ObjectType { get; set; }
        public string Name { get; set; }

        public InteriorObject()
        {
            Id = Guid.NewGuid();
        }
    }
    public class ShipMapTile
    {
        public int X { get; set; }
        public int Y { get; set; }

        public InteriorTileType TileType { get; set; }
        public FacilityType FacilityType { get; set; }

        public bool IsWalkable { get; set; }

        public Guid? RoomId { get; set; }
        public Guid? ObjectId { get; set; }

        public List<Guid> RoomIds { get; set; }

        public ShipMapTile()
        {
            RoomIds = new List<Guid>();
        }
    }
}
