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

    // what the thing actually is / does
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

    // What is drawn on the map
    public enum InteriorTileType 
    {
        // Workstations
        Cockpit,
        WeaponsConsole,
        MedicalConsole,

        Floor, 
        Wall, 
        Door, 
        Bed, 
        StorageBox, 
        Workbench, 
        KitchenCounter,
        Table, 
        Sofa
    }

    public enum InteractionDirection
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public class ShipInterior
    {
        public List<InteriorRoom> Rooms { get; set; }

        [JsonIgnore]
        public Dictionary<string, ShipMapTile> GlobalTileMap { get; set; }

        [JsonIgnore]
        public Dictionary<Guid, InteriorObject> ObjectLookup { get; set; }

        public ShipInterior()
        {
            Rooms = new List<InteriorRoom>();
            GlobalTileMap = new Dictionary<string, ShipMapTile>();
            ObjectLookup = new Dictionary<Guid, InteriorObject>();
        }

        public void RebuildObjectLookup()
        {
            ObjectLookup = new Dictionary<Guid, InteriorObject>();

            foreach (InteriorRoom room in Rooms)
            {
                foreach (InteriorObject obj in room.Objects)
                {
                    obj.ParentRoom = room;
                    ObjectLookup[obj.Id] = obj;
                }
            }
        }

        public InteriorObject GetObjectById(Guid objectId)
        {
            if (ObjectLookup == null)
                RebuildObjectLookup();

            InteriorObject obj;
            ObjectLookup.TryGetValue(objectId, out obj);

            return obj;
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
        public InteriorTileType TileType { get; set; }

        public string Name { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        [JsonIgnore]
        public InteriorRoom ParentRoom { get; set; }

        [JsonIgnore]
        public int WorldX => ParentRoom.WorldX + X;

        [JsonIgnore]
        public int WorldY => ParentRoom.WorldY + Y;

        public List<InteractionDirection> AllowedInteractionDirections { get; set; }

        public bool AllowMultipleCrew { get; set; }

        public Guid? ReservedByCrewId { get; set; }

        public Guid? OccupiedByCrewId { get; set; }

        public InteriorObject()
        {
            Id = Guid.NewGuid();
            AllowedInteractionDirections = new List<InteractionDirection>();

            AllowMultipleCrew = false;
            ReservedByCrewId = null;
            OccupiedByCrewId = null;
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
