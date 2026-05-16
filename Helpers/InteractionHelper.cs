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
        public static List<Point> GetInteractionTiles(InteriorObject obj)
        {
            List<Point> result = new List<Point>();

            foreach (InteractionDirection direction in obj.AllowedInteractionDirections)
            {
                switch (direction)
                {
                    case InteractionDirection.Top:
                        result.Add(new Point(obj.WorldX, obj.WorldY - 1));
                        break;

                    case InteractionDirection.Bottom:
                        result.Add(new Point(obj.WorldX, obj.WorldY + 1));
                        break;

                    case InteractionDirection.Left:
                        result.Add(new Point(obj.WorldX - 1, obj.WorldY));
                        break;

                    case InteractionDirection.Right:
                        result.Add(new Point(obj.WorldX + 1, obj.WorldY));
                        break;
                }
            }

            return result;
        }
    }
}
