using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG
{
    public class ShipManager : ViewModelBase
    {
        #region Public Properties
        private ObservableCollection<Ship> _ships = new ObservableCollection<Ship>();
        public ObservableCollection<Ship> Ships
        {
            get { return _ships; }
            set { _ships = value; OnPropertyChanged(); }
        }
        #endregion Public Properties

        #region Private Properties
        private Planet _planet;
        #endregion Private Properties

        #region Constructor
        public ShipManager()
        {
            MainWindow.UniverseTime.UniverseTickPerMin += UniverseTime_UniverseTickPerMin;
        }

        private void UniverseTime_UniverseTickPerMin(object sender, EventArgs e)
        {
            //if (MainWindow.mainVm.MyShip.Crews == null || MainWindow.mainVm.MyShip.Crews.Count == 0)
            //    return;

            //double hungerRatio = 0.125;

            //foreach (Crew c in MainWindow.mainVm.MyShip.Crews)
            //{
            //    if (c.Alive)
            //    {
            //        c.Hunger = c.Hunger >= hungerRatio ? c.Hunger -= hungerRatio : 0;
            //        if (c.Hunger <= 0) 
            //        {
            //            c.Health = c.Health >= hungerRatio ? c.Health -= hungerRatio : 0;
            //            if (c.Health <= 0)
            //            { 
            //                c.Alive = false;
            //                MainWindow.mainVm.MyShip.RemoveAssignment(c);
            //            }
            //        }
            //    }
            //}
        }
        #endregion Constructor

        #region Public Methods
        public void CreateMyShip()
        {
            var weapons = new ObservableCollection<Ship.Weapon>();

            for (int i = 0; i < 3; i++)
            {
                weapons.Add(new Ship.Weapon() { 
                    Name = $"Turret{i+1}",
                    Damage = 10, 
                    MaxHealth = 100,
                    CurrentHealth = 100,
                });
            }

            Ships.Add(new Ship()
            {
                engine = new Ship.Engine() { 
                    Health = 1000, 
                    MaxFuel = 1000, 
                    State = Ship.Engine.state.Off 
                },
                State = Ship.state.Docked,
                Location = _planet.Location,
                Weapons = weapons,
                Food = 1000
            });
        }
        public void AssignMyPilot(Crew crew)
        {
            //Ships.First().Pilot = crew.Name;
            //Ships.First().PilotSkill = crew.Skills.Piloting;
        }
        public void AssignMyCaptain(Crew crew)
        {
            //Ships.First().Captain = crew.Name;
        }
        public void loadPlanet(Planet planet)
        {
            _planet = planet;
        }
        public Ship GetMyShip()
        {
            return Ships[0];
        }
        #endregion Public Methods
    }
}
