using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.ViewModel
{
    public class MainVM :ViewModelBase
    {
		//private ShipManager _shipManager;
		//public ShipManager ShipManager
		//{
		//	get { return _shipManager; }
		//	set { _shipManager = value; OnPropertyChanged(); }
		//}

        private Ship _myShip;
        public Ship MyShip
        {
            get { return _myShip; }
            set { _myShip = value; OnPropertyChanged(); }
        }

        //private Crew _myCaptain;
        //public Crew MyCaptain
        //{
        //    get { return _myCaptain; }
        //    set { _myCaptain = value; OnPropertyChanged(); }
        //}

        private CrewManager _crewManager;
        public CrewManager CrewManager
        {
            get { return _crewManager; }
            set { _crewManager = value; OnPropertyChanged(); }
        }

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

        BackgroundWorker _bgwUpdate;

        public MainVM()
        {
            CrewManager = MainWindow.Crew;

            //_bgwUpdate = new BackgroundWorker();
            //_bgwUpdate.WorkerSupportsCancellation = true;
            //_bgwUpdate.DoWork += _bgwUpdate_DoWork;
            //_bgwUpdate.RunWorkerAsync();
        }

        private void _bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_bgwUpdate.CancellationPending)
            {
                OnPropertyChanged(nameof(ShipManager));
                OnPropertyChanged(nameof(CrewManager));
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
