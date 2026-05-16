using Space_RPG.Models;
using Space_RPG.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Space_RPG.Services
{
    public class CrewManager : ViewModelBase
    {
        #region Public Members
        public enum CrewNameEnum
        {
            Liam,
            Noah,
            Oliver,
            James,
            Elijah,
            William,
            Henry,
            Lucas,
            Benjamin,
            Theodore,
            Mateo,
            Levi,
            Sebastian,
            Daniel,
            Jack,
            Michael,
            Alexander,
            Owen,
            Asher,
            Samuel,
            Olivia,
            Emma,
            Charlotte,
            Amelia,
            Sophia,
            Mia,
            Isabella,
            Ava,
            Evelyn,
            Luna,
            Harper,
            Sofia,
            Camila,
            Eleanor,
            Elizabeth,
            Violet,
            Scarlett,
            Emily,
            Hazel,
            Lily,
            Gianna,
            Aurora,
            Penelope,
            Aria,
            Chloe,
            Ellie,
            Mila,
            Layla,
            Abigail,
            Ella,
            Eliana,
            Nova,
            Madison,
            Zoe,
            Ivy,
            Grace,
            Lucy,
            Emilia,
            Riley,
            Naomi,
            Victoria,
            Stella,
            Elena,
            Hannah,
            Valentina,
            Maya,
            Zoey,
            Delilah,
            Leah,
            Lainey,
            Lillian,
            Madelyn,
            Sophie,
            Natalie,
            Josephine,
            Alice,
            Ruby,
            Claire
        }
        #endregion Public Members

        #region Public Properties
        private ObservableCollection<Crew> _crews = new ObservableCollection<Crew>();
        public ObservableCollection<Crew> Crews
        {
            get { return _crews; }
            set { _crews = value; OnPropertyChanged(); }
        }

        public double hungerDepletion { get; } = 0.125;
        #endregion Public Properties

        #region Private Variables
        private static readonly object syncLock = new object();
        private Crew _applicant;
        private readonly Utilities _utilities;
        #endregion Private Variables

        #region Constructor
        public CrewManager(Utilities utilities)
        {
            MainWindow.UniverseTime.UniverseTickPerMin += UniverseTime_UniverseTickPerMin;
            _utilities = utilities;
        }
        #endregion Constructor

        #region Public Methods
        public Crew CreatePlayer(string name, Ship ship)
        {
            var aiming = _utilities.RandomNumber(0, 100);
            var pilot = _utilities.RandomNumber(0, 100);
            var engineRepair = _utilities.RandomNumber(0, 100);
            var weaponsRepair = _utilities.RandomNumber(0, 100);

            var job = Job.Captain;

            var newCrew = new Crew()
            {
                Name = name,
                Job = job,
                Hunger = 90,
                Cash = 10000000,
                
                //Skills = new CrewMgr.skills()
            };
            PlaceCrewInsideShip(ship, newCrew);
            return newCrew;
        }
        public ObservableCollection<Guid> GetCrewsId(ObservableCollection<Crew> crews)
        {
            if (crews.Count == 0 || crews == null)
                return null;

            var crewsId = new ObservableCollection<Guid>();
            foreach (Crew crew in crews)
            {
                crewsId.Add(crew.Id);
            }
            return crewsId;
        }
        public FacilityType SetActionLocation(Crew crew, CrewAction action, CrewAction prevAction)
        {
            return FacilityType.MainDeck;
            //if (prevAction == action)
            //    return;
            
            //switch (action)
            //{
            //    case CrewAction.Eat:

            //}

        }
        public void UpdateTargetXY(Crew crew, CrewAction action, CrewAction prevAction, Ship ship)
        {
            if (action == prevAction)
            {
                return;
            }

            switch (action)
            {
                case CrewAction.Work:
                    SetCrewToWork(ship, crew);
                    break;

                case CrewAction.Eat:
                    SetCrewToEat(ship, crew);
                    break;

                case CrewAction.Sleep:
                    SetCrewToSleep(ship, crew);
                    break;
            }
        }
      
        public ObservableCollection<Crew> ApplicantsToCrews(
     ObservableCollection<Applicant> applicants,
     Ship ship)
        {
            if (applicants == null || applicants.Count == 0)
                return null;

            var newCrews = new ObservableCollection<Crew>();

            foreach (var applicant in applicants)
            {
                var newCrew = new Crew()
                {
                    Id = Guid.NewGuid(),
                    Name = applicant.Name,
                    Age = applicant.Age,
                    Price = applicant.Price,
                    Hunger = 100,
                    Fatigue = 0,
                    IsAlive = true
                };

                PlaceCrewInsideShip(ship, newCrew);

                newCrews.Add(newCrew);
                ship.Crews.Add(newCrew.Id);
            }

            return newCrews;
        }
        public Crew CreateRandomCrew()
        {
            var name = String.Empty;
            do
            {
                name = ((CrewNameEnum)_utilities.RandomNumber(0, 77)).ToString();
            } while (CrewNameExist(name));
            var aiming = _utilities.RandomNumber(0, 100);
            var pilot = _utilities.RandomNumber(0, 100);
            var engineRepair = _utilities.RandomNumber(0, 100);
            var weaponsRepair = _utilities.RandomNumber(0, 100);
            var age = _utilities.RandomNumber(18, 30);
            //var job = (CrewMgr.CrewJob)RandomNumber(1, 3);

            var newCrew = new Crew()
            {
                Name = name,
                Hunger = 100,
                Cash = 500,
                Age = age,
            };

            return newCrew;
        }
        public void FindApplicant()
        {
            var name = String.Empty;
            do
            {
                name = ((CrewNameEnum)_utilities.RandomNumber(0, 77)).ToString();
            } while (CrewNameExist(name));
            var aiming = _utilities.RandomNumber(0, 80);
            var piloting = _utilities.RandomNumber(0, 80);
            var engineRepair = _utilities.RandomNumber(0, 80);
            var weaponsRepair = _utilities.RandomNumber(0, 80);
            var job = Job.None;

            var price = CalculatePrice(aiming, piloting, engineRepair, weaponsRepair);

            var newCrew = new Crew()
            {
                Name = name,
                Job = job,
                Hunger = 100,
                //Cash = 500,
                Price = price,
                //Skills = new Crew.skills() {
                //    Piloting = piloting,
                //    Aiming = aiming, 
                //    EngineRepair = engineRepair, 
                //    WeaponsRepair = weaponsRepair 
                //} 
            };

            _applicant = newCrew;
        }
        public void DisplayApplicant()
        {
            //Log("Name: " + _applicant.Name);
            //Log("Piloting: " + _applicant.Skills.Piloting);
            //Log("Aiming: " + _applicant.Skills.Aiming);
            //Log("EngineRepair: " + _applicant.Skills.EngineRepair);
            //Log("WeaponsRepair: " + _applicant.Skills.WeaponsRepair);
            //Log("Price: $" + _applicant.Price.ToString("n0"));
            //Log("");
        }
        public void HireApplicant(string name)
        {
            if (name.ToUpper() == _applicant.Name.ToUpper())
            {
                Crews.Add(_applicant);
            }
            else
            {
                Log("No applicant with that name...");
            }
        }
        public bool GetApplicant(string name, out Crew crew, out string err)
        {
            err = "";
            crew = new Crew();
            if (_applicant == null)
            {
                err = "There is no applicant";
                return false;
            }

            if (name.ToUpper() == _applicant.Name.ToUpper())
            {
                crew = _applicant;
                return true;
            }
            else
            {
                err = "No applicant with that name...";
                return false;
            }
        }
        public void RemoveApplicant(Crew crew)
        {
            if (crew.Name == _applicant.Name)
            {
                _applicant = null;
            }
        }
        public void AddCaptain()
        {
            var newCrew = new Crew()
            {
                Name = "Paul",
                Job = Job.Captain,
                Hunger = 100,
                //Cash = 100000,
                //Skills = new Crew.skills()
                //{
                //    Piloting = 90,
                //    Aiming = 90,
                //    EngineRepair = 90,
                //    WeaponsRepair = 90,
                //},
            };
            Crews.Add(newCrew);
        }

        #endregion Public Methods

        #region Private Methods
        private void PlaceCrewInsideShip(Ship ship, Crew crew)
        {
            ShipMapBuilder.RebuildGlobalTileMap(ship);

            ShipMapTile spawnTile = ship.Interior.GlobalTileMap.Values
                .FirstOrDefault(t =>
                    t.FacilityType == FacilityType.MainDeck &&
                    t.IsWalkable);

            if (spawnTile == null)
                return;

            crew.X = spawnTile.X;
            crew.Y = spawnTile.Y;

            crew.TargetX = spawnTile.X;
            crew.TargetY = spawnTile.Y;

            crew.IsInShip = true;
            crew.IsInPlanet = false;
            crew.IsInRover = false;

            crew.ShipId = ship.Id;
        }
        private void UniverseTime_UniverseTickPerMin(object sender, EventArgs e)
        {
            //foreach (var crew in MainWindow.mainVm.MyShip.Crews)
            //{
            //    //if (crew.Alive)
            //    //{
            //    //    crew.UpdateHunger();
            //    //    crew.TaskLoop();
            //    //}
            //}
        }
        private bool CrewNameExist(string name)
        {
            if (Crews == null)
                return false;

            var crew = Crews.FirstOrDefault(x => x.Name == name);
            if (crew == null)
                return false;
            else
                return true;
        }

        private Point GetTableInCafeteria(Ship ship, Crew crew)
        {
            InteriorRoom cafeteria = ship.Interior.Rooms.FirstOrDefault(r => r.RoomType == FacilityType.Cafeteria);

            InteriorObject table = cafeteria.Objects.FirstOrDefault(o => o.ObjectType == InteriorObjectType.Table);

            List<InteriorTile> tableTiles = cafeteria.Tiles.Where(t => t.ObjectId == table.Id).ToList();

            // If the table is a 1-tile object:
            InteriorTile tile = tableTiles.First();

            int x = tile.X;
            int y = tile.Y;

            //If it is a multi - tile object:
            //foreach (InteriorTile tile in tableTiles)
            //{
            //    Console.WriteLine($"Table tile at {tile.X}, {tile.Y}");
            //}

            return new Point(x, y);

        }
        private void Log(string message)
        {
            MainWindow.mainVm.Log.Add(message);
        }
        private void SetCrewToWork(Ship ship, Crew crew)
        {
            Mapper.ReleaseWorkstation(ship, crew);

            Guid? selectedObjectId;

            Point workPoint = Mapper.GetWorkstationTargetPosition(
                ship,
                crew.Job,
                crew.Id,
                out selectedObjectId);

            if (workPoint.X == -1 || workPoint.Y == -1)
                return;

            crew.TargetX = (int)workPoint.X;
            crew.TargetY = (int)workPoint.Y;
            crew.ReservedObjectId = selectedObjectId;
        }
        private void SetCrewToEat(Ship ship, Crew crew)
        {
            Mapper.ReleaseWorkstation(ship, crew);

            Guid? selectedObjectId;

            Point target = Mapper.GetObjectInteractionTargetPosition(
                ship,
                FacilityType.Cafeteria,
                InteriorObjectType.Table,
                crew.Id,
                out selectedObjectId);

            if (target.X == -1 || target.Y == -1)
                return;

            crew.TargetX = (int)target.X;
            crew.TargetY = (int)target.Y;
            crew.ReservedObjectId = selectedObjectId;
        }
        private void SetCrewToSleep(Ship ship, Crew crew)
        {
            Mapper.ReleaseWorkstation(ship, crew);

            Guid? selectedObjectId;

            Point target = Mapper.GetObjectInteractionTargetPosition(
                ship,
                FacilityType.FemaleCrewQuarters,
                InteriorObjectType.Bed,
                crew.Id,
                out selectedObjectId);

            if (target.X == -1 || target.Y == -1)
                return;

            crew.TargetX = (int)target.X;
            crew.TargetY = (int)target.Y;
            crew.ReservedObjectId = selectedObjectId;
        }
        private void SetCrewToWander(Ship ship, Crew crew)
        {
            Mapper.ReleaseWorkstation(ship, crew);

            Point target = Mapper.GetRandomWalkableTile(ship);

            if (target.X == -1 || target.Y == -1)
                return;

            crew.TargetX = (int)target.X;
            crew.TargetY = (int)target.Y;
        }
        private int CalculatePrice(int Stat1, int Stat2, int Stat3, int Stat4)
        {
            var basePrice = 100;
            var price = (Stat1 * basePrice) +
                        (Stat2 * basePrice) +
                        (Stat3 * basePrice) +
                        (Stat4 * basePrice);
            return price;
        }
        #endregion Private Methods
    }
}
