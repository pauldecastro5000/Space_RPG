using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG
{
    public class ShipManager
    {
        #region Public Properties
        public Ship MyShip { get; set; }
        #endregion Public Properties

        #region Constructor
        public ShipManager()
        {
        }
        #endregion Constructor

        #region Public Methods
        public void CreateMyShip()
        {
            var weapons = new List<Ship.Weapon>();

            for (int i = 0; i < 3; i++)
            {
                weapons.Add(new Ship.Weapon() { Damage = 10, Health = 100});
            }

            MyShip = new Ship()
            {
                Engine = 1000,
                Food = 1000,
                Location = new System.Windows.Point(500, 500),
                State = Ship.state.Hovering,
                Weapons = weapons
            };
        }
        public Ship GetMyShip()
        {
            return MyShip;
        }
        #endregion Public Methods
    }
}
