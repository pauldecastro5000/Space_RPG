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
            Captain,
            Pilot,
            Weapons,
            Engineer
        }

        private enum Task
        {
            None,
            Eating,
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
        private string _strCurrentTask = "-";
        public string StrCurrentTask
        {
            get { return _strCurrentTask; }
            set { _strCurrentTask = value; OnPropertyChanged(); }
        }
        #endregion Public Properties

        #region Private Properties
        private bool _isBusy = false;
        //private List<string> _Command = new List<string>();
        private Task _currentTask = Task.None;
        private int _currentTaskId = 0;
        #endregion Private Properties

        #region Constructor
        public Crew()
        {
            MainWindow.UniverseTime.UniverseTickPerMin += UniverseTime_UniverseTickPerMin;
        }
        #endregion Constructor

        #region Public Methods
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
        private void UniverseTime_UniverseTickPerMin(object sender, EventArgs e)
        {
            if (Alive)
                Hunger -= MainWindow.Crew.hungerDepletion;
            else
                return;

            if (_isBusy)
            { 
                switch (_currentTask)
                {
                    case Task.StartEngine:
                        if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.On)
                            TaskDone();
                        break;
                    case Task.ShutOffEngine:
                        if (MainWindow.mainVm.MyShip.engine.State == Ship.Engine.state.Off)
                            TaskDone();
                        break;
                }
                return; 
            }

            var task = "";

            switch (Job)
            {
                case CrewJob.Pilot:
                    StrCurrentTask = "-";
                    task = MainWindow.mainVm.MyShip.GetCrewTask(Job, out int TaskId);
                    _currentTaskId = TaskId;
                    if (task == "")
                        return;
                    _currentTask = GetPilotTask(task);
                    switch (_currentTask)
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
            }
        }
        private void StartTask(string task)
        {
            StrCurrentTask = task;
            _isBusy = true;
        }
        private Task GetPilotTask(string command)
        {
            var cmd = command.ToUpper();
            // Highest Priority

            // High Priority
            if (Hunger < 70)
                return Task.Eating;

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
        private void TaskDone()
        {
            StrCurrentTask = " - ";
            _currentTask = Task.None;
            _isBusy = false;
            MainWindow.mainVm.MyShip.TaskDone(_currentTaskId);
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
