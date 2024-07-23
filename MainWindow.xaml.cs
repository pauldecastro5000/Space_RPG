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
        public static PlanetManager PlanetMgr = new PlanetManager();
        public static CrewManager CrewMgr = new CrewManager();
        public static ShipManager ShipMgr = new ShipManager();
        public static StorageManager StorageMgr = new StorageManager();
        public static Utilities Util = new Utilities();
        public static CommandManager CommandMgr = new CommandManager();
        public static TaskManager TaskMgr = new TaskManager();
        public static ImageManager ImgMgr = new ImageManager();
        public static SaveLoadManager saveLoadMgr = new SaveLoadManager();

        public static MainVM mainVm = new MainVM();

        private string _prevCommand;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = mainVm;

            // Generate Planets
            PlanetMgr.CreateColonizedPlanet();
            for (int i = 0; i < 100; i++)
            {
                PlanetMgr.CreateRandomPlanet();
            }

            ShipMgr.loadPlanet(PlanetMgr.Planets.First());

            ShipMgr.CreateMyShip();

            //CrewMgr.AddCaptain();

            // Initialize my ship
            mainVm.MyShip = ShipMgr.Ships.First();              // load first ship as my ship
            mainVm.MyShip.AddCaptain();                      // add caption to my ship

            // Initialize planets
            mainVm.Planets = PlanetMgr.Planets;                 // assign planets
            mainVm.CurrentPlanet = PlanetMgr.Planets.First();   // load first planet as my current planet

            mainVm.PlanetType = Enum.GetName(typeof(Planet.Type),
                PlanetMgr.Planets.First().type);
            mainVm.EngineState = Enum.GetName(typeof(Ship.Engine.state),
                mainVm.MyShip.engine.State);

            mainVm.dateTime = new DateTime(2024, 1, 1, 12, 0, 0);

            UniverseTime.TimeStart();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //mainVm.MyShip.Weapons[1].CurrentHealth -= 1;
            //mainVm.MyShip.Food -= 1;
            //mainVm.MyShip.engine.Health -= 1;
            //mainVm.MyShip.State = Space_RPG.ShipMgr.state.Hovering;
            mainVm.MyShip.AddCrewToFacility(Space_RPG.Ship.FacilityType.Cargo);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (!CommandMgr.ProcessCommand(mainVm.Command, out string err))
                {
                    Log($"Command: [{mainVm.Command}] returns error.");
                    Log($"ERROR: {err}");
                }
                _prevCommand = mainVm.Command;
                mainVm.Command = "";
            }
        }

        private void Log(string message)
        {
            MainWindow.mainVm.Log.Add(message);
        }

        private Boolean AutoScroll = true;

        private void scrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (mainVm.Log.Count > 20)
            {
                mainVm.Log.RemoveAt(0);
            }

            // User scroll event : set or unset auto-scroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set auto-scroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and auto-scroll mode set
                // Autoscroll
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
            }
        }
    }
}
