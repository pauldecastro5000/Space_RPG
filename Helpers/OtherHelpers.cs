using Space_RPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG.Helpers
{
    internal class OtherHelpers
    {
        public static Point GetObjectCenter(InteriorRoom room, Guid objectId)
        {
            List<InteriorTile> tiles =
                room.Tiles
                    .Where(t => t.ObjectId == objectId)
                    .ToList();

            int minX = tiles.Min(t => t.X);
            int maxX = tiles.Max(t => t.X);

            int minY = tiles.Min(t => t.Y);
            int maxY = tiles.Max(t => t.Y);

            return new Point(
                (minX + maxX) / 2,
                (minY + maxY) / 2);
        }
    }
}
