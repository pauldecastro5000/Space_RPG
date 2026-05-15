using Space_RPG.Helpers;
using Space_RPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Space_RPG.Services
{
    public class PathNode
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get { return GCost + HCost; } }

        public PathNode Parent { get; set; }
    }

    public static class PathfindingService
    {
        public static List<ShipMapTile> FindPath(
            Ship ship,
            int startX,
            int startY,
            int targetX,
            int targetY)
        {
            if (ship == null || ship.Interior == null || ship.Interior.GlobalTileMap == null)
                return new List<ShipMapTile>();

            ShipMapTile startTile = ShipMapBuilder.GetTile(ship, startX, startY);
            ShipMapTile targetTile = ShipMapBuilder.GetTile(ship, targetX, targetY);

            if (startTile == null || targetTile == null)
                return new List<ShipMapTile>();

            if (!startTile.IsWalkable || !targetTile.IsWalkable)
                return new List<ShipMapTile>();

            List<PathNode> openList = new List<PathNode>();
            HashSet<string> closedList = new HashSet<string>();

            PathNode startNode = new PathNode
            {
                X = startX,
                Y = startY,
                GCost = 0,
                HCost = GetDistance(startX, startY, targetX, targetY)
            };

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                PathNode currentNode = openList
                    .OrderBy(n => n.FCost)
                    .ThenBy(n => n.HCost)
                    .First();

                openList.Remove(currentNode);
                closedList.Add(GetKey(currentNode.X, currentNode.Y));

                if (currentNode.X == targetX && currentNode.Y == targetY)
                    return BuildPath(ship, currentNode);

                foreach (PathNode neighbour in GetNeighbours(ship, currentNode))
                {
                    string neighbourKey = GetKey(neighbour.X, neighbour.Y);

                    if (closedList.Contains(neighbourKey))
                        continue;

                    int newMovementCost = currentNode.GCost + 10;

                    PathNode existingNode = openList.FirstOrDefault(n =>
                        n.X == neighbour.X &&
                        n.Y == neighbour.Y);

                    if (existingNode == null)
                    {
                        neighbour.GCost = newMovementCost;
                        neighbour.HCost = GetDistance(neighbour.X, neighbour.Y, targetX, targetY);
                        neighbour.Parent = currentNode;

                        openList.Add(neighbour);
                    }
                    else if (newMovementCost < existingNode.GCost)
                    {
                        existingNode.GCost = newMovementCost;
                        existingNode.Parent = currentNode;
                    }
                }
            }

            return new List<ShipMapTile>();
        }

        private static List<PathNode> GetNeighbours(Ship ship, PathNode node)
        {
            List<PathNode> neighbours = new List<PathNode>();

            AddNeighbour(ship, neighbours, node.X, node.Y - 1); // Up
            AddNeighbour(ship, neighbours, node.X, node.Y + 1); // Down
            AddNeighbour(ship, neighbours, node.X - 1, node.Y); // Left
            AddNeighbour(ship, neighbours, node.X + 1, node.Y); // Right

            return neighbours;
        }

        private static void AddNeighbour(
            Ship ship,
            List<PathNode> neighbours,
            int x,
            int y)
        {
            ShipMapTile tile = ShipMapBuilder.GetTile(ship, x, y);

            if (tile == null)
                return;

            if (!tile.IsWalkable)
                return;

            neighbours.Add(new PathNode
            {
                X = x,
                Y = y
            });
        }

        private static List<ShipMapTile> BuildPath(Ship ship, PathNode endNode)
        {
            List<ShipMapTile> path = new List<ShipMapTile>();

            PathNode currentNode = endNode;

            while (currentNode != null)
            {
                ShipMapTile tile = ShipMapBuilder.GetTile(
                    ship,
                    currentNode.X,
                    currentNode.Y);

                if (tile != null)
                    path.Add(tile);

                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private static int GetDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        private static string GetKey(int x, int y)
        {
            return x + "," + y;
        }
    }
}