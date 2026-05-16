using Space_RPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG.Helpers
{
    public static class InteractionHelper
    {
        public static string GetTileKey(int x, int y)
        {
            return x + "," + y;
        }

        public static Point? GetRandomAvailableInteractionTile(
            InteriorObject obj,
            Guid crewId,
            Random random)
        {
            List<Point> availableTiles = GetInteractionTiles(obj)
                .Where(p =>
                {
                    string key = GetTileKey((int)p.X, (int)p.Y);

                    bool reservedByOther =
                        obj.ReservedInteractionTiles.ContainsKey(key) &&
                        obj.ReservedInteractionTiles[key] != crewId;

                    bool occupiedByOther =
                        obj.OccupiedInteractionTiles.ContainsKey(key) &&
                        obj.OccupiedInteractionTiles[key] != crewId;

                    return !reservedByOther && !occupiedByOther;
                })
                .OrderBy(x => random.Next())
                .ToList();

            if (availableTiles.Count == 0)
                return null;

            return availableTiles[0];
        }
        public static List<Point> GetInteractionTiles(InteriorObject obj)
        {
            List<Point> result = new List<Point>();

            foreach (InteractionDirection direction in obj.AllowedInteractionDirections)
            {
                switch (direction)
                {
                    case InteractionDirection.Top:
                        for (int x = obj.WorldX; x < obj.WorldX + obj.Width; x++)
                        {
                            result.Add(new Point(x, obj.WorldY - 1));
                        }
                        break;

                    case InteractionDirection.Bottom:
                        for (int x = obj.WorldX; x < obj.WorldX + obj.Width; x++)
                        {
                            result.Add(new Point(x, obj.WorldY + obj.Height));
                        }
                        break;

                    case InteractionDirection.Left:
                        for (int y = obj.WorldY; y < obj.WorldY + obj.Height; y++)
                        {
                            result.Add(new Point(obj.WorldX - 1, y));
                        }
                        break;

                    case InteractionDirection.Right:
                        for (int y = obj.WorldY; y < obj.WorldY + obj.Height; y++)
                        {
                            result.Add(new Point(obj.WorldX + obj.Width, y));
                        }
                        break;
                }
            }

            return result;
        }
    }
}
