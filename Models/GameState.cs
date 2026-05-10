using System;
using System.Collections.Generic;
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
        public List<Crew> Crews { get; set; }
        public Rover Rover { get; set; }
        public int MinutesPerTick { get; set; }
        public GameState()
        { 
            Crews = new List<Crew>();
        }

    }
}
