using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG
{
    public class PlanetManager
    {
        #region Private Variables
        private List<Planet> planets = new List<Planet>();
        #endregion Private Variables

        #region Public Methods
        public void CreateRandomPlanet()
        {
            var distBetweenPlanets = 20;
            bool distOK = true;
            var loc = new Point(0, 0);

            do
            {
                loc = new Point(MainWindow.Util.RandomNumber(1, 1000), MainWindow.Util.RandomNumber(1, 1000));
                distOK = true;
                foreach (var planet in planets)
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
            newPlanet.Location = loc;
            planets.Add(newPlanet);
        }
        #endregion Public Methods
    }
}
