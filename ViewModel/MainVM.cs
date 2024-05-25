﻿using System;
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
		private ShipManager _shipManager;
		public ShipManager ShipManager
		{
			get { return _shipManager; }
			set { _shipManager = value; OnPropertyChanged(); }
		}

        private CrewManager _crewManager;
        public CrewManager CrewManager
        {
            get { return _crewManager; }
            set { _crewManager = value; OnPropertyChanged(); }
        }

        BackgroundWorker _bgwUpdate;

        public MainVM()
        {
            //ShipManager = new ShipManager()
            //{
            //    Ship = 100,
            //    Weapons = 100,
            //    Engine = 100,
            //    Food = 100
            //};

            ShipManager = MainWindow.Ship;
            CrewManager = MainWindow.Crew;

            _bgwUpdate = new BackgroundWorker();
            _bgwUpdate.WorkerSupportsCancellation = true;
            _bgwUpdate.DoWork += _bgwUpdate_DoWork;
            _bgwUpdate.RunWorkerAsync();
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
