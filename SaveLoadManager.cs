using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Space_RPG.ViewModel;

namespace Space_RPG
{
    public class SaveLoadManager
    {
        private string saveFilename = "SpaceGameSave.txt";
        public bool Save(out string err)
        {
            err = "";
            try
            {
                var json = JsonConvert.SerializeObject(MainWindow.mainVm, Formatting.Indented);
                File.WriteAllText(saveFilename, json);
            } catch (Exception ex)
            {
                err = "Saving failed: " + ex.Message;
                return false;
            }
            return true;
        }
        public bool Load(out string err)
        {
            err = "";
            if (!File.Exists(saveFilename))
            {
                err = $"File does not exist: {saveFilename}";
                return false;
            }
            using (StreamReader r = new StreamReader(saveFilename))
            {
                string json = r.ReadToEnd();
                MainVM vm = JsonConvert.DeserializeObject<MainVM>(json);
                MainWindow.mainVm.MyShip = vm.MyShip;
                MainWindow.mainVm.CrewManager = vm.CrewManager;
                MainWindow.mainVm.Command = vm.Command;
                MainWindow.mainVm.Log = vm.Log;
                MainWindow.mainVm.Planets = vm.Planets;
                MainWindow.mainVm.CurrentPlanet = vm.CurrentPlanet;
                MainWindow.mainVm.PlanetType = vm.PlanetType;
                MainWindow.mainVm.EngineState = vm.EngineState;
                MainWindow.mainVm.CockpitImage = vm.CockpitImage;
                MainWindow.mainVm.dateTime = vm.dateTime;
            }
            return true;
        }
    }
}
