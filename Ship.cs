using Space_RPG.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace Space_RPG
{
    public class Ship : ViewModelBase
    {
        #region Public Members
        public enum state
        {
            None,
            Docked,
            Landed,
            Warping,
            Hovering
        }
        #endregion Public Members

        #region Public Properties
        private Engine _engine;
        public Engine engine
        {
            get { return _engine; }
            set { _engine = value; OnPropertyChanged(); }
        }
        private Crew _captain;
        public Crew Captain
        {
            get { return _captain; }
            set { _captain = value; OnPropertyChanged(); }
        }

        private Crew _pilot;
        public Crew Pilot
        {
            get { return _pilot; }
            set { _pilot = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Weapon> _weapons;
        public ObservableCollection<Weapon> Weapons
        {
            get { return _weapons; }
            set { _weapons = value; OnPropertyChanged(); }
        }
        private state _state;
        public state State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged(); }
        }
        private Point _location;
        public Point Location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged(); }
        }
        private int _food;
        public int Food
        {
            get { return _food; }
            set { _food = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Crew> _crews = new ObservableCollection<Crew>();
        public ObservableCollection<Crew> Crews
        {
            get { return _crews; }
            set { _crews = value; OnPropertyChanged(); }
        }
        #endregion Public Properties

        #region Private Properties
        private static object _lockCommands = new object();
        public int OrderId { get; set; } = 0;
        private List<Order> _CaptainsOrders = new List<Order>();
        #endregion Private Properties

        #region Public Methods
        public bool Assign(string name, string crewJob, out string err)
        {
            if (Crews.Count == 0)
            {
                err = "You have no crew";
                return false;
            }
            err = "";
            var crew = Crews.FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());

            if (crew != null)
            {
                if (!crew.Alive)
                {
                    err = $"{crew.Name} is no longer alive";
                    return false;
                }
                switch (crewJob.ToUpper())
                {
                    case "CAPTAIN":
                        RemoveAssignment(crew);
                        crew.Job = Crew.CrewJob.Captain;
                        Captain = crew;
                        break;

                    case "PILOT":
                        RemoveAssignment(crew);
                        crew.Job = Crew.CrewJob.Pilot;
                        Pilot = crew;
                        break;

                    default:
                        // check weapons names
                        foreach (var weapon in Weapons)
                        {
                            if (crewJob.ToUpper() == weapon.Name.ToUpper())
                            {
                                RemoveAssignment(crew);
                                weapon.Gunner = crew;
                                return true;
                            }
                        }
                        err = $"Job {crewJob} is not a known designation";
                        return false;
                }
                return true;
            }
            else
            {
                err = $"{name} is not one of the crew member";
                return false;
            }
        }
        public void AddCaptain()
        {
            var newCrew = new Crew()
            {
                Name = "Paul",
                Job = Crew.CrewJob.Captain,
                Hunger = 100,
                Cash = 100000,
                Skills = new Crew.skills()
                {
                    Piloting = 90,
                    Aiming = 90,
                    EngineRepair = 90,
                    WeaponsRepair = 90,
                },
            };
            Captain = newCrew;
            Crews.Add(newCrew);
        }
        public void HireApplicant(Crew crew)
        {
            Crews.Add(crew);
        }
        public void RemoveAssignment(Crew crew)
        {
            // CAPTAIN
            if (Captain?.Name == crew.Name)
            {
                Captain = null;
                return;
            }
            // PILOT
            if (Pilot?.Name == crew.Name)
            {
                Pilot = null;
                return;
            }
            // WEAPONS
            foreach (var weapon in Weapons)
            {
                if (weapon.Gunner?.Name == crew.Name)
                {
                    weapon.Gunner = null;
                    return;
                }
            }
        }
        public void StartEngine()
        {
            engine.State = Engine.state.On;
            MainWindow.mainVm.EngineState = Enum.GetName(typeof(Ship.Engine.state),
                                                         MainWindow.mainVm.MyShip.engine.State);
        }
        public void TaskDone(int taskId)
        {
            lock (_lockCommands)
            {
                _CaptainsOrders.RemoveAll(x => x.Id == taskId);
            }
        }
        public void ShutOffEngine()
        {
            engine.State = Engine.state.Off;
            MainWindow.mainVm.EngineState = Enum.GetName(typeof(Ship.Engine.state),
                                                         MainWindow.mainVm.MyShip.engine.State);
        }
        public void AddCrewTask(Crew.CrewJob job, string command)
        {
            lock (_lockCommands)
            {
                _CaptainsOrders.Add(new Order()
                {
                    Id = OrderId,
                    Job = job,
                    Command = command

                });
                OrderId++;
            }
        }
        public void RemoveCrewTask(int Id)
        {
            lock (_lockCommands)
            {
                _CaptainsOrders.RemoveAll(x => x.Id == Id);
            }
        }
        public string GetCrewTask(Crew.CrewJob job, out int taskId)
        {
            taskId = -1;
            var task = _CaptainsOrders.FirstOrDefault(x => x.Job == job);
            if (task != null)
            { 
                taskId = task.Id;
                return task.Command;
            }
            else
                return "";
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods

        #region Public Class
        public class Weapon : ViewModelBase
        {
            private string _name = "Turret";
            public string Name
            {
                get { return _name; }
                set { _name = value; OnPropertyChanged(); }
            }
            private Crew _gunner;
            public Crew Gunner
            {
                get { return _gunner; }
                set { _gunner = value; OnPropertyChanged(); }
            }

            private int _maxHealth = 1000;
            public int MaxHealth
            {
                get { return _maxHealth; }
                set { _maxHealth = value; OnPropertyChanged(); }
            }
            private int _currentHealth = 1000;
            public int CurrentHealth
            {
                get { return _currentHealth; }
                set { _currentHealth = value; OnPropertyChanged(); }
            }
            public double HealthPercent => (CurrentHealth / MaxHealth) * 100;

            private int _damage = 10;
            public int Damage
            {
                get { return _damage; }
                set { _damage = value; OnPropertyChanged(); }
            }
            private int _ammo = 1000;
            public int Ammo
            {
                get { return _ammo; }
                set { _ammo = value; OnPropertyChanged(); }
            }
        }
        public class Engine : ViewModelBase
        {
            public enum state
            {
                Unknown,
                On,
                Off,
                Jammed
            }

            private int _health = 0;
            public int Health
            {
                get { return _health; }
                set { _health = value; OnPropertyChanged(); }
            }
            private int _maxFuel = 1000;
            public int MaxFuel
            {
                get { return _maxFuel; }
                set { _maxFuel = value; OnPropertyChanged(); }
            }
            private int _currFuel = 1000;
            public int CurrentFuel
            {
                get { return _currFuel; }
                set { _currFuel = value; OnPropertyChanged(); }
            }
            public double FuelPercent => (_currFuel / _maxFuel) * 100;

            private state _state = state.Unknown;
            public state State
            {
                get { return _state; }
                set { _state = value; OnPropertyChanged(); }
            }
        }
        public class Order
        {
            public int Id { get; set; }
            public Crew.CrewJob Job { get; set; }
            public string Command { get; set; }
        }
        #endregion Public Class
    }
}
