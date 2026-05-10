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

        private readonly Timer _gameLoopTimer;
        private readonly Dispatcher _uiDispatcher;
        private readonly Stopwatch _gameLoopStopwatch;

        private readonly PlanetManager planetMgr = new PlanetManager();
        private readonly CrewManager crewMgr;
        private readonly Utilities util;
        private readonly ShipManager shipMgr = new ShipManager();

        private GameState _state;
        private int _gameLoopCallbackQueued;
        private long _lastGameLoopMs;
        private double _gameTickAccumulatorMs;


        private bool _isGameTickRunning;
        BackgroundWorker _bgwUpdate;

        public ObservableCollection<string> LogEntries { get; private set; }
        #endregion Members

        #region Properties
        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set { _statusText = value; OnPropertyChanged(); }
        }
        private Ship _myShip;
        public Ship MyShip
        {
            get { return _myShip; }
            set { _myShip = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Crew> _crews = new ObservableCollection<Crew>();
        public ObservableCollection<Crew> Crews
        {
            get { return _crews; }
            set { _crews = value; OnPropertyChanged(); }
        }

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

        private ObservableCollection<Planet> _planets;
        public ObservableCollection<Planet> Planets
        {
            get { return _planets; }
            set { _planets = value; OnPropertyChanged(); }
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
            OpenApplicantsCommand = new RelayCommand(OpenApplicants);
            #endregion Applicants


            _uiDispatcher = Dispatcher.CurrentDispatcher;
            _gameLoopStopwatch = Stopwatch.StartNew();
            _lastGameLoopMs = 0;

            _gameLoopTimer = new Timer(OnGameLoopTimer, null, Timeout.Infinite, Timeout.Infinite);

            ResetGame();
            _gameLoopTimer.Change(0, 50);
        }
        #endregion Constructor

        private void ResetGame()
        {
            _state = CreateNewGame();

            LogEntries.Clear();

            SyncAll();
        }

        private void CreateFirstPlanet()
        {
            Planets = new ObservableCollection<Planet>();
            var newPlanet = planetMgr.CreateColonizedPlanet(Planets);
            Planets.Add(newPlanet);
        }

        private void CreateMyShip()
        {
            MyShip = shipMgr.CreateMyShip();
        }

        private void CreatePlayer()
        {
            var newPlayer = crewMgr.CreatePlayer("Paul");
            newPlayer.IsInShip = true;
            newPlayer.ShipId = MyShip.Id;
            Crews.Add(newPlayer);
        }

        private void PlaceShipInFirstPlanet()
        {
            MyShip.Location = Planets[0].Location;
            PlanetType = Planets[0].Type.ToString();
        }

        private void PlacePlayerInFirstShip()
        {
            var facility = MyShip.Facilities.FirstOrDefault(x => x.Type == FacilityType.MainDeck);
            if (facility != null)
            {
                facility.CrewIds.Add(Crews[0].Id);
            }
        }

        private GameState CreateNewGame()
        {
            GameState state = new GameState();
            state.TickCount = 0;

            state.TimeOfDay = 360;
            state.MinutesPerTick = 1;


            CreateFirstPlanet();
            CreateMyShip();
            CreatePlayer();
            PlaceShipInFirstPlanet();
            PlacePlayerInFirstShip();
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

                var newCrews = crewMgr.ApplicantsToCrews(vm.SelectedApplicants);

                foreach (var crew in newCrews)
                {
                    crew.IsInShip = true;
                    crew.ShipId = MyShip.Id;
                    MyShip.Facilities[0].CrewIds.Add(crew.Id);
                }

                util.AddRange(Crews, newCrews);
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
            _state.TickCount++;
            AdvanceClock();

            foreach (Crew crew in Crews.Where(v => v.IsAlive && !v.IsPlayer))
            {
                UpdateNeeds(crew);
                UpdateCrew(crew);
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

        private void UpdateCrew(Crew crew)
        {
            if (!crew.IsAlive)
                return;

            // Get Desired Action
            var action = AIActionSelector.GetAction(crew);

            // Set Target location based on action


            //



            //// Rest if not in battle mode
            //if (crew.Fatigue > 80 &&
            //    ((!_state.IsInShipBattleMode && crew.IsInShip) || (!_state.IsInRoverBattleMode && crew.IsInRover)))
            //{
            //    SendCrewToSleep(crew);
            //}


            //if (!IsDayTime())
            //{
            //    SendVillagerToBed(crew);
            //    return;
            //}

            //if (crew.IsSleeping || crew.IsInsideBuilding)
            //{
            //    WakeVillager(crew);
            //    if (crew.IsSleeping || crew.IsInsideBuilding)
            //        return;
            //}

            //if (crew.IsReturningHome)
            //{
            //    ReturnHome(crew);
            //    return;
            //}

            //if (crew.CurrentRole == VillagerTaskType.Idle)
            //{
            //    if (crew.X != crew.HomeX || crew.Y != crew.HomeY)
            //        MoveTowards(crew, crew.HomeX, crew.HomeY);
            //    return;
            //}

            //JobModel currentJob = ResolveCurrentJob(crew);

            //if (currentJob == null)
            //{
            //    currentJob = FindBestJobForVillager(crew);
            //    if (currentJob == null)
            //        return;

            //    ReserveJob(crew, currentJob);
            //}

            //if (!EnsureVillagerPreparedForJob(crew, currentJob))
            //    return;

            //if (crew.X != currentJob.WorkX || crew.Y != currentJob.WorkY)
            //{
            //    MoveTowards(crew, currentJob.WorkX, currentJob.WorkY);
            //    return;
            //}

            //PerformJobWork(crew, currentJob);
        }



        private void UpdateNeeds(Crew crew)
        {
            crew.Hunger = Math.Max(0, crew.Hunger - 0.1f);
            return;


            if (crew.IsSleeping)
            {
                // Sleeping crews should recover fatigue instead of gaining more fatigue.
                // Keep this in UpdateNeeds so the value changes every game tick while they remain asleep.
                crew.Fatigue = Math.Max(0, crew.Fatigue - 4);
            }
            else
            {
                crew.Fatigue = Math.Min(100, crew.Fatigue + 1);
            }

            if (crew.IsEating && _state.TickCount % 2 == 0)
            {
                crew.Hunger = Math.Min(100, crew.Hunger + 1);
            }

            if (crew.Health <= 0)
            {
                string reason = "something";// damageReasons.Count > 0 ? string.Join(" and ", damageReasons) : "unknown causes";
                Killcrew(crew, reason);
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
            _state.TimeOfDay += _state.MinutesPerTick;
            if (_state.TimeOfDay >= 1440)
                _state.TimeOfDay -= 1440;
        }

        private void MarkDirty()
        {
            //_hasUnsavedChanges = true; TODO for autosave
        }

        private void SyncAll()
        {
            OnPropertyChanged(nameof(MyShip));
            OnPropertyChanged(nameof(Crews));
            OnPropertyChanged(nameof(Planets));
            //OnPropertyChanged(nameof(Meat));

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
