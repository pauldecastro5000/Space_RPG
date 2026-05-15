using Space_RPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG.Helpers
{
    public static class Mapper
    {
        public static Point GetWorkstationTargetPosition(Ship ship, Job job)
        {
            InteriorObjectType workstationType = CrewHelper.GetWorkstationType(job);

            if (workstationType == InteriorObjectType.None)
                return new Point(-1, -1);

            foreach (InteriorRoom room in ship.Interior.Rooms)
            {
                InteriorObject workstation = room.Objects
                    .FirstOrDefault(o => o.ObjectType == workstationType);

                if (workstation == null)
                    continue;

                List<InteriorTile> objectTiles = room.Tiles
                    .Where(t => t.ObjectId == workstation.Id)
                    .ToList();

                if (objectTiles.Count == 0)
                    continue;

                List<InteriorTile> walkableNeighborTiles = new List<InteriorTile>();

                foreach (InteriorTile objectTile in objectTiles)
                {
                    walkableNeighborTiles.AddRange(
                        GetWalkableNeighborTiles(room, objectTile.X, objectTile.Y));
                }

                InteriorTile targetTile = walkableNeighborTiles
                    .FirstOrDefault();

                if (targetTile == null)
                    continue;

                return new Point(
                    room.WorldX + targetTile.X,
                    room.WorldY + targetTile.Y);
            }

            return new Point(-1, -1);
        }

        public static Point GetInteractionTargetPosition(
    Ship ship,
    FacilityType roomType,
    InteriorObjectType objectType)
        {
            InteriorRoom room = ship.Interior.Rooms
                .FirstOrDefault(r => r.RoomType == roomType);

            if (room == null)
                return new Point(-1, -1);

            InteriorObject targetObject = room.Objects
                .FirstOrDefault(o => o.ObjectType == objectType);

            if (targetObject == null)
                return new Point(-1, -1);

            List<InteriorTile> objectTiles = room.Tiles
                .Where(t => t.ObjectId == targetObject.Id)
                .ToList();

            if (objectTiles.Count == 0)
                return new Point(-1, -1);

            List<InteriorTile> walkableNeighborTiles = new List<InteriorTile>();

            foreach (InteriorTile objectTile in objectTiles)
            {
                walkableNeighborTiles.AddRange(GetWalkableNeighborTiles(room, objectTile.X, objectTile.Y));
            }

            InteriorTile targetTile = walkableNeighborTiles
                .Distinct()
                .FirstOrDefault();

            if (targetTile == null)
                return new Point(-1, -1);

            return new Point(
     room.WorldX + targetTile.X,
     room.WorldY + targetTile.Y);
        }

        private static List<InteriorTile> GetWalkableNeighborTiles(
    InteriorRoom room,
    int x,
    int y)
        {
            List<InteriorTile> neighborTiles = new List<InteriorTile>();

            AddIfWalkable(room, neighborTiles, x, y - 1);
            AddIfWalkable(room, neighborTiles, x + 1, y);
            AddIfWalkable(room, neighborTiles, x, y + 1);
            AddIfWalkable(room, neighborTiles, x - 1, y);

            return neighborTiles;
        }

        private static void AddIfWalkable(
            InteriorRoom room,
            List<InteriorTile> tiles,
            int x,
            int y)
        {
            InteriorTile tile = room.Tiles
                .FirstOrDefault(t => t.X == x && t.Y == y);

            if (tile == null)
                return;

            if (!tile.IsWalkable)
                return;

            tiles.Add(tile);
        }
    }
}
