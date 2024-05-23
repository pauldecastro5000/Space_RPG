using Space_RPG.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Space_RPG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static PlanetManager Planet = new PlanetManager();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.mainVm;

            //for (int i = 0; i < 5; i++)
            //{
            //    Crew.AddRandomCrew();
            //}
            

            // Generate Planets
            for (int i = 0; i < 100; i++)
            {
                Planet.CreateRandomPlanet();
            }

            // Initialize the ship

        }
    }
}
