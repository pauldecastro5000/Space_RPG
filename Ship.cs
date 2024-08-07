﻿using Space_RPG.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
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
            LiftingOff,
            Landed,
            Warping,
            Hovering
        }
        public enum FacilityType
        {
            MainDeck,
            Cafeteria,
            Armoury,
            CrewQuarters,
            Garden,
            Gym,
            MedicalBay,
            Cargo,
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
        private ObservableCollection<Facility> _facilities;
        public ObservableCollection<Facility> Facilities
        {
            get { return _facilities; }
            set { _facilities = value; OnPropertyChanged(); }
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
        public int OrderId { get; set; } = 0;
        #endregion Public Properties

        #region Private Properties
        private static object _lockCommands = new object();
        private List<Order> _CaptainsOrders = new List<Order>();
        private double _engineFuelDepletionRate = 0.23;
        private double _LiftoffFuelDepletionRate = 1;
        #endregion Private Properties

        #region Constructor
        public Ship()
        {
            MainWindow.UniverseTime.UniverseTickPerMin += UniverseTime_UniverseTickPerMin;
        }
        #endregion Constructor

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
                        RemoveAssignment(Captain);
                        crew.Job = Crew.CrewJob.Captain;
                        Captain = crew;
                        break;

                    case "PILOT":
                        RemoveAssignment(crew);
                        RemoveAssignment(Pilot);
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
                                RemoveAssignment(weapon.Gunner);
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
            //Crews.Add(newCrew);
            PlaceCrew(FacilityType.MainDeck, newCrew);
        }

        public void AddCrewToFacility(FacilityType facilityType)
        {
            var newCrew = new Crew()
            {
                Name = "Temp",
                Job = Crew.CrewJob.None,
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

            var facility = Facilities.FirstOrDefault(x => x.type == facilityType);
            if (facility != null)
            {
                facility.Crews.Add(newCrew);
            } else
            {
                MainWindow.mainVm.Log.Add($"facility [{facilityType.ToString()}] cannot be found");
            }

        }

        public bool HireApplicant(Crew crew, out string err)
        {
            err = "";
            if (crew.Price > MainWindow.mainVm.MyShip.Captain.Cash)
            {
                err = $"You don't have enough cash to hire {crew.Name}";
                return false;
            }
            MainWindow.mainVm.MyShip.Captain.Cash -= crew.Price;
            Crews.Add(crew);
            return true;
        }
        public void RemoveAssignment(Crew crew)
        {
            // CAPTAIN
            if (Captain?.Name == crew?.Name)
            {
                Captain = null;
                return;
            }
            // PILOT
            if (Pilot?.Name == crew?.Name)
            {
                Pilot = null;
                return;
            }
            // WEAPONS
            foreach (var weapon in Weapons)
            {
                if (weapon.Gunner?.Name == crew?.Name)
                {
                    weapon.Gunner = null;
                    return;
                }
            }
        }
        public async void StartEngine()
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(2000);
                if (engine.CurrentFuel > 0)
                {
                    SetEngineState(Engine.state.On);
                }
            });
        }
        public async void ShutOffEngine()
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(2000);
                SetEngineState(Engine.state.Off);
            });
        }
        public async void Liftoff()
        {
            State = state.LiftingOff;
            await Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    System.Threading.Thread.Sleep(200);
                    if (engine.CurrentFuel <= 0)
                    {
                        ShutOffEngine();
                    }
                }
                State = state.Hovering;
            });
        }
        public void TaskDone(int taskId)
        {
            lock (_lockCommands)
            {
                _CaptainsOrders.RemoveAll(x => x.Id == taskId);
            }
        }
        public void AddCrewTask(Crew.CrewJob job, string command)
        {
            lock (_lockCommands)
            {
                _CaptainsOrders.Add(new Order()
                {
                    Id = OrderId,
                    Job = job,
                    Command = command,
                    Timestamp = DateTime.Now,
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
            var task = _CaptainsOrders.FirstOrDefault(x => (x.Job == job || x.Job == Crew.CrewJob.All));
            if (task != null)
            { 
                taskId = task.Id;
                return task.Command;
            }
            else
                return "";
        }
        public bool AllCrewSeated()
        {
            foreach (var crew in Crews)
            {
                if (crew.Job == Crew.CrewJob.Captain || crew.Job == Crew.CrewJob.Pilot)
                    continue;
                if (crew.CurrentTask != Crew.Task.TakeSeat && crew.Job != Crew.CrewJob.Pilot)
                    return false;
            }
            return true;
        }
        #endregion Public Methods

        #region Private Methods
        private void SetEngineState(Engine.state State)
        {
            engine.State = State;
            MainWindow.mainVm.EngineState = Enum.GetName(typeof(Ship.Engine.state),
                                     MainWindow.mainVm.MyShip.engine.State);
        }
        private void UniverseTime_UniverseTickPerMin(object sender, EventArgs e)
        {
            if (engine.State == Engine.state.On)
            {
                if (State == state.LiftingOff)
                {
                    engine.ConsumeFuel(_LiftoffFuelDepletionRate);
                }
                else
                    engine.ConsumeFuel(_engineFuelDepletionRate);
            }
        }
        private void PlaceCrew(FacilityType facilityType, Crew crew)
        {
            var facility = Facilities.FirstOrDefault(x => x.type == facilityType);
            if (facility != null)
            {
                facility.Crews.Add(crew);
            }
        }
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
            private int _fuelCapacity = 10000;
            public int FuelCapacity
            {
                get { return _fuelCapacity; }
                set { _fuelCapacity = value; OnPropertyChanged(); }
            }
            private double _currFuel = 10000;
            public double CurrentFuel
            {
                get { return _currFuel; }
                set { _currFuel = value; OnPropertyChanged(); }
            }
            private double _fuelPercent = 100.0;
            public double FuelPercent
            {
                get { return _fuelPercent; }
                set { _fuelPercent = value; OnPropertyChanged(); }
            }

            private state _state = state.Unknown;
            public state State
            {
                get { return _state; }
                set { _state = value; OnPropertyChanged(); }
            }

            public void ConsumeFuel(double amount)
            {
                CurrentFuel = CurrentFuel > amount ? CurrentFuel - amount : 0;
                if (CurrentFuel == 0)
                    MainWindow.mainVm.MyShip.ShutOffEngine();
                FuelPercent = (CurrentFuel / FuelCapacity) * 100;
            }
            public void AddFuel(double amount)
            {
                CurrentFuel = CurrentFuel + amount < FuelCapacity ? CurrentFuel + amount : FuelCapacity;
                FuelPercent = (CurrentFuel / FuelCapacity) * 100;
            }
        }
        public class Order
        {
            public int Id { get; set; }
            public Crew.CrewJob Job { get; set; }
            public string Command { get; set; }
            public DateTime Timestamp { get; set; }
        }
        public class Facility
        {
            public string Name { get; set; }
            public FacilityType type { get; set; }
            public ObservableCollection<Crew> Crews { get; set; } = new ObservableCollection<Crew>();
        }
        #endregion Public Class
    }
}
