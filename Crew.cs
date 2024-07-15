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
        public enum Task
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
            MainWindow.UniverseTime.UniverseTickPerMin += UniverseTime_UniverseTickPerMin;
        }

        private void UniverseTime_UniverseTickPerMin(object sender, EventArgs e)
        {
            if (Alive)
            {
                if (CurrentTask != Task.Eat)
                    Hunger -= MainWindow.Crew.hungerDepletionRate;

                TaskLoop();
            }
        }
        #endregion Constructor

        #region Public Methods
        public void TaskLoop()
        {
            // CHECK IF CREW IS HUNGRY
            if (Hunger < 70 && CurrentTask != Task.Eat)
            {
                if (MainWindow.mainVm.MyShip.Food > 0)
                {
                    MainWindow.mainVm.MyShip.Food--;
                    SavePrevTask();
                    StartTask(Task.Eat);
                    return;
                }
            }

            // CHECK CURRENT TASK STATUS
            if (_isBusy)
            {
                switch (CurrentTask)
                {       // GENERAL TASKS
                    case Task.Eat:
                        var foodFill = 1.5; // 30 hunger in 20 mins. // TODO: must be in settings
                        Hunger = Hunger + foodFill < 100 ? Hunger + foodFill : 100; // Slowly add to hunger
                        if (Hunger >= 100)
                        {
                            PersonalTaskDone();
                        }
                        break;
                    case Task.PrepareToLiftoff:
                        if (Job != CrewJob.Pilot)
                        {
                            StartTask(Task.TakeSeat);
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
                    case Task.StartEngine:
                    case Task.ShutdownEngine:
                    case Task.Liftoff:
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
                            StartTask(CurrentTask);
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
        private void StartTask(Task task)
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
            CurrentTask = Task.None;
            _isBusy = false;
        }
        private Task DecodePilotTask(string command)
        {
            var cmd = command.ToUpper();

            if (DecodeForAllTask(command, out Task task))
                return task;

            // Highest Priority


            // Medium Priority
            if (cmd.Contains("ENGINE"))
            {
                if (cmd.Contains("START"))
                    return Task.StartEngine;
                else if (cmd.Contains("SHUTOFF"))
                    return Task.ShutdownEngine;
            }

            return Task.None;
        }
        private void GetPilotTask(string task)
        {
            CurrentTask = DecodePilotTask(task);

            switch (CurrentTask)
            {
                case Task.StartEngine:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.On)
                    {
                        CrewLog("The engine is already on sir.");
                        TaskDone();
                        return;
                    }
                    StartTask(CurrentTask);
                    MainWindow.mainVm.MyShip.StartEngine();
                    break;
                case Task.ShutdownEngine:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.Off)
                    {
                        CrewLog("The engine is already shut down sir.");
                        TaskDone();
                        return;
                    }
                    StartTask(CurrentTask);
                    MainWindow.mainVm.MyShip.ShutOffEngine();
                    break;
                case Task.PrepareToLiftoff:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.Off)
                    {
                        CrewLog("The engine is not on sir.");
                        TaskDone();
                        return;
                    }
                    StartTask(CurrentTask);
                    break;
                case Task.Liftoff:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.Off)
                    {
                        CrewLog("The engine is not on sir.");
                        TaskDone();
                        return;
                    }
                    StartTask(CurrentTask);
                    MainWindow.mainVm.MyShip.Liftoff();
                    break;
                case Task.PrepareToLand:

                    break;
            }
        }
        private void DoPilotTask(Task task)
        {
            switch (task)
            {
                case Task.StartEngine:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.On)
                        TaskDone();
                    break;
                case Task.ShutdownEngine:
                    if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.Off)
                        TaskDone();
                    break;
                case Task.Liftoff:
                    if (MainWindow.mainVm.MyShip.State == Ship.state.Hovering)
                    {
                        CrewLog("We have successfully exited the planet");
                        TaskDone();
                    }
                    break;
            }
        }
        private Task GetNoJobTask(string command)
        {
            var cmd = command.ToUpper();

            if (DecodeForAllTask(command, out Task task))
                return task;

            // Highest Priority

            return Task.None;
        }
        private bool DecodeForAllTask(string command, out Task task)
        {
            var cmd = command.ToUpper();
            task = Task.None;

            // Highest Priority
            if (cmd.Contains("LIFTOFF"))
            {
                if (cmd.Contains("PREPARE"))
                { 
                    task = Task.PrepareToLiftoff;
                    return true;
                }
                else
                {
                    task = Task.Liftoff;
                    return true;
                } 
            }

            if (cmd.Contains("LAND"))
            {
                if (cmd.Contains("PREPARE"))
                { 
                    task = Task.PrepareToLand;
                    return true;
                }
                else
                { 
                    task = Task.Land;
                    return true;
                }
            }

            return false;
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
                ClearTask();
            }
            else
            {
                DoPrevTask();
            }
        }
        private Task GetEngineerTask(string command)
        {
            // Highest Priority

            // High Priority
            if (Hunger < 70)
                return Task.Eat;

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
        private void SavePrevTask()
        {
            _prevTask = CurrentTask;
            _strPrevTask = StrCurrentTask;
        }
        private string GetTaskActionText(Task task)
        {
            switch (task)
            {
                case Task.Eat:
                    state = State.Eating;
                    return "Eating";

                case Task.TakeSeat:
                    state = State.Seated;
                    return "Seated";

                case Task.StandbyForInstruction:
                    return "Waiting for instruction";

                case Task.PrepareToLiftoff:
                    return "Preparing for liftoff";

                case Task.Liftoff:
                    return "Lifting off";

                case Task.PrepareToLand:
                    return "Preparing for landing";

                case Task.Land:
                    return "Landing";

                // Pilot Tasks
                case Task.StartEngine:
                    return "Starting the engine";

                case Task.ShutdownEngine:
                    return "Shutting down engine";

                // Mechanic Tasks
                case Task.FixEngine:
                    return "Fixing the engine";

                case Task.FixWeapon:
                    return "Fixing the weapon";

                case Task.None:
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
