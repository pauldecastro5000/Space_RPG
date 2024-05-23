using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG
{
    public class Ship
    {
        public enum state
        {
            None,
            Docked,
            Flying,
            Hovering
        }

        #region Public Properties
        public int Engine { get; set; }
        public int Health { get; set; }
        public state State { get; set; }
        public Location location { get; set; }

        #endregion Public Properties

        #region Public Class
        public class Location
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
        #endregion Public Class

    }
}
