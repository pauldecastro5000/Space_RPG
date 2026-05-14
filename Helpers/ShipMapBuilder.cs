using Space_RPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Helpers
{
    public static class ShipMapBuilder
    {
        public static void RebuildGlobalTileMap(Ship ship)
        {
            if (ship == null || ship.Interior == null)
                return;

            if (ship.Interior.GlobalTileMap == null)
                ship.Interior.GlobalTileMap = new Dictionary<string, ShipMapTile>();

            ship.Interior.GlobalTileMap.Clear();

            foreach (InteriorRoom room in ship.Interior.Rooms)
            {
                foreach (InteriorTile localTile in room.Tiles)
                {
                    int globalX = room.WorldX + localTile.X;
                    int globalY = room.WorldY + localTile.Y;

                    string key = GetKey(globalX, globalY);

                    if (!ship.Interior.GlobalTileMap.ContainsKey(key))
                    {
                        ship.Interior.GlobalTileMap[key] = new ShipMapTile
                        {
                            X = globalX,
                            Y = globalY,
                            TileType = localTile.TileType,
                            FacilityType = room.RoomType,
                            IsWalkable = localTile.IsWalkable,
                            RoomId = room.Id,
                            ObjectId = localTile.ObjectId
                        };

                        ship.Interior.GlobalTileMap[key].RoomIds.Add(room.Id);
                    }
                    else
                    {
                        MergeTile(
                            ship.Interior.GlobalTileMap[key],
                            room,
                            localTile);
                    }
                }
            }
        }

        private static void MergeTile(
            ShipMapTile existingTile,
            InteriorRoom room,
            InteriorTile newTile)
        {
            if (!existingTile.RoomIds.Contains(room.Id))
                existingTile.RoomIds.Add(room.Id);

            // Door has highest priority.
            if (newTile.TileType == InteriorTileType.Door)
            {
                existingTile.TileType = InteriorTileType.Door;
                existingTile.IsWalkable = true;
                existingTile.FacilityType = room.RoomType;
                existingTile.RoomId = room.Id;
                return;
            }

            // Object tile has second priority.
            if (newTile.ObjectId.HasValue)
            {
                existingTile.TileType = newTile.TileType;
                existingTile.IsWalkable = newTile.IsWalkable;
                existingTile.ObjectId = newTile.ObjectId;
                existingTile.FacilityType = room.RoomType;
                existingTile.RoomId = room.Id;
                return;
            }

            // If one side is floor and the other is wall, keep floor.
            // This prevents shared walls from becoming double-thick.
            if (existingTile.TileType == InteriorTileType.Wall &&
                newTile.TileType == InteriorTileType.Floor)
            {
                existingTile.TileType = InteriorTileType.Floor;
                existingTile.IsWalkable = true;
                existingTile.FacilityType = room.RoomType;
                existingTile.RoomId = room.Id;
                return;
            }

            // If existing is floor and new is wall, keep floor.
            if (existingTile.TileType == InteriorTileType.Floor &&
                newTile.TileType == InteriorTileType.Wall)
            {
                return;
            }

            // If both are walls, keep only one wall.
            if (existingTile.TileType == InteriorTileType.Wall &&
                newTile.TileType == InteriorTileType.Wall)
            {
                existingTile.IsWalkable = false;
                return;
            }
        }

        public static string GetKey(int x, int y)
        {
            return x + "," + y;
        }

        public static ShipMapTile GetTile(Ship ship, int x, int y)
        {
            if (ship == null ||
                ship.Interior == null ||
                ship.Interior.GlobalTileMap == null)
                return null;

            string key = GetKey(x, y);

            if (!ship.Interior.GlobalTileMap.ContainsKey(key))
                return null;

            return ship.Interior.GlobalTileMap[key];
        }
    }
}
