using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG
{
    public class ImageManager
    {
        private string _newImgPath = "";
        public ImageManager()
        {
            MainWindow.UniverseTime.UniverseTickPerMin += UniverseTime_UniverseTickPerMin;
        }

        private void UniverseTime_UniverseTickPerMin(object sender, EventArgs e)
        {
            switch (MainWindow.mainVm.MyShip.State)
            {
                case Ship.state.Docked:
                    _newImgPath = "../Resources/DOCKED.jpg";
                    if (_newImgPath != MainWindow.mainVm.CockpitImage)
                    {
                        MainWindow.mainVm.CockpitImage = _newImgPath;
                    }
                    break;

                case Ship.state.Hovering:
                    _newImgPath = "../Resources/SPACE_EARTH_STATIC.jpg";
                    if (_newImgPath != MainWindow.mainVm.CockpitImage)
                    {
                        MainWindow.mainVm.CockpitImage = _newImgPath;
                    }
                    break;

            }
        }
    }
}
