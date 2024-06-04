using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;

namespace Space_RPG
{
    public class Crew : ViewModelBase
    {
        public enum CrewJob
        {
            None,
            All,
            Captain,
            Pilot,
            Weapons,
            Engineer
        }

        public enum Task
        {
            None,
            // General Tasks
            Eating,
            Seated,
            PrepareToLiftoff,
            Liftingoff,
            PrepareToLand,
            Landing,

            // Pilot Tasks
            StartEngine,     // Start the engine
            ShutOffEngine,   // shutoff the engine

            // Mechanic Tasks
            FixEngine,       // fix the engine
            FixWeapon        // fix weapon#
        }

        #region Public Properties
        private string _name = "unknown";
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }
        private CrewJob _job = CrewJob.None;
        public CrewJob Job
        {
            get { return _job; }
            set { _job = value; OnPropertyChanged(); }
        }
        private double _hunger = 100.0;
        public double Hunger
        {
            get { return _hunger; }
            set { _hunger = value; OnPropertyChanged(); }
        }
        private skills _skills;
        public skills Skills
        {
            get { return _skills; }
            set { _skills = value; OnPropertyChanged(); }
        }
        private int _cash = 0;
        public int Cash
        {
            get { return _cash; }
            set { _cash = value; OnPropertyChanged(); }
        }
        private bool _alive = true;
        public bool Alive
        {
            get { return _alive; }
            set { _alive = value; OnPropertyChanged(); }
        }
        private double _health = 100;
        public double Health
        {
            get { return _health; }
            set { _health = value; OnPropertyChanged(); }
        }
        private string _strCurrentTask = " - ";
        public string StrCurrentTask
        {
            get { return _strCurrentTask; }
            set { _strCurrentTask = value; OnPropertyChanged(); }
        }
        private int _price = 0;
        public int Price
        {
            get { return _price; }
            set { _price = value; OnPropertyChanged(); }
        }
        public Task CurrentTask { get; set; } = Task.None;
        #endregion Public Properties

        #region Private Properties
        private bool _isBusy = false;
        //private List<string> _Command = new List<string>();
        private Task _prevTask = Task.None;
        private string _strPrevTask = " - ";
        private int _currentTaskId = 0;
        #endregion Private Properties

        #region Constructor
        public Crew()
        {
            //MainWindow.UniverseTime.UniverseTickPerMin += UniverseTime_UniverseTickPerMin;
        }
        #endregion Constructor

        #region Public Methods
        public void TaskLoop()
        {
            if (Alive)
            {
                if (CurrentTask != Task.Eating)
                    Hunger -= MainWindow.Crew.hungerDepletion;
            }
            else
                return;



            // CHECK IF CREW IS HUNGRY
            if (Hunger < 70 && CurrentTask != Task.Eating)
            {
                if (MainWindow.mainVm.MyShip.Food > 0)
                {
                    MainWindow.mainVm.MyShip.Food--;
                    _prevTask = CurrentTask;
                    _strPrevTask = StrCurrentTask;
                    CurrentTask = Task.Eating;
                    StrCurrentTask = "Eating";
                    _isBusy = true;
                    return;
                }
            }

            // CHECK CURRENT TASK
            if (_isBusy)
            {
                switch (CurrentTask)
                {       // GENERAL TASKS
                    case Task.Eating:
                        var foodFill = 1.5; // 30 hunger in 20 mins. // TODO: must be in settings
                        Hunger = Hunger + foodFill < 100 ? Hunger + foodFill : 100;
                        if (Hunger >= 100)
                        {
                            PersonalTaskDone();
                        }
                        break;
                    case Task.PrepareToLiftoff:
                        if (Job != CrewJob.Pilot)
                        {
                            StartTask("Seated");
                            CurrentTask = Task.Seated;
                        }
                        else
                        {
                            if (!MainWindow.mainVm.MyShip.AllCrewSeated())
                                return;
                            CrewLog("All crew are ready.");
                            CrewLog("Initiate liftoff.");
                            StrCurrentTask = "Liftingoff ship";
                            CurrentTask = Task.Liftingoff;
                        }
                        break;

                    // PILOT TASKS
                    case Task.StartEngine:
                        if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.On)
                            TaskDone();
                        break;
                    case Task.ShutOffEngine:
                        if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.Off)
                            TaskDone();
                        break;

                    case Task.Liftingoff:

                        break;


                }
                return;
            }

            var task = "";

            StrCurrentTask = " - ";
            task = MainWindow.mainVm.MyShip.GetCrewTask(Job, out int TaskId);
            if (task == "")
                return;
            _currentTaskId = TaskId;

            switch (Job)
            {
                case CrewJob.Pilot:
                    CurrentTask = GetPilotTask(task);
                    switch (CurrentTask)
                    {
                        case Task.StartEngine:
                            if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.On)
                            {
                                CrewLog("The engine is already on sir.");
                                TaskDone();
                                return;
                            }
                            StartTask("Starting the engine");
                            MainWindow.mainVm.MyShip.StartEngine();
                            break;
                        case Task.ShutOffEngine:
                            if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.Off)
                            {
                                CrewLog("The engine is already shut down sir.");
                                TaskDone();
                                return;
                            }
                            StartTask("Shutting off the engine");
                            MainWindow.mainVm.MyShip.ShutOffEngine();
                            break;
                        case Task.PrepareToLiftoff:
                            if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.On)
                            {
                                CrewLog("The engine is not on sir.");
                                TaskDone();
                                return;
                            }
                            StartTask("Preparing to liftoff");
                            break;

                        case Task.PrepareToLand:

                            break;
                    }
                    break;

                case CrewJob.Weapons:

                    break;

                case CrewJob.Engineer:
                    //selectedTask = GetEngineerTask(_Command.First());
                    //switch (selectedTask)
                    //{
                    //    case Task.FixEngine:

                    //        break;
                    //    case Task.FixWeapon:

                    //        break;
                    //}
                    break;
                case CrewJob.None:
                    CurrentTask = GetNoJobTask(task);
                    switch (CurrentTask)
                    {
                        case Task.PrepareToLiftoff:
                            StartTask("Preparing for liftoff");
                            break;

                        case Task.PrepareToLand:

                            break;
                    }
                    break;
            }
        }
        public void AddCommand(string task)
        {
            //_Command.Add(task);
        }

        public Crew GetCrew()
        {
            return this;
        }
        #endregion Public Methods

        #region Private Methods
        private void StartTask(string task)
        {
            StrCurrentTask = task;
            _isBusy = true;
        }
        private Task GetPilotTask(string command)
        {
            var cmd = command.ToUpper();
            // Highest Priority
            if (cmd.Contains("PREPARE"))
            {
                if (cmd.Contains("LIFTOFF"))
                    return Task.PrepareToLiftoff;
                if (cmd.Contains("LAND"))
                    return Task.PrepareToLand;
            }

            // Medium Priority
            if (cmd.Contains("ENGINE"))
            {
                if (cmd.Contains("START"))
                    return Task.StartEngine;
                else if (cmd.Contains("SHUTOFF"))
                    return Task.ShutOffEngine;
            }

            return Task.None;
        }
        private Task GetNoJobTask(string command)
        {
            var cmd = command.ToUpper();

            // Highest Priority
            if (cmd.Contains("PREPARE"))
            {
                if (cmd.Contains("LIFTOFF"))
                    return Task.PrepareToLiftoff;
                if (cmd.Contains("LAND"))
                    return Task.PrepareToLand;
            }

            return Task.None;
        }
        private void TaskDone()
        {
            StrCurrentTask = " - ";
            CurrentTask = Task.None;
            _isBusy = false;
            MainWindow.mainVm.MyShip.TaskDone(_currentTaskId);
        }
        private void PersonalTaskDone()
        {
            if (_prevTask == Task.None)
            {
                StrCurrentTask = " - ";
                CurrentTask = Task.None;
                _isBusy = false;
            }
            else
            {
                CurrentTask = _prevTask;
                StrCurrentTask = _strPrevTask;
            }

        }
        private Task GetEngineerTask(string command)
        {
            // Highest Priority

            // High Priority
            if (Hunger < 70)
                return Task.Eating;

            // Medium Priority



            return Task.None;
        }
        private void CrewLog(string message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                MainWindow.mainVm.Log.Add($"{Name}: {message}");
            });
        }
        #endregion Private Methods

        #region Public Class
        public class skills : ViewModelBase
        {
            private double _piloting = 0.0;
            public double Piloting
            {
                get { return _piloting; }
                set { _piloting = value; OnPropertyChanged(); }
            }
            private double _aiming = 0.0;
            public double Aiming
            {
                get { return _aiming; }
                set { _aiming = value; OnPropertyChanged(); }
            }
            private double _engineRepair = 0.0;
            public double EngineRepair
            {
                get { return _engineRepair; }
                set { _engineRepair = value; OnPropertyChanged(); }
            }
            private double _weaponsRepair = 0.0;
            public double WeaponsRepair
            {
                get { return _weaponsRepair; }
                set { _weaponsRepair = value; OnPropertyChanged(); }
            }
        }
        #endregion Public Class
    }


}
