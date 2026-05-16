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
        public static Point GetWorkstationTargetPosition(
            Ship ship,
            Job job,
            Guid crewId,
            out Guid? selectedObjectId)
        {
            selectedObjectId = null;

            InteriorObjectType workstationType = CrewHelper.GetWorkstationType(job);

            if (workstationType == InteriorObjectType.None)
                return new Point(-1, -1);

            foreach (InteriorObject workstation in ship.Interior.ObjectLookup.Values)
            {
                if (workstation.ObjectType != workstationType)
                    continue;

                if (!IsWorkstationAvailable(workstation, crewId))
                    continue;

                Point targetPoint = GetTargetPositionFromInteractionDirections(
                    workstation.ParentRoom,
                    workstation);

                if (targetPoint.X >= 0 && targetPoint.Y >= 0)
                {
                    workstation.ReservedByCrewId = crewId;
                    selectedObjectId = workstation.Id;
                    return targetPoint;
                }
            }

            return new Point(-1, -1);
        }

        public static void MarkWorkstationOccupied(
            Ship ship,
            Crew crew)
        {
            if (!crew.ReservedObjectId.HasValue)
                return;

            InteriorObject obj = ship.Interior.GetObjectById(crew.ReservedObjectId.Value);

            if (obj == null)
                return;

            obj.ReservedByCrewId = null;
            obj.OccupiedByCrewId = crew.Id;

            crew.OccupiedObjectId = obj.Id;
            crew.ReservedObjectId = null;
        }

        public static void ReleaseWorkstation(
       Ship ship,
       Crew crew)
        {
            if (crew.ReservedObjectId.HasValue)
            {
                InteriorObject reservedObj =
                    ship.Interior.GetObjectById(crew.ReservedObjectId.Value);

                if (reservedObj != null &&
                    reservedObj.ReservedByCrewId == crew.Id)
                {
                    reservedObj.ReservedByCrewId = null;
                }

                crew.ReservedObjectId = null;
            }

            if (crew.OccupiedObjectId.HasValue)
            {
                InteriorObject occupiedObj =
                    ship.Interior.GetObjectById(crew.OccupiedObjectId.Value);

                if (occupiedObj != null &&
                    occupiedObj.OccupiedByCrewId == crew.Id)
                {
                    occupiedObj.OccupiedByCrewId = null;
                }

                crew.OccupiedObjectId = null;
            }
        }

        private static bool IsWorkstationAvailable(
            InteriorObject workstation,
            Guid crewId)
        {
            if (workstation.AllowMultipleCrew)
                return true;

            if (workstation.ReservedByCrewId.HasValue &&
                workstation.ReservedByCrewId.Value != crewId)
                return false;

            if (workstation.OccupiedByCrewId.HasValue &&
                workstation.OccupiedByCrewId.Value != crewId)
                return false;

            return true;
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

            return GetTargetPositionFromInteractionDirections(room, targetObject);
        }

        private static Point GetTargetPositionFromInteractionDirections(
      InteriorRoom room,
      InteriorObject targetObject)
        {
            List<Point> interactionPoints =
                InteractionHelper.GetInteractionTiles(targetObject);

            foreach (Point point in interactionPoints)
            {
                int localX = (int)point.X - room.WorldX;
                int localY = (int)point.Y - room.WorldY;

                InteriorTile tile = room.GetTileFast(localX, localY);

                if (tile == null)
                    continue;

                if (!tile.IsWalkable)
                    continue;

                return point;
            }

            return new Point(-1, -1);
        }

        // NO LONGER IN USE
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

        // NO LONGER IN USE
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
