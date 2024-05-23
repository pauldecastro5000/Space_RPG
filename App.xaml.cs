﻿using Space_RPG.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static CrewManager Crew = new CrewManager();
        
        public static ShipManager Ship = new ShipManager();
        public static StorageManager Storage = new StorageManager();
        public static Utilities Util = new Utilities();

        public static MainVM mainVm = new MainVM();
    }
}
