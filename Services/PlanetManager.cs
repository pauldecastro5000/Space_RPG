using System;
using Space_RPG.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG.Services
{
    public class PlanetManager : ViewModelBase
    {
        #region Public Properties
        //private ObservableCollection<Planet> _planets = new ObservableCollection<Planet>();
        //public ObservableCollection<Planet> Planets
        //{
        //    get { return _planets; }
        //    set { _planets = value; OnPropertyChanged(); }
        //}
        #endregion Public Properties

        #region Private Variables

        #endregion Private Variables

        #region Public Methods
        public void CreateRandomPlanet()
        {
            //var distBetweenPlanets = 20;
            //bool distOK = true;
            //var loc = new Point(0, 0);

            //do
            //{
            //    loc = new Point(MainWindow.Util.RandomNumber(1, 1000), MainWindow.Util.RandomNumber(1, 1000));
            //    distOK = true;
            //    foreach (var planet in Planets)
            //    {
            //        var dist = MainWindow.Util.Distance2Points(loc, planet.Location);
            //        if (dist < distBetweenPlanets)
            //        { 
            //            distOK = false;
            //            break;
            //        }
            //    }
            //} while (!distOK);

            //var newPlanet = new Planet();
            //newPlanet.Location = loc;
            //Planets.Add(newPlanet);
        }
        public Planet CreateColonizedPlanet(ObservableCollection<Planet> Planets)
        {
            var distBetweenPlanets = 20;
            bool distOK = true;
            var loc = new Point(0, 0);

            do
            {
                loc = new Point(MainWindow.Util.RandomNumber(1, 1000), MainWindow.Util.RandomNumber(1, 1000));
                distOK = true;
                foreach (var planet in Planets)
                {
                    var dist = MainWindow.Util.Distance2Points(loc, planet.Location);
                    if (dist < distBetweenPlanets)
                    {
                        distOK = false;
                        break;
                    }
                }
            } while (!distOK);

            var newPlanet = new Planet();
            newPlanet.Type = Planet.PlanetType.Colonized;
            newPlanet.Location = loc;
            newPlanet.Id = new Guid();
            //Planets.Add(newPlanet);
            return newPlanet;
        }
        #endregion Public Methods
    }
}
