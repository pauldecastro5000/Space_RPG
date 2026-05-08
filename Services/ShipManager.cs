using System;
using Space_RPG.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG.Services
{
    public class ShipManager : ViewModelBase
    {
        #region Public Properties

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
        public Ship CreateMyShip()
        {
            // Create Weapons
            var weapons = new ObservableCollection<ShipWeapon>();
            for (int i = 0; i < 2; i++)
            {
                weapons.Add(new ShipWeapon()
                {
                    Type = WeaponType.Turret
                });
            }

            // Create Facilities
            var facilities = new ObservableCollection<Facility>();
            for (int i = 0; i < Enum.GetValues(typeof(Facility.FacilityType)).Length - 1; i++)
            {
                facilities.Add(new Facility()
                {
                    Type = (Facility.FacilityType)Enum.GetValues(typeof(Facility.FacilityType)).GetValue(i)
                });
            }

            var newShip = new Ship()
            {
                Weapons = weapons,
                Facilities = facilities
            };

            return newShip;
        }
        public void loadPlanet(Planet planet)
        {
            _planet = planet;
        }
        #endregion Public Methods
    }
}
