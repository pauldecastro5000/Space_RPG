using Space_RPG.Helpers;
using Space_RPG.Models;
using Space_RPG.Services;
using Space_RPG.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Space_RPG.ViewModel
{
    public class MainVM : ViewModelBase
    {
        #region Members
        private const int GameTickIntervalMs = 650;

        private const int ActionDecisionIntervalTicks = 10;
        private const int MinimumActionDurationTicks = 30;

        private readonly Timer _gameLoopTimer;
        private readonly Dispatcher _uiDispatcher;
        private readonly Stopwatch _gameLoopStopwatch;

        private readonly PlanetManager planetMgr = new PlanetManager();
        private readonly CrewManager crewMgr;
        private readonly Utilities util;
        private readonly ShipManager shipMgr = new ShipManager();


        private int _gameLoopCallbackQueued;
        private long _lastGameLoopMs;
        private double _gameTickAccumulatorMs;
        private CrewAction _prevAction;

        private bool _isGameTickRunning;
        BackgroundWorker _bgwUpdate;

        public ObservableCollection<string> LogEntries { get; private set; }
        #endregion Members

        #region Properties

        private GameState _state;
        public GameState State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged(); }
        }

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set { _statusText = value; OnPropertyChanged(); }
        }

        private Crew _selectedCrew;
        public Crew SelectedCrew
        {
            get { return _selectedCrew; }
            set
            {
                _selectedCrew = value;
                OnPropertyChanged(nameof(SelectedCrew));
            }
        }

        public Array JobOptions
        {
            get { return Enum.GetValues(typeof(Job)); }
        }
        public ObservableCollection<Crew> Crews { get { return State.Crews; } }
        public Ship MyShip { get { return State.MyShip; } }
        //public ObservableCollection<Crew> Crews { get { return _state.Crews; } }
        public ObservableCollection<Planet> Planets { get { return State.Planets; } }

        //private Crew _player;
        //public Crew Player
        //{
        //    get { return _player; }
        //    set { _player = value; OnPropertyChanged(); }
        //}

        //private CrewManager _crewManager;
        //public CrewManager CrewManager
        //{
        //    get { return _crewManager; }
        //    set { _crewManager = value; OnPropertyChanged(); }
        //}

        private string _command;
        public string Command
        {
            get { return _command; }
            set { _command = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _log = new ObservableCollection<string>();
        public ObservableCollection<string> Log
        {
            get { return _log; }
            set { _log = value; OnPropertyChanged(); }
        }

        private Planet _currentPlanet;
        public Planet CurrentPlanet
        {
            get { return _currentPlanet; }
            set { _currentPlanet = value; OnPropertyChanged(); }
        }

        private string _planetType;
        public string PlanetType
        {
            get { return _planetType; }
            set { _planetType = value; OnPropertyChanged(); }
        }

        private string _engineState;
        public string EngineState
        {
            get { return _engineState; }
            set { _engineState = value; OnPropertyChanged(); }
        }

        private string _cockpitImage = "../Resources/DOCKED.jpg";
        public string CockpitImage
        {
            get { return _cockpitImage; }
            set { _cockpitImage = value; OnPropertyChanged(); }
        }
        private double _mainImgOpacity = 1;
        public double MainImgOpacity
        {
            get { return _mainImgOpacity; }
            set { _mainImgOpacity = value; OnPropertyChanged(); }
        }
        private DateTime _dateTime;
        public DateTime dateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; OnPropertyChanged(); }
        }
        #endregion Properties

        #region Applicants
        public ObservableCollection<Crew> AcceptedApplicants { get; set; }

        public RelayCommand OpenApplicantsCommand { get; }

        #endregion Applicants

        #region Constructor

        public MainVM(CrewManager CrewMgr, Utilities utilities)
        {
            crewMgr = CrewMgr;
            util = utilities;
            //CrewManager = MainWindow.CrewMgr;
            LogEntries = new ObservableCollection<string>();
            #region Applicants
            AcceptedApplicants = new ObservableCollection<Crew>();
            OpenApplicantsCommand = new RelayCommand(OpenApplicants, CanOpenApplicants);
            #endregion Applicants


            _uiDispatcher = Dispatcher.CurrentDispatcher;
            _gameLoopStopwatch = Stopwatch.StartNew();
            _lastGameLoopMs = 0;

            _gameLoopTimer = new Timer(OnGameLoopTimer, null, Timeout.Infinite, Timeout.Infinite);

            ResetGame();
            _gameLoopTimer.Change(0, 50);
        }


        #endregion Constructor

        private bool CanOpenApplicants()
        {
            return State.ShipIsInPlanet;
        }
        private void ResetGame()
        {
            State = CreateNewGame();

            CreateFirstPlanet();
            CreateMyShip();
            CreatePlayer();
            PlaceShipInFirstPlanet();
            PlacePlayerInFirstShip();
            LogEntries.Clear();

            SyncAll();
        }

        private void CreateFirstPlanet()
        {
            var newPlanet = planetMgr.CreateColonizedPlanet(State.Planets);
            State.Planets.Add(newPlanet);
        }

        private void CreateMyShip()
        {
            //_state.MyShip = shipMgr.CreateMyShip();
            State.MyShip = ShipGenerator.GenerateDefaultShip();
        }

        private void CreatePlayer()
        {
            var newPlayer = crewMgr.CreatePlayer("Paul", State.MyShip);
            newPlayer.IsInShip = true;
            newPlayer.ShipId = MyShip.Id;
            State.Crews.Add(newPlayer);
        }

        private void PlaceShipInFirstPlanet()
        {
            State.MyShip.Location = Planets[0].Location;
            PlanetType = State.Planets[0].Type.ToString();
            State.ShipIsInPlanet = true;
        }

        private void PlacePlayerInFirstShip()
        {
            //var facility = _state.MyShip.Facilities.FirstOrDefault(x => x.Type == FacilityType.MainDeck);
            //if (facility != null)
            //{
            //    facility.CrewIds.Add(Crews[0].Id);
            //}
        }

        private GameState CreateNewGame()
        {
            GameState state = new GameState();
            state.TickCount = 0;

            state.TimeOfDay = 360;
            state.MinutesPerTick = 1;



            //state.WorldMap = CreateMap(MapSize, MapSize);

            //int center = MapSize / 2;

            //state.Villagers.Add(new Villager
            //{
            //    Name = "Me",
            //    IsPlayer = true,
            //    Health = 100,
            //    Hunger = 0,
            //    Fatigue = 0,
            //    CurrentRole = VillagerTaskType.Idle,
            //    PlayerAction = JobType.None,
            //    IsAlive = true,
            //    X = center,
            //    Y = center,
            //    HomeX = center,
            //    HomeY = center
            //});

            //Villager worker = CreateVillager(center, center);
            //worker.CurrentRole = VillagerTaskType.Gathering;
            //state.Villagers.Add(worker);

            return state;
        }

        #region Applicants
        private void OpenApplicants()
        {
            var vm = new ApplicantSelectionViewModel();

            var window = new ApplicantSelectionWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            bool? result = window.ShowDialog();

            if (result == true)
            {
                AcceptedApplicants.Clear();

                var newCrews = crewMgr.ApplicantsToCrews(vm.SelectedApplicants, State.MyShip);

                foreach (var crew in newCrews)
                {
                    crew.IsInShip = true;
                    crew.ShipId = MyShip.Id;
                    //MyShip.Facilities[0].CrewIds.Add(crew.Id);
                }

                util.AddRange(State.Crews, newCrews);
                var crewsId = crewMgr.GetCrewsId(newCrews);

                //util.AddRange(MyShip.Facilities[0].CrewIds, crewsId);
            }
        }
        #endregion Applicants

        #region Private Methods
        private void OnGameLoopTimer(object state)
        {
            if (Interlocked.Exchange(ref _gameLoopCallbackQueued, 1) == 1)
                return;

            _uiDispatcher.BeginInvoke(new Action(ProcessGameLoop), DispatcherPriority.Normal);
        }

        private void ProcessGameLoop()
        {
            if (_isGameTickRunning)
            {
                Interlocked.Exchange(ref _gameLoopCallbackQueued, 0);
                return;
            }

            _isGameTickRunning = true;

            try
            {
                long nowMs = _gameLoopStopwatch.ElapsedMilliseconds;
                long elapsedMs = nowMs - _lastGameLoopMs;
                _lastGameLoopMs = nowMs;

                if (elapsedMs < 0)
                    elapsedMs = 0;

                _gameTickAccumulatorMs += elapsedMs;

                int processedTicks = 0;
                const int maxCatchUpTicks = 3;

                while (_gameTickAccumulatorMs >= GameTickIntervalMs && processedTicks < maxCatchUpTicks)
                {
                    Tick();
                    MarkDirty();
                    _gameTickAccumulatorMs -= GameTickIntervalMs;
                    processedTicks++;
                }

                if (_gameTickAccumulatorMs > GameTickIntervalMs * maxCatchUpTicks)
                    _gameTickAccumulatorMs = 0;
            }
            finally
            {
                _isGameTickRunning = false;
                Interlocked.Exchange(ref _gameLoopCallbackQueued, 0);
            }
        }

        private void Tick()
        {
            State.TickCount++;
            AdvanceClock();

            foreach (Crew crew in State.Crews.Where(v => v.IsAlive && !v.IsPlayer))
            {
                UpdateCrewStatus(crew);
                UpdateCrew(crew, State.MyShip);
            }
            SyncAll();
        }

        private void SendCrewToTargetLoc(Crew crew, CrewAction crewAction)
        {
            switch (crewAction)
            {
                case CrewAction.Eat:
                    break;
                case CrewAction.Work:
                    break;
                case CrewAction.Sleep:
                    break;
            }
        }

        private void UpdateCrewAction(Crew crew)
        {
            if (crew == null || !crew.IsAlive)
                return;

            //var action = AIActionSelector.GetAction(crew);
            //SendCrewToTargetLoc(crew, action);


            //if (crew.Activity == Activity.None)
            //{

            //}




            //EnsureVillagerBedAssignment(crew);
            //crew.IsReturningHome = true;

            //if (crew.X != crew.HomeX || crew.Y != crew.HomeY)
            //{
            //    crew.IsSleeping = false;
            //    crew.IsInsideBuilding = false;
            //    MoveTowards(crew, crew.HomeX, crew.HomeY);
            //    return;
            //}

            //if (crew.CarryingWood > 0 || crew.CarryingFood > 0 || crew.CarryingStone > 0)
            //    ReturnHome(crew);

            //if (crew.EquippedTool != ToolType.None)
            //{
            //    StoreToolInBuilding(GetHomeBuilding(crew), crew.EquippedTool);
            //    AddLog(crew.Name + " put away " + crew.EquippedTool + " before sleeping.");
            //    crew.EquippedTool = ToolType.None;
            //}

            //if (!crew.IsInsideBuilding)
            //{
            //    EnterVillagerHomeThroughDoor(crew);
            //    return;
            //}

            //if (crew.InteriorX != crew.AssignedBedX || crew.InteriorY != crew.AssignedBedY)
            //{
            //    crew.IsSleeping = false;
            //    MoveVillagerInsideTowards(crew, crew.AssignedBedX, crew.AssignedBedY, false);
            //    return;
            //}

            //crew.IsSleeping = true;
            //crew.IsReturningHome = true;
        }

        private void UpdateCrew(Crew crew, Ship ship)
        {
            if (!crew.IsAlive)
                return;

            UpdateCurrentActionProgress(crew);

            if (IsCrewBusy(crew))
            {
                UpdateCrewPath(ship, crew);
                MoveCrewAlongPath(crew);
                return;
            }

            if (State.TickCount < crew.NextActionDecisionTick)
                return;

            CrewAction nextAction = AIActionSelector.GetAction(crew);

            StartCrewAction(crew, ship, nextAction);
        }

        private void UpdateCrewPath(Ship ship, Crew crew)
        {
            if (crew.X == crew.TargetX &&
                crew.Y == crew.TargetY)
            {
                ClearCrewPath(crew);
                Mapper.MarkWorkstationOccupied(ship, crew);
                //crew.Activity = Activity.None;
                return;
            }

            if (crew.CurrentPath != null &&
                crew.CurrentPathIndex < crew.CurrentPath.Count)
            {
                return;
            }

            ShipMapBuilder.RebuildGlobalTileMap(ship);

            crew.CurrentPath = PathfindingService.FindPath(
                ship,
                crew.X,
                crew.Y,
                crew.TargetX,
                crew.TargetY);

            crew.CurrentPathIndex = 0;

            if (crew.CurrentPath == null || crew.CurrentPath.Count == 0)
            {
                ClearCrewPath(crew);
                return;
            }

            crew.Activity = Activity.Walking;
        }
        private void MoveCrewAlongPath(Crew crew)
        {
            if (crew.CurrentPath == null)
                return;

            if (crew.CurrentPathIndex >= crew.CurrentPath.Count)
                return;

            ShipMapTile nextTile = crew.CurrentPath[crew.CurrentPathIndex];

            if (nextTile.X == crew.X && nextTile.Y == crew.Y)
            {
                crew.CurrentPathIndex++;

                if (crew.CurrentPathIndex >= crew.CurrentPath.Count)
                    return;

                nextTile = crew.CurrentPath[crew.CurrentPathIndex];
            }

            crew.X = nextTile.X;
            crew.Y = nextTile.Y;
            crew.CurrentPathIndex++;

            if (crew.X == crew.TargetX &&
     crew.Y == crew.TargetY)
            {
                ClearCrewPath(crew);

                Mapper.MarkWorkstationOccupied(State.MyShip, crew);

                if (crew.Action == CrewAction.Eat)
                {
                    crew.IsEating = true;
                    crew.Activity = Activity.Eating;
                }
                else if (crew.Action == CrewAction.Sleep)
                {
                    crew.IsSleeping = true;
                    crew.Activity = Activity.None;
                }
                else if (crew.Action == CrewAction.Work)
                {
                    crew.Activity = Activity.Working;
                }
            }
        }
        private void ClearCrewPath(Crew crew)
        {
            crew.CurrentPath = null;
            crew.CurrentPathIndex = 0;
        }

        private void MoveCrewTowardsTarget(
    Ship ship,
    Crew crew)
        {
            // DO NOT USE THIS METHOD ANYMORE
            if (crew.X == crew.TargetX &&
                crew.Y == crew.TargetY)
            {
                return;
            }

            int nextX = crew.X;
            int nextY = crew.Y;

            // Horizontal movement first
            if (crew.X < crew.TargetX)
                nextX++;
            else if (crew.X > crew.TargetX)
                nextX--;

            // Vertical movement
            else if (crew.Y < crew.TargetY)
                nextY++;
            else if (crew.Y > crew.TargetY)
                nextY--;

            InteriorTile nextTile = GetTile(ship, nextX, nextY);

            if (nextTile == null)
                return;

            if (!nextTile.IsWalkable)
                return;

            crew.X = nextX;
            crew.Y = nextY;
        }

        private InteriorTile GetTile(
    Ship ship,
    int x,
    int y)
        {
            foreach (InteriorRoom room in ship.Interior.Rooms)
            {
                InteriorTile tile = room.Tiles
                    .FirstOrDefault(t =>
                        t.X == x &&
                        t.Y == y);

                if (tile != null)
                    return tile;
            }

            return null;
        }

        private void UpdateCrewStatus(Crew crew)
        {
            UpdateHunger(crew);
            UpdateFatigue(crew);
        }

        private void UpdateHunger(Crew crew)
        {
            if (crew.Activity == Activity.Eating) 
            {
                crew.Hunger = Math.Min(100, crew.Hunger + 1f);
            } else
            {
                // TODO: hunger depletion rate depends on activity type
                crew.Hunger = Math.Max(0, crew.Hunger - 0.1f);
            }
        }

        private void UpdateFatigue(Crew crew)
        {
            switch (crew.Activity)
            {
                case Activity.Eating:
                    crew.Fatigue = Math.Max(0, crew.Fatigue - 0.05f);
                    break;

                case Activity.Walking:
                    crew.Fatigue = Math.Max(0, crew.Fatigue - 0.1f);
                    break;

                case Activity.Sleeping:
                    crew.Fatigue = Math.Min(100, crew.Fatigue + 0.5f);
                    break;

                default:
                    crew.Fatigue = Math.Max(0, crew.Fatigue - 0.2f);
                    break;
            }
        }

        private void Killcrew(Crew crew, string reason)
        {
            if (crew == null || !crew.IsAlive)
                return;

            crew.IsAlive = false;
            crew.DeathReason = reason;

            AddLog(crew.Name + " has died from " + reason + ".");
        }

        private void AddLog(string message)
        {
            LogEntries.Insert(0, message);
            StatusText = message;
            while (LogEntries.Count > 350)
                LogEntries.RemoveAt(LogEntries.Count - 1);
        }

        private void AdvanceClock()
        {
            State.TimeOfDay += State.MinutesPerTick;
            if (State.TimeOfDay >= 1440)
                State.TimeOfDay -= 1440;
        }

        private bool IsCrewBusy(Crew crew)
        {
            if (crew.Activity == Activity.Walking)
                return true;

            if (crew.Activity == Activity.Eating)
                return true;

            if (crew.Activity == Activity.Working)
                return true;

            if (crew.IsSleeping)
                return true;

            return false;
        }

        private void StartCrewAction(
    Crew crew,
    Ship ship,
    CrewAction action)
        {
            crew.Action = action;
            crew.ActionStartedTick = State.TickCount;
            crew.NextActionDecisionTick = State.TickCount + ActionDecisionIntervalTicks;

            crew.IsEating = false;
            crew.IsSleeping = false;

            ClearCrewPath(crew);

            crewMgr.UpdateTargetXY(
                crew,
                action,
                CrewAction.None,
                ship);

            UpdateCrewPath(ship, crew);
        }

        private void UpdateCurrentActionProgress(Crew crew)
        {
            if (crew.Activity == Activity.Eating)
            {
                crew.Hunger += 0.1f;

                if (crew.Hunger >= 100)
                {
                    crew.Hunger = 100;
                    FinishCrewAction(crew);
                }

                return;
            }

            if (crew.IsSleeping)
            {
                crew.Fatigue -= 2;

                if (crew.Fatigue <= 0)
                {
                    crew.Fatigue = 0;
                    FinishCrewAction(crew);
                }

                return;
            }

            if (crew.Activity == Activity.Working)
            {
                if (State.TickCount - crew.ActionStartedTick >= MinimumActionDurationTicks)
                {
                    FinishCrewAction(crew);
                }

                return;
            }
        }

        private void FinishCrewAction(Crew crew)
        {
            Mapper.ReleaseWorkstation(State.MyShip, crew);

            crew.Action = CrewAction.None;
            crew.Activity = Activity.None;

            crew.IsEating = false;
            crew.IsSleeping = false;

            ClearCrewPath(crew);

            crew.NextActionDecisionTick =
                State.TickCount + ActionDecisionIntervalTicks;
        }

        private void MarkDirty()
        {
            //_hasUnsavedChanges = true; TODO for autosave
        }

        private void SyncAll()
        {
            OnPropertyChanged(nameof(MyShip));
            OnPropertyChanged(nameof(State.Crews));
            OnPropertyChanged(nameof(Planets));
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(JobOptions));

            RaiseCommandStates();
        }

        private void RaiseCommandStates()
        {
            //AssignGatherCommand.RaiseCanExecuteChanged();
            //AssignHuntCommand.RaiseCanExecuteChanged();
            //AssignFarmCommand.RaiseCanExecuteChanged();
            //AssignBuilderCommand.RaiseCanExecuteChanged();

        }
        #endregion Private Methods
    }
}
