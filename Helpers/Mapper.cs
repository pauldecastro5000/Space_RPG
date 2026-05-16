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
        private static readonly Random _random = new Random();
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

            List<InteriorObject> workstations = ship.Interior.ObjectLookup.Values
                .Where(o => o.ObjectType == workstationType)
                .OrderBy(o => _random.Next())
                .ToList();

            foreach (InteriorObject workstation in workstations)
            {
                if (!IsWorkstationAvailable(workstation, crewId))
                    continue;

                Point? targetPoint =
                    InteractionHelper.GetRandomAvailableInteractionTile(
                        workstation,
                        crewId,
                        _random);

                if (targetPoint.HasValue)
                {
                    string key = InteractionHelper.GetTileKey(
                        (int)targetPoint.Value.X,
                        (int)targetPoint.Value.Y);

                    workstation.ReservedInteractionTiles[key] = crewId;

                    selectedObjectId = workstation.Id;

                    return targetPoint.Value;
                }
            }

            return new Point(-1, -1);
        }
        public static Point GetObjectInteractionTargetPosition(
    Ship ship,
    FacilityType roomType,
    InteriorObjectType objectType,
    Guid crewId,
    out Guid? selectedObjectId)
        {
            selectedObjectId = null;

            List<InteriorObject> objects = ship.Interior.ObjectLookup.Values
                .Where(o =>
                    o.ParentRoom != null &&
                    o.ParentRoom.RoomType == roomType &&
                    o.ObjectType == objectType)
                .OrderBy(o => _random.Next())
                .ToList();

            foreach (InteriorObject obj in objects)
            {
                Point? targetPoint =
                    InteractionHelper.GetRandomAvailableInteractionTile(
                        obj,
                        crewId,
                        _random);

                if (!targetPoint.HasValue)
                    continue;

                string key = InteractionHelper.GetTileKey(
                    (int)targetPoint.Value.X,
                    (int)targetPoint.Value.Y);

                obj.ReservedInteractionTiles[key] = crewId;

                selectedObjectId = obj.Id;

                return targetPoint.Value;
            }

            return new Point(-1, -1);
        }
        public static void MarkWorkstationOccupied(Ship ship, Crew crew)
        {
            if (!crew.ReservedObjectId.HasValue)
                return;

            InteriorObject obj = ship.Interior.GetObjectById(crew.ReservedObjectId.Value);

            if (obj == null)
                return;

            string reservedKey = obj.ReservedInteractionTiles
                .FirstOrDefault(x => x.Value == crew.Id)
                .Key;

            if (reservedKey == null)
                return;

            obj.ReservedInteractionTiles.Remove(reservedKey);
            obj.OccupiedInteractionTiles[reservedKey] = crew.Id;

            crew.OccupiedObjectId = obj.Id;
            crew.ReservedObjectId = null;
        }

        public static void ReleaseWorkstation(Ship ship, Crew crew)
        {
            Guid? objectId = crew.ReservedObjectId ?? crew.OccupiedObjectId;

            if (!objectId.HasValue)
                return;

            InteriorObject obj = ship.Interior.GetObjectById(objectId.Value);

            if (obj == null)
                return;

            RemoveCrewFromTileDictionary(obj.ReservedInteractionTiles, crew.Id);
            RemoveCrewFromTileDictionary(obj.OccupiedInteractionTiles, crew.Id);

            crew.ReservedObjectId = null;
            crew.OccupiedObjectId = null;
        }

        private static void RemoveCrewFromTileDictionary(
            Dictionary<string, Guid> dictionary,
            Guid crewId)
        {
            string keyToRemove = dictionary
                .FirstOrDefault(x => x.Value == crewId)
                .Key;

            if (keyToRemove != null)
                dictionary.Remove(keyToRemove);
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
