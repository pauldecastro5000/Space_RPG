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
        public static UniverseTime UniverseTime = new UniverseTime();
        public static PlanetManager Planet = new PlanetManager();
        public static CrewManager Crew = new CrewManager();
        public static ShipManager Ship = new ShipManager();
        public static StorageManager Storage = new StorageManager();
        public static Utilities Util = new Utilities();
        

        public static MainVM mainVm = new MainVM();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = mainVm;

            // Generate Planets
            for (int i = 0; i < 100; i++)
            {
                Planet.CreateRandomPlanet();
            }

            Ship.loadPlanet(Planet.Planets[0]);
            Ship.CreateMyShip();

            Crew.AddCaptain();
            // generate crews
            for (int i = 0; i < 3; i++)
            {
                Crew.AddRandomCrew();
            }

            // Initialize my ship
            mainVm.MyShip = Ship.Ships.First();

            mainVm.CrewManager = Crew;
            mainVm.MyCaptain = mainVm.CrewManager.Crews.First();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            ////Crew.Crews[0].Hunger -= 1;
            //Ship.MyShip.Engine -= 10;
            //Crew.AddRandomCrew();
            //Ship.MyShip.Weapons[0].Health -= 1;
            mainVm.MyShip.Weapons[1].Health -= 1;
            mainVm.MyShip.Food -= 1;
            mainVm.MyShip.Engine -= 1;
        }
    }
}
