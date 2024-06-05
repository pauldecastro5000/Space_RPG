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
        private bool _isInTransition = false;
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
                    if (_newImgPath != MainWindow.mainVm.CockpitImage && !_isInTransition)
                    {
                        ChangeImage(_newImgPath);
                    }
                    break;

                case Ship.state.LiftingOff:
                    _newImgPath = "../Resources/EMPTY.jpg";
                    if (_newImgPath != MainWindow.mainVm.CockpitImage && !_isInTransition)
                    {
                        ChangeImage(_newImgPath);
                    }
                    break;

                case Ship.state.Hovering:
                    _newImgPath = "../Resources/SPACE_EARTH_STATIC.jpg";
                    if (_newImgPath != MainWindow.mainVm.CockpitImage && !_isInTransition)
                    {
                        ChangeImage(_newImgPath);
                    }
                    break;

            }
        }

        private async void ChangeImage(string imgPath)
        {
            if (_isInTransition)
                return;

            _isInTransition = true;

            await Task.Run(() =>
            {
                // Fade out
                for (int i = 100; i > 0; i--)
                {
                    MainWindow.mainVm.MainImgOpacity = Convert.ToDouble(i) / 100;
                    System.Threading.Thread.Sleep(10);
                }
                MainWindow.mainVm.CockpitImage = imgPath;
                // Fade in
                for (int i = 0; i < 100; i++)
                {
                    MainWindow.mainVm.MainImgOpacity = Convert.ToDouble(i) / 100;
                    System.Threading.Thread.Sleep(10);
                }
                _isInTransition = false;
            });
        }
    }
}
