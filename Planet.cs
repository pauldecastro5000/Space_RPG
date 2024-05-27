using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG
{
    public class Planet : ViewModelBase
    {
        #region Private Variables
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        #endregion Private Variables

        #region Enums
        public enum Type
        {
            Unknown,
            Colonized,
            Uncolonized
        }
        #endregion

        #region Constructor
        public Planet()
        {
            GenerateResources();
        }
        #endregion Constructor

        #region Public Properties
        private Resources _resources = new Resources();
        public Resources resources
        {
            get { return _resources; }
            set { _resources = value; OnPropertyChanged(); }
        }
        private Point _location = new Point(0,0);
        public Point Location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged(); }
        }
        private Type _type;
        public Type type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }
        #endregion Public Properties

        #region Public Methods
        public int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
        public void MineFood(int amount, out int food)
        {
            food = 0;
            if (amount >= resources.Food)
            {
                food = amount;
                resources.Food -= amount;
            }
            else
            {
                food = resources.Food;
                resources.Food = 0;
            }
        }
        public void MineFuel(int amount, out int fuel)
        {
            fuel = 0;
            if (amount >= resources.Fuel)
            {
                fuel = amount;
                resources.Fuel -= amount;
            }
            else
            {
                fuel = resources.Fuel;
                resources.Fuel = 0;
            }
        }
        #endregion Public Methods

        #region Private Methods
        private void GenerateResources()
        {
            resources.Food = RandomNumber(0, 100);
            resources.Fuel = RandomNumber(0, 100);
        }
        #endregion Private Methods

        #region Public Class
        public class Resources : ViewModelBase
        {
            private int _food = 0;
            public int Food
            {
                get { return _food; }
                set { _food = value; OnPropertyChanged(); }
            }
            private int _fuel = 0;
            public int Fuel
            {
                get { return _fuel; }
                set { _fuel = value; OnPropertyChanged(); }
            }
        }
        #endregion Public Class
    }
}
