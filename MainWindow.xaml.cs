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
        public static CommandManager Command = new CommandManager();
        public static TaskManager TaskMgr = new TaskManager();
        public static MainVM mainVm = new MainVM();

        private string _prevCommand;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = mainVm;

            // Generate Planet
            Planet.CreateColonizedPlanet();
            for (int i = 0; i < 100; i++)
            {
                Planet.CreateRandomPlanet();
            }

            Ship.loadPlanet(Planet.Planets.First());

            Ship.CreateMyShip();

            Crew.AddCaptain();

            // Initialize my ship
            mainVm.MyShip = Ship.Ships.First();              // load first ship as my ship
            mainVm.MyShip.AddCaptain();                      // add caption to my ship
            
            // Initialize planets
            mainVm.Planets = Planet.Planets;                 // assign planets
            mainVm.CurrentPlanet = Planet.Planets.First();   // load first planet as my current planet

            mainVm.PlanetType = Enum.GetName(typeof(Planet.Type), 
                Planet.Planets.First().type);
            mainVm.EngineState = Enum.GetName(typeof(Ship.Engine.state),
                mainVm.MyShip.engine.State);
            
            UniverseTime.TimeStart();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            mainVm.MyShip.Weapons[1].CurrentHealth -= 1;
            mainVm.MyShip.Food -= 1;
            mainVm.MyShip.engine.Health -= 1;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (!Command.ProcessCommand(mainVm.Command, out string err))
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
