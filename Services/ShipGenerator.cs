using Space_RPG.Helpers;
using Space_RPG.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG.Services
{
    public static class ShipGenerator
    {
        public static Ship GenerateDefaultShip()
        {
            Ship ship = new Ship
            {
                Interior = new ShipInterior(),
                Location = new Point(0, 0),
                Food = 1000
            };

            // IMPORTANT:
            // Rooms/corridors overlap by 1 tile when touching.
            // This creates 1 shared wall layer only.
            //
            // Example:
            // Room A right wall = X 20
            // Room B left wall  = X 20
            //
            // Door is placed on the shared wall tile.

            // Main Rooms
            InteriorRoom mainDeck = CreateRoom("Main Deck", FacilityType.MainDeck, 10, 8, 21, 2);

            InteriorRoom cafeteria = CreateRoom("Cafeteria", FacilityType.Cafeteria, 8, 8, 11, 6);

            InteriorRoom medicalBay = CreateRoom("Medical Bay", FacilityType.MedicalBay, 8, 8, 33, 6);


            InteriorRoom femaleCrewQuarter = CreateRoom("Female Crew Quarter", FacilityType.FemaleCrewQuarters, 10, 8, 3, 19);

            InteriorRoom maleCrewQuarter = CreateRoom("Male Crew Quarter", FacilityType.MaleCrewQuarters, 10, 8, 39, 19);


            InteriorRoom captainsRoom = CreateRoom("Captains Room", FacilityType.CaptainsRoom, 7, 5, 15, 21);

            InteriorRoom engineRoom = CreateRoom("Engine Room", FacilityType.EngineRoom, 7, 5, 30, 21);


            InteriorRoom garden = CreateRoom("Garden", FacilityType.Garden, 8, 7, 10, 28);

            InteriorRoom armoury = CreateRoom("Armoury", FacilityType.Armoury, 8, 7, 34, 28);


            InteriorRoom cargo = CreateRoom("Cargo", FacilityType.Cargo, 11, 8, 20, 32);


            // Corridors
            InteriorRoom topLeftCorridor = CreateRoom("Corridor", FacilityType.Corridor, 7, 3, 18, 11);

            InteriorRoom topRightCorridor = CreateRoom("Corridor", FacilityType.Corridor, 7, 3, 27, 11);

            InteriorRoom leftHallway = CreateRoom("Corridor", FacilityType.Corridor, 16, 4, 9, 16);

            InteriorRoom rightHallway = CreateRoom("Corridor", FacilityType.Corridor, 16, 4, 27, 16);

            InteriorRoom centerVertical = CreateRoom("Corridor", FacilityType.Corridor, 4, 24, 24, 9);

            InteriorRoom leftMidConnector = CreateRoom("Corridor", FacilityType.Corridor, 3, 3, 19, 19);

            InteriorRoom rightMidConnector = CreateRoom("Corridor", FacilityType.Corridor, 3, 3, 30, 19);

            InteriorRoom bottomLeftHallway = CreateRoom("Corridor", FacilityType.Corridor, 8, 3, 17, 28);

            InteriorRoom bottomRightHallway = CreateRoom("Corridor", FacilityType.Corridor, 8, 3, 27, 28);

            //InteriorRoom cargoConnector = CreateRoom("Corridor", FacilityType.Corridor, 3, 4, 25, 30);


            AddObjects(mainDeck, medicalBay, femaleCrewQuarter, cafeteria, cargo);

            ship.Interior.Rooms.Add(mainDeck);

            ship.Interior.Rooms.Add(cafeteria);
            ship.Interior.Rooms.Add(medicalBay);

            ship.Interior.Rooms.Add(femaleCrewQuarter);
            ship.Interior.Rooms.Add(maleCrewQuarter);

            ship.Interior.Rooms.Add(captainsRoom);
            ship.Interior.Rooms.Add(engineRoom);

            ship.Interior.Rooms.Add(garden);
            ship.Interior.Rooms.Add(armoury);

            ship.Interior.Rooms.Add(cargo);

            // Corridors
            ship.Interior.Rooms.Add(topLeftCorridor);
            ship.Interior.Rooms.Add(topRightCorridor);

            ship.Interior.Rooms.Add(leftHallway);
            ship.Interior.Rooms.Add(rightHallway);

            ship.Interior.Rooms.Add(centerVertical);

            ship.Interior.Rooms.Add(leftMidConnector);
            ship.Interior.Rooms.Add(rightMidConnector);

            ship.Interior.Rooms.Add(bottomLeftHallway);
            ship.Interior.Rooms.Add(bottomRightHallway);

            //ship.Interior.Rooms.Add(cargoConnector);

            AddSharedWallDoors(ship.Interior.Rooms);

            foreach (InteriorRoom room in ship.Interior.Rooms)
                room.RebuildTileIndex();

            ShipMapBuilder.RebuildGlobalTileMap(ship);

            return ship;
        }

        private static InteriorRoom CreateRoom(
            string name,
            FacilityType roomType,
            int width,
            int height,
            int worldX,
            int worldY)
        {



            InteriorRoom room = new InteriorRoom
            {
                Id = Guid.NewGuid(),
                Name = name,
                RoomType = roomType,
                Width = width,
                Height = height,
                WorldX = worldX,
                WorldY = worldY,
                Objects = new List<InteriorObject>(),
                Tiles = new List<InteriorTile>()
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isWall =
                        x == 0 ||
                        y == 0 ||
                        x == width - 1 ||
                        y == height - 1;

                    room.Tiles.Add(new InteriorTile
                    {
                        X = x,
                        Y = y,
                        TileType = isWall ? InteriorTileType.Wall : InteriorTileType.Floor,
                        IsWalkable = !isWall
                    });
                }
            }

            return room;
        }

        private static InteriorRoom CreateCorridor(
            string name,
            int width,
            int height,
            int worldX,
            int worldY)
        {
            return CreateRoom(
                name,
                FacilityType.Corridor,
                width,
                height,
                worldX,
                worldY);
        }

        private static void AddSharedWallDoors(List<InteriorRoom> rooms)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                for (int j = i + 1; j < rooms.Count; j++)
                {
                    TryCreateSharedWallDoor(rooms[i], rooms[j]);
                }
            }
        }

        private static void TryCreateSharedWallDoor(InteriorRoom a, InteriorRoom b)
        {
            int aLeft = a.WorldX;
            int aRight = a.WorldX + a.Width - 1;
            int aTop = a.WorldY;
            int aBottom = a.WorldY + a.Height - 1;

            int bLeft = b.WorldX;
            int bRight = b.WorldX + b.Width - 1;
            int bTop = b.WorldY;
            int bBottom = b.WorldY + b.Height - 1;

            // A left/right shared wall with B
            if (aRight == bLeft || bRight == aLeft)
            {
                int sharedX = aRight == bLeft ? aRight : bRight;

                int overlapTop = Math.Max(aTop + 1, bTop + 1);
                int overlapBottom = Math.Min(aBottom - 1, bBottom - 1);

                if (overlapTop <= overlapBottom)
                {
                    int doorY = (overlapTop + overlapBottom) / 2;

                    SetDoorByWorldPosition(a, sharedX, doorY);
                    SetDoorByWorldPosition(b, sharedX, doorY);
                }
            }

            // A top/bottom shared wall with B
            if (aBottom == bTop || bBottom == aTop)
            {
                int sharedY = aBottom == bTop ? aBottom : bBottom;

                int overlapLeft = Math.Max(aLeft + 1, bLeft + 1);
                int overlapRight = Math.Min(aRight - 1, bRight - 1);

                if (overlapLeft <= overlapRight)
                {
                    int doorX = (overlapLeft + overlapRight) / 2;

                    SetDoorByWorldPosition(a, doorX, sharedY);
                    SetDoorByWorldPosition(b, doorX, sharedY);
                }
            }
        }

        private static void SetDoorByWorldPosition(InteriorRoom room, int worldX, int worldY)
        {
            int localX = worldX - room.WorldX;
            int localY = worldY - room.WorldY;

            InteriorTile tile = room.Tiles.FirstOrDefault(t => t.X == localX && t.Y == localY);

            if (tile == null)
                return;

            tile.TileType = InteriorTileType.Door;
            tile.IsWalkable = true;
        }

        private static void AddObjects(
            InteriorRoom mainDeck,
            InteriorRoom medical,
            InteriorRoom crew,
            InteriorRoom cafeteria,
            InteriorRoom cargo)
        {
            AddObject(mainDeck, InteriorObjectType.Cockpit, "Cockpit", 4, 1);
            AddObject(mainDeck, InteriorObjectType.WeaponsConsole, "Weapons Console", 2, 3);
            AddObject(mainDeck, InteriorObjectType.WeaponsConsole, "Weapons Console", 6, 3);

            AddObject(medical, InteriorObjectType.MedicalConsole, "Medical Console", 3, 1);
            AddObject(medical, InteriorObjectType.Bed, "Medical Bed", 2, 4, InteriorTileType.Bed);
            AddObject(medical, InteriorObjectType.Bed, "Medical Bed", 5, 4, InteriorTileType.Bed);

            AddObject(crew, InteriorObjectType.Bed, "Bed 1", 1, 1, InteriorTileType.Bed);
            AddObject(crew, InteriorObjectType.Bed, "Bed 2", 7, 1, InteriorTileType.Bed);
            AddObject(crew, InteriorObjectType.Bed, "Bed 3", 1, 5, InteriorTileType.Bed);
            AddObject(crew, InteriorObjectType.Bed, "Bed 4", 7, 5, InteriorTileType.Bed);
            AddObject(crew, InteriorObjectType.Table, "Small Table", 4, 3, InteriorTileType.Table);

            AddObject(cafeteria, InteriorObjectType.Stove, "Stove", 1, 1, InteriorTileType.KitchenCounter);
            AddObject(cafeteria, InteriorObjectType.Table, "Dining Table", 5, 3, InteriorTileType.Table);
            AddObject(cafeteria, InteriorObjectType.Sofa, "Sofa", 8, 5, InteriorTileType.Sofa);

            AddObject(cargo, InteriorObjectType.None, "Storage Box 1", 2, 2, InteriorTileType.StorageBox);
            AddObject(cargo, InteriorObjectType.None, "Storage Box 2", 5, 2, InteriorTileType.StorageBox);
            AddObject(cargo, InteriorObjectType.None, "Storage Box 3", 8, 2, InteriorTileType.StorageBox);
            AddObject(cargo, InteriorObjectType.None, "Workbench", 5, 5, InteriorTileType.Workbench);
        }

        private static void AddObject(
            InteriorRoom room,
            InteriorObjectType objectType,
            string name,
            int x,
            int y,
            InteriorTileType tileType = InteriorTileType.Floor)
        {
            InteriorTile tile = room.Tiles.FirstOrDefault(t => t.X == x && t.Y == y);

            if (tile == null)
                return;

            InteriorObject obj = new InteriorObject
            {
                Id = Guid.NewGuid(),
                ObjectType = objectType,
                Name = name
            };

            room.Objects.Add(obj);

            tile.ObjectId = obj.Id;
            tile.TileType = tileType;
            tile.IsWalkable = false;
        }
    }
}
