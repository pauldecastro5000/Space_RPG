using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Models
{
    public enum EngineState
    {
        On,
        Off
    }
    public class Engine
    {
        public double Health { get; set; }
        public double FuelCapacity { get; set; }
        public double CurrentFuel { get; set; }
        public double FuelPercent => CurrentFuel / FuelCapacity;
        public EngineState State { get; set; }

        public Engine()
        {
            Health = 100.0;
            FuelCapacity = 10000.0;
            CurrentFuel = 10000.0;
            State = EngineState.Off;
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
