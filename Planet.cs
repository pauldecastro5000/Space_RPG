using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG
{
    public class Planet
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
        public Resources resources { get; private set; } = new Resources();
        public Point Location { get; set; } = new Point(0,0);
        public Type type { get; private set; } = Type.Unknown;
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
        public class Resources
        {
            public int Food { get; set; }
            public int Fuel { get; set; }
        }
        #endregion Public Class
    }
}
