using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG
{
    public class ShipManager : ViewModelBase
    {
        #region Public Properties
        private ObservableCollection<Ship> _ships = new ObservableCollection<Ship>();
        public ObservableCollection<Ship> Ships
        {
            get { return _ships; }
            set { _ships = value; OnPropertyChanged(); }
        }
        #endregion Public Properties

        #region Constructor
        public ShipManager()
        {
        }
        #endregion Constructor

        #region Public Methods
        public void CreateMyShip()
        {
            var weapons = new ObservableCollection<Ship.Weapon>();

            for (int i = 0; i < 3; i++)
            {
                weapons.Add(new Ship.Weapon() { Damage = 10, Health = 100});
            }

            Ships.Add(new Ship()
            {
                Engine = 1000,
                State = Ship.state.Hovering,
                Location = new System.Windows.Point(100, 100),
                Weapons = weapons,
                Food = 1000
            });
        }
        public Ship GetMyShip()
        {
            return Ships[0];
        }
        #endregion Public Methods
    }
}
