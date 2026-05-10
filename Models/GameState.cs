using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Models
{
    public class GameState
    {
        public bool IsInShipBattleMode { get; set; }
        public bool IsInRoverBattleMode { get; set; }
        public int TickCount { get; set; }
        public int TimeOfDay { get; set; }
        public ObservableCollection<Crew> Crews { get; set; }
       public ObservableCollection<Planet> Planets { get; set; }
        public ObservableCollection<Rover> Rovers { get; set; }
        public Ship MyShip { get; set; }
        public int MinutesPerTick { get; set; }
        public GameState()
        { 
            Crews = new ObservableCollection<Crew>();
            MyShip = new Ship();
            Rovers = new ObservableCollection<Rover>();
            Planets = new ObservableCollection<Planet>();
        }

    }
}
