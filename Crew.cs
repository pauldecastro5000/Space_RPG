using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;

namespace Space_RPG
{
    public class Crew : ViewModelBase
    {
        #region Public Members
        public enum CrewJob
        {
            None,
            All,
            Captain,
            Pilot,
            Weapons,
            Engineer
        }
        public enum State
        {
            None,
            // General Tasks
            Eating,
            Seated,
            StandbyForInstruction,
            PreparingToLiftoff,
            Liftingoff,
            PreparingToLand,
            Landing,

            // Pilot Tasks
            StartingEngine,
            ShuttingdownEngine,

            // Mechanic Tasks
            FixingEngine,
            FixingWeapon
        }
        public enum TaskType
        {
            None,
            // General Tasks
            Eat,
            TakeSeat,
            StandbyForInstruction,
            PrepareToLiftoff, // Prepare to liftoff
            Liftoff,          // Liftoff
            PrepareToLand,    // Prepare to land
            Land,             // Land

            // Pilot Tasks
            StartEngine,      // Start the engine
            ShutdownEngine,    // shutoff the engine

            // Mechanic Tasks
            FixEngine,        // fix the engine
            FixWeapon         // fix weapon#
        }
        #endregion Public Members

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
        private State _state;
        public State state
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged(); }
        }
        
        public TaskType CurrentTask { get; set; } = TaskType.None;
        #endregion Public Properties

        #region Private Properties
        private bool _isBusy = false;
        //private List<string> _Command = new List<string>();
        private TaskType _prevTask = TaskType.None;
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
        public void UpdateHunger()
        {
            if (CurrentTask != TaskType.Eat)
                Hunger -= MainWindow.CrewMgr.hungerDepletion;
        }
        public void TaskLoop()
        {
            // CHECK IF CREW IS HUNGRY
            if (Hunger < 70 && CurrentTask != TaskType.Eat)
            {
                if (MainWindow.mainVm.MyShip.Food > 0)
                {
                    MainWindow.mainVm.MyShip.Food--;
                    SavePrevTask();
                    StartTask(TaskType.Eat);
                    return;
                }
            }

            // CHECK CURRENT TASK STATUS
            if (_isBusy)
            {
                switch (CurrentTask)
                {       // GENERAL TASKS
                    case TaskType.Eat:



                        var foodFill = 1.5; // 30 hunger in 20 mins. // TODO: must be in settings
                        Hunger = Hunger + foodFill < 100 ? Hunger + foodFill : 100;
                        if (Hunger >= 100)
                        {
                            PersonalTaskDone();
                        }
                        break;
                    case TaskType.PrepareToLiftoff:
                        if (Job != CrewJob.Pilot)
                        {
                            StartTask(TaskType.TakeSeat);
                        }
                        else
                        {
                            if (!MainWindow.mainVm.MyShip.AllCrewSeated())
                                return;
                            CrewLog("All crew are seated.");
                            CrewLog("Ship is ready for liftoff on your command Captain.");
                            TaskDone();
                        }
                        break;

                    // PILOT TASKS
                    case TaskType.StartEngine:
                    case TaskType.ShutdownEngine:
                    case TaskType.Liftoff:
                        DoPilotTask(CurrentTask);
                        break;


                }
                return;
            }

            // GET NEW TASK
            var task = "";
            StrCurrentTask = " - ";
            task = MainWindow.mainVm.MyShip.GetCrewTask(Job, out int TaskId);
            if (task == "")
                return;
            _currentTaskId = TaskId;

            switch (Job)
            {
                case CrewJob.Pilot:
                    GetPilotTask(task);
                    break;

                case CrewJob.Weapons:

                    break;

                case CrewJob.Engineer:
                    //selectedTask = GetEngineerTask(_Command.First());
                    //switch (selectedTask)
                    //{
                    //    case TaskType.FixEngine:

                    //        break;
                    //    case TaskType.FixWeapon:

                    //        break;
                    //}
                    break;
                case CrewJob.None:
                    CurrentTask = GetNoJobTask(task);
                    switch (CurrentTask)
                    {
                        case TaskType.PrepareToLiftoff:
                            StartTask(CurrentTask);
                            break;

                        case TaskType.PrepareToLand:
                            StartTask(CurrentTask);
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
        private void StartTask(TaskType task)
        {
            CurrentTask = task;
            StrCurrentTask = GetTaskActionText(task);
            _isBusy = true;
        }
        private void DoPrevTask()
        {
            CurrentTask = _prevTask;
            StrCurrentTask = _strPrevTask;
        }
        private void ClearTask()
        {
            StrCurrentTask = " - ";
            CurrentTask = TaskType.None;
            _isBusy = false;
        }
        private TaskType DecodePilotTask(string command)
        {
            var cmd = command.ToUpper();

            if (DecodeForAllTask(command, out TaskType task))
                return task;

            // Highest Priority


            // Medium Priority
            if (cmd.Contains("ENGINE"))
            {
                if (cmd.Contains("START"))
                    return TaskType.StartEngine;
                else if (cmd.Contains("SHUTOFF"))
                    return TaskType.ShutdownEngine;
            }

            return TaskType.None;
        }
        private void GetPilotTask(string task)
        {
            CurrentTask = DecodePilotTask(task);

            switch (CurrentTask)
            {
                case TaskType.StartEngine:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.On)
                    {
                        CrewLog("The engine is already on sir.");
                        TaskDone();
                        return;
                    }
                    StartTask(CurrentTask);
                    MainWindow.mainVm.MyShip.StartEngine();
                    break;
                case TaskType.ShutdownEngine:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.Off)
                    {
                        CrewLog("The engine is already shut down sir.");
                        TaskDone();
                        return;
                    }
                    StartTask(CurrentTask);
                    MainWindow.mainVm.MyShip.ShutOffEngine();
                    break;
                case TaskType.PrepareToLiftoff:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.Off)
                    {
                        CrewLog("The engine is not on sir.");
                        TaskDone();
                        return;
                    }
                    StartTask(CurrentTask);
                    break;
                case TaskType.Liftoff:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.Off)
                    {
                        CrewLog("The engine is not on sir.");
                        TaskDone();
                        return;
                    }
                    StartTask(CurrentTask);
                    MainWindow.mainVm.MyShip.Liftoff();
                    break;
                case TaskType.PrepareToLand:

                    break;
            }
        }
        private void DoPilotTask(TaskType task)
        {
            switch (task)
            {
                case TaskType.StartEngine:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.On)
                        TaskDone();
                    break;
                case TaskType.ShutdownEngine:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.Off)
                        TaskDone();
                    break;
                case TaskType.Liftoff:
                    if (MainWindow.mainVm.MyShip.State == Ship.state.Hovering)
                    {
                        CrewLog("We have successfully exited the planet");
                        TaskDone();
                    }
                    break;
            }
        }
        private TaskType GetNoJobTask(string command)
        {
            var cmd = command.ToUpper();

            if (DecodeForAllTask(command, out TaskType task))
                return task;

            // Highest Priority

            return TaskType.None;
        }
        private bool DecodeForAllTask(string command, out TaskType task)
        {
            var cmd = command.ToUpper();
            task = TaskType.None;

            // Highest Priority
            if (cmd.Contains("LIFTOFF"))
            {
                if (cmd.Contains("PREPARE"))
                { 
                    task = TaskType.PrepareToLiftoff;
                    return true;
                }
                else
                {
                    task = TaskType.Liftoff;
                    return true;
                } 
            }

            if (cmd.Contains("LAND"))
            {
                if (cmd.Contains("PREPARE"))
                { 
                    task = TaskType.PrepareToLand;
                    return true;
                }
                else
                { 
                    task = TaskType.Land;
                    return true;
                }
            }

            return false;
        }
        private void TaskDone()
        {
            StrCurrentTask = " - ";
            CurrentTask = TaskType.None;
            _isBusy = false;
            MainWindow.mainVm.MyShip.TaskDone(_currentTaskId);
        }
        private void PersonalTaskDone()
        {
            if (_prevTask == TaskType.None)
            {
                ClearTask();
            }
            else
            {
                DoPrevTask();
            }
        }
        private TaskType GetEngineerTask(string command)
        {
            // Highest Priority

            // High Priority
            if (Hunger < 70)
                return TaskType.Eat;

            // Medium Priority



            return TaskType.None;
        }
        private void CrewLog(string message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                MainWindow.mainVm.Log.Add($"{Name}: {message}");
            });
        }
        private void SavePrevTask()
        {
            _prevTask = CurrentTask;
            _strPrevTask = StrCurrentTask;
        }
        private string GetTaskActionText(TaskType task)
        {
            switch (task)
            {
                case TaskType.Eat:
                    state = State.Eating;
                    return "Eating";

                case TaskType.TakeSeat:
                    state = State.Seated;
                    return "Seated";

                case TaskType.StandbyForInstruction:
                    return "Waiting for instruction";

                case TaskType.PrepareToLiftoff:
                    return "Preparing for liftoff";

                case TaskType.Liftoff:
                    return "Lifting off";

                case TaskType.PrepareToLand:
                    return "Preparing for landing";

                case TaskType.Land:
                    return "Landing";

                // Pilot Tasks
                case TaskType.StartEngine:
                    return "Starting the engine";

                case TaskType.ShutdownEngine:
                    return "Shutting down engine";

                // Mechanic Tasks
                case TaskType.FixEngine:
                    return "Fixing the engine";

                case TaskType.FixWeapon:
                    return "Fixing the weapon";

                case TaskType.None:
                    state = State.None;
                    return " - ";
            }
            return "Unknown Task item";
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

        #region Private Class

        #endregion Private Class
    }


}
