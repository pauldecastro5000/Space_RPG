using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG
{
    public class Ship : ViewModelBase
    {
        #region Public Members
        public enum state
        {
            None,
            Docked,
            Landed,
            Warping,
            Hovering
        }
        #endregion Public Members

        #region Public Properties
        private int _engine;
        public int Engine
        {
            get { return _engine; }
            set { _engine = value; OnPropertyChanged(); }
        }
        private string _captain;
        public string Captain
        {
            get { return _captain; }
            set { _captain = value; OnPropertyChanged(); }
        }

        private string _pilot = "None";
        public string Pilot
        {
            get { return _pilot; }
            set { _pilot = value; OnPropertyChanged(); }
        }
        private double _pilotSkill;
        public double PilotSkill
        {
            get { return _pilotSkill; }
            set { _pilotSkill = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Weapon> _weapons;
        public ObservableCollection<Weapon> Weapons
        {
            get { return _weapons; }
            set { _weapons = value; OnPropertyChanged(); }
        }
        private state _state;
        public state State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged(); }
        }
        private Point _location;
        public Point Location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged(); }
        }
        private int _food;
        public int Food
        {
            get { return _food; }
            set { _food = value; OnPropertyChanged(); }
        }

        #endregion Public Properties

        #region Private Properties

        #endregion Private Properties

        #region Public Methods
        public void Assign(string name, string assignedTo)
        {
            switch (assignedTo.ToUpper())
            {
                case "CAPTAIN":
                    Captain = name;
                    break;

                case "PILOT":
                    Pilot = name;
                    break;
            }
        }
        #endregion Public Methods

        #region Public Class
        public class Weapon : ViewModelBase
        {
            private string _name = "Weapon 1";
            public string Name
            {
                get { return _name; }
                set { _name = value; OnPropertyChanged(); }
            }
            private string _gunner = "None";
            public string Gunner
            {
                get { return _gunner; }
                set { _gunner = value; OnPropertyChanged(); }
            }

            private int _health;
            public int Health
            {
                get { return _health; }
                set { _health = value; OnPropertyChanged(); }
            }
            private int _damage;
            public int Damage
            {
                get { return _damage; }
                set { _damage = value; OnPropertyChanged(); }
            }
        }
        #endregion Public Class

    }
}
