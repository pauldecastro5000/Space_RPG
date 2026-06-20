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

            ship.Interior.RebuildObjectLookup();

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
            if (rooms == null || rooms.Count == 0)
                return;

            foreach (InteriorRoom room in rooms)
                room.RebuildTileIndex();

            for (int i = 0; i < rooms.Count; i++)
            {
                for (int j = i + 1; j < rooms.Count; j++)
                {
                    CreateDoorsOnAllInternalSharedWalls(rooms, rooms[i], rooms[j]);
                }
            }
        }

        private static void CreateDoorsOnAllInternalSharedWalls(List<InteriorRoom> rooms, InteriorRoom a, InteriorRoom b)
        {
            int overlapLeft = Math.Max(a.WorldX, b.WorldX);
            int overlapRight = Math.Min(a.WorldX + a.Width - 1, b.WorldX + b.Width - 1);
            int overlapTop = Math.Max(a.WorldY, b.WorldY);
            int overlapBottom = Math.Min(a.WorldY + a.Height - 1, b.WorldY + b.Height - 1);

            if (overlapLeft > overlapRight || overlapTop > overlapBottom)
                return;

            for (int worldY = overlapTop; worldY <= overlapBottom; worldY++)
            {
                for (int worldX = overlapLeft; worldX <= overlapRight; worldX++)
                {
                    InteriorTile tileA = GetTileByWorldPosition(a, worldX, worldY);
                    InteriorTile tileB = GetTileByWorldPosition(b, worldX, worldY);

                    if (tileA == null || tileB == null)
                        continue;

                    // Only room wall + room wall overlap can become a generated door.
                    // Object tiles or existing floor overlaps should not be changed here.
                    if (tileA.TileType != InteriorTileType.Wall || tileB.TileType != InteriorTileType.Wall)
                        continue;

                    // A shared wall is internal only when there is usable ship space
                    // on both opposite sides of the wall tile.
                    //
                    // Vertical shared wall example:
                    //     Room A floor | shared wall | Room B floor
                    //
                    // Horizontal shared wall example:
                    //     Room A floor
                    //     shared wall
                    //     Room B floor
                    //
                    // If only one side has ship space and the other side is empty,
                    // that wall is part of the external hull and must stay as a wall.
                    bool hasInteriorSpaceLeftAndRight =
                        IsInteriorSpace(rooms, worldX - 1, worldY) &&
                        IsInteriorSpace(rooms, worldX + 1, worldY);

                    bool hasInteriorSpaceTopAndBottom =
                        IsInteriorSpace(rooms, worldX, worldY - 1) &&
                        IsInteriorSpace(rooms, worldX, worldY + 1);

                    if (hasInteriorSpaceLeftAndRight || hasInteriorSpaceTopAndBottom)
                    {
                        SetDoorByWorldPosition(a, worldX, worldY);
                        SetDoorByWorldPosition(b, worldX, worldY);
                    }
                }
            }
        }

        private static InteriorTile GetTileByWorldPosition(InteriorRoom room, int worldX, int worldY)
        {
            if (room == null)
                return null;

            int localX = worldX - room.WorldX;
            int localY = worldY - room.WorldY;

            if (localX < 0 || localY < 0 || localX >= room.Width || localY >= room.Height)
                return null;

            return room.GetTileFast(localX, localY);
        }

        private static bool IsInteriorSpace(List<InteriorRoom> rooms, int worldX, int worldY)
        {
            foreach (InteriorRoom room in rooms)
            {
                InteriorTile tile = GetTileByWorldPosition(room, worldX, worldY);

                if (tile == null)
                    continue;

                if (tile.TileType != InteriorTileType.Wall)
                    return true;
            }

            return false;
        }

        private static void SetDoorByWorldPosition(InteriorRoom room, int worldX, int worldY)
        {
            InteriorTile tile = GetTileByWorldPosition(room, worldX, worldY);

            if (tile == null)
                return;

            tile.TileType = InteriorTileType.Door;
            tile.IsWalkable = true;
        }

        public static Ship GenerateShipFromFile(string filePath)
        {
            ShipDesign design = ShipDesignLoader.LoadFromFile(filePath);

            if (design == null)
                return GenerateDefaultShip();

            return GenerateShipFromDesign(design);
        }

        public static Ship GenerateShipFromDesign(ShipDesign design)
        {
            Ship ship = new Ship
            {
                Interior = new ShipInterior(),
                Location = new Point(0, 0),
                Food = 1000
            };

            foreach (ShipRoomDesign roomDesign in design.Rooms)
            {
                InteriorRoom room = CreateRoom(
                    roomDesign.Name,
                    roomDesign.RoomType,
                    roomDesign.Width,
                    roomDesign.Height,
                    roomDesign.WorldX,
                    roomDesign.WorldY);

                foreach (ShipObjectDesign objectDesign in roomDesign.Objects)
                {
                    AddObject(
                        room,
                        objectDesign.ObjectType,
                        objectDesign.Name,
                        objectDesign.X,
                        objectDesign.Y,
                        objectDesign.TileType,
                        objectDesign.Width,
                        objectDesign.Height,
                        objectDesign.AllowMultipleCrew,
                        objectDesign.InteractionDirections);
                }

                ship.Interior.Rooms.Add(room);
            }

            AddSharedWallDoors(ship.Interior.Rooms);

            foreach (InteriorRoom room in ship.Interior.Rooms)
                room.RebuildTileIndex();

            ship.Interior.RebuildObjectLookup();

            ShipMapBuilder.RebuildGlobalTileMap(ship);

            return ship;
        }

        private static void AddObjects(
            InteriorRoom mainDeck,
            InteriorRoom medical,
            InteriorRoom crew,
            InteriorRoom cafeteria,
            InteriorRoom cargo)
        {
            var Interaction_Bottom = new List<InteractionDirection>() { InteractionDirection.Bottom };
            var Interaction_Left = new List<InteractionDirection>() { InteractionDirection.Left };
            var Interaction_Right = new List<InteractionDirection>() { InteractionDirection.Right };
            var Interaction_Top = new List<InteractionDirection>() { InteractionDirection.Top };
            var IntAct_LR = new List<InteractionDirection>() {
                InteractionDirection.Left,
                InteractionDirection.Right };
            var IntAct_ALL = new List<InteractionDirection>() { InteractionDirection.Top, 
                InteractionDirection.Bottom, 
                InteractionDirection.Left, 
                InteractionDirection.Right };


            AddObject(mainDeck, InteriorObjectType.Cockpit, "Cockpit", 4, 5, InteriorTileType.Cockpit, 2, 1);

            AddObject(mainDeck, InteriorObjectType.WeaponsConsole, "Weapons Console", 2, 1, InteriorTileType.WeaponsConsole, 2, 1);
            AddObject(mainDeck, InteriorObjectType.WeaponsConsole, "Weapons Console", 5, 1, InteriorTileType.WeaponsConsole, 2, 1);
            AddObject(mainDeck, InteriorObjectType.WeaponsConsole, "Weapons Console", 2, 3, InteriorTileType.WeaponsConsole, 2, 1);
            AddObject(mainDeck, InteriorObjectType.WeaponsConsole, "Weapons Console", 5, 3, InteriorTileType.WeaponsConsole, 2, 1);

            AddObject(medical, InteriorObjectType.MedicalConsole, "Medical Console", 2, 1, InteriorTileType.MedicalConsole, 1, 2, false, IntAct_LR);
            AddObject(medical, InteriorObjectType.MedicalConsole, "Medical Console", 5, 1, InteriorTileType.MedicalConsole, 1, 2, false, IntAct_LR);
            AddObject(medical, InteriorObjectType.MedicalConsole, "Medical Console", 2, 4, InteriorTileType.MedicalConsole, 1, 2, false, IntAct_LR);
            AddObject(medical, InteriorObjectType.MedicalConsole, "Medical Console", 5, 4, InteriorTileType.MedicalConsole, 1, 2, false, IntAct_LR);

            //AddObject(medical, InteriorObjectType.Bed, "Medical Bed", 2, 4, InteriorTileType.Bed, 1, 2, false, IntAct_LR);
            //AddObject(medical, InteriorObjectType.Bed, "Medical Bed", 5, 4, InteriorTileType.Bed, 1, 2, false, IntAct_LR);

            AddObject(crew, InteriorObjectType.Bed, "Bed 1", 1, 1, InteriorTileType.Bed, 1, 2, false, IntAct_LR);
            AddObject(crew, InteriorObjectType.Bed, "Bed 2", 7, 1, InteriorTileType.Bed, 1, 2, false, IntAct_LR);
            AddObject(crew, InteriorObjectType.Bed, "Bed 3", 1, 5, InteriorTileType.Bed, 1, 2, false, IntAct_LR);
            AddObject(crew, InteriorObjectType.Bed, "Bed 4", 7, 5, InteriorTileType.Bed, 1, 2, false, IntAct_LR);
            AddObject(crew, InteriorObjectType.Table, "Small Table", 4, 3, InteriorTileType.Table);

            AddObject(cafeteria, InteriorObjectType.Stove, "Stove", 1, 1, InteriorTileType.KitchenCounter);
            AddObject(cafeteria, InteriorObjectType.Table, "Dining Table", 3, 1, InteriorTileType.Table, 3, 1, true, Interaction_Bottom);
            AddObject(cafeteria, InteriorObjectType.Table, "Dining Table", 3, 3, InteriorTileType.Table, 3, 1, true, Interaction_Bottom);
            AddObject(cafeteria, InteriorObjectType.Table, "Dining Table", 3, 5, InteriorTileType.Table, 3, 1, true, Interaction_Bottom);
            //AddObject(cafeteria, InteriorObjectType.Sofa, "Sofa", 1, 6, InteriorTileType.Sofa, 5, 1);

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
         InteriorTileType tileType = InteriorTileType.Floor,
         int width = 1,
         int height = 1,
         bool allowMultipleCrew = false,
         List<InteractionDirection> interactionDirections = null)
        {
            if (interactionDirections == null)
            {
                interactionDirections = new List<InteractionDirection>
        {
            InteractionDirection.Bottom
        };
            }

            InteriorObject obj = new InteriorObject
            {
                ObjectType = objectType,
                TileType = tileType,
                Name = name,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                ParentRoom = room,
                AllowedInteractionDirections = interactionDirections,
                AllowMultipleCrew = allowMultipleCrew
            };

            room.Objects.Add(obj);

            for (int tileY = y; tileY < y + height; tileY++)
            {
                for (int tileX = x; tileX < x + width; tileX++)
                {
                    InteriorTile tile = room.GetTileFast(tileX, tileY);

                    if (tile == null)
                        continue;

                    tile.ObjectId = obj.Id;
                    tile.TileType = tileType;
                    tile.IsWalkable = false;
                }
            }
        }
    }
}
