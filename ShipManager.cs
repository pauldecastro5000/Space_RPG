using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        #region Private Properties
        private Planet _planet;
        #endregion Private Properties

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
                weapons.Add(new Ship.Weapon() { 
                    Name = $"Turret{i+1}",
                    Damage = 10, 
                    MaxHealth = 100,
                    CurrentHealth = 100,
                });
            }

            Ships.Add(new Ship()
            {
                engine = new Ship.Engine() { 
                    Health = 1000, 
                    FuelCapacity = 10000, 
                    State = Ship.Engine.state.Off 
                },
                State = Ship.state.Docked,
                Location = _planet.Location,
                Weapons = weapons,
                Food = 1000
            });
        }
        public void loadPlanet(Planet planet)
        {
            _planet = planet;
        }
        #endregion Public Methods
    }
}
