using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG
{
    public class Ship
    {
        #region Public Members
        public enum state
        {
            None,
            Docked,
            Flying,
            Hovering
        }
        #endregion Public Members

        #region Public Properties
        public int Engine { get; set; }
        public List<Weapon> Weapons { get; set; }
        public state State { get; set; }
        public Point Location { get; set; }
        public int Food { get; set; }

        #endregion Public Properties

        #region Public Methods

        #endregion Public Methods

        #region Public Class
        public class Weapon
        {
            public string Name { get; set; } = "Weapon 1";
            public int Health { get; set; }
            public int Damage { get; set; }
        }
        #endregion Public Class

    }
}
