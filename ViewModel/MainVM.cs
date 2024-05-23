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
		private ShipManager _shipManager;
		public ShipManager ShipManager
		{
			get { return _shipManager; }
			set { _shipManager = value; OnPropertyChanged(); }
		}

        BackgroundWorker _bgwShip;

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

            //_bgwShip = new BackgroundWorker();
            //_bgwShip.WorkerSupportsCancellation = true;
            //_bgwShip.DoWork += _bgwShip_DoWork;
            //_bgwShip.RunWorkerAsync();
        }

        private void _bgwShip_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_bgwShip.CancellationPending)
            {
                OnPropertyChanged(nameof(ShipManager));
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
