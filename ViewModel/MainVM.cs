using System;
using System.Collections.Generic;
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

        public MainVM()
        {
            ShipManager = new ShipManager()
            {
                Ship = 100,
                Weapons = 100,
                Engine = 100,
                Food = 100
            };
        }
    }
}
