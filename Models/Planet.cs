using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG.Models
{
    public class Planet
    {
        #region Public Properties
        //public PlanetResources Resources { get; set; }
        public Point Location { get; set; }
        //public PlanetType Type { get; set; }

        #endregion Public Properties





        //#region Enums
        //public enum PlanetType
        //{
        //    Unknown,
        //    Colonized,
        //    Uncolonized
        //}
        //#endregion

        //#region Private Variables
        //private static readonly Random random = new Random();
        //private static readonly object syncLock = new object();
        //#endregion Private Variables

        #region Constructor
        public Planet()
        {
            //GenerateResources();
            //Resources = new PlanetResources();

            //Location = new Point(0, 0);

            //Type = Type.Unknown
        }
        #endregion Constructor



        //#region Public Methods
        //public int RandomNumber(int min, int max)
        //{
        //    lock (syncLock)
        //    { // synchronize
        //        return random.Next(min, max);
        //    }
        //}
        //public void MineFood(int amount, out int food)
        //{
        //    food = 0;
        //    if (amount >= resources.Food)
        //    {
        //        food = amount;
        //        resources.Food -= amount;
        //    }
        //    else
        //    {
        //        food = resources.Food;
        //        resources.Food = 0;
        //    }
        //}
        //public void MineFuel(int amount, out int fuel)
        //{
        //    fuel = 0;
        //    if (amount >= resources.Fuel)
        //    {
        //        fuel = amount;
        //        resources.Fuel -= amount;
        //    }
        //    else
        //    {
        //        fuel = resources.Fuel;
        //        resources.Fuel = 0;
        //    }
        //}
        //#endregion Public Methods

        //#region Private Methods
        //private void GenerateResources()
        //{
        //    resources.Food = RandomNumber(0, 100);
        //    resources.Fuel = RandomNumber(0, 100);
        //}
        //#endregion Private Methods

        //#region Public Class
        //public class PlanetResources : ViewModelBase
        //{
        //    private int _food = 0;
        //    public int Food
        //    {
        //        get { return _food; }
        //        set { _food = value; OnPropertyChanged(); }
        //    }
        //    private int _fuel = 0;
        //    public int Fuel
        //    {
        //        get { return _fuel; }
        //        set { _fuel = value; OnPropertyChanged(); }
        //    }
        //}
        //#endregion Public Class
    }
}
