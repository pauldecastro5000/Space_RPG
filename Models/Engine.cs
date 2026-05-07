using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Models
{
    public class Engine
    {

        public int Health { get; set; }
        public double FuelCapacity { get; set; } = 10000.0;
        public double CurrentFuel { get; set; } = 10000.0;
        public double FuelPercent => CurrentFuel / FuelCapacity;

        public Engine()
        {
            FuelCapacity = 10000;
            CurrentFuel = 10000;
        }


        //public void ConsumeFuel(double amount)
        //{
        //    CurrentFuel = CurrentFuel > amount ? CurrentFuel - amount : 0;
        //    if (CurrentFuel == 0)
        //        MainWindow.mainVm.MyShip.ShutOffEngine();
        //    FuelPercent = (CurrentFuel / FuelCapacity) * 100;
        //}
        //public void AddFuel(double amount)
        //{
        //    CurrentFuel = CurrentFuel + amount < FuelCapacity ? CurrentFuel + amount : FuelCapacity;
        //    FuelPercent = (CurrentFuel / FuelCapacity) * 100;
        //}
    }
}
