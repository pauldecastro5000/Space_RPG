using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG
{
    public class Crew
    {
        public enum CrewJob
        {
            None,
            Captain,
            Pilot,
            Weapons,
            Engineer
        }

        public struct Skills
        {
            public double Pilot { get; set; }
            public double Aiming { get; set; }
            public double EngineRepair { get; set; }
            public double WeaponsRepair { get; set; }
        }

        #region Public Properties
        public string Name { get; set; }
        public CrewJob Job { get; set; } = CrewJob.None;
        public int Hunger { get; set; } = 100;
        public Skills skills { get; set; }
        public int Money { get; set; }
        #endregion Public Properties

        #region Public Methods

        public void DecreaseHunger()
        {
            Hunger--;
        }

        #endregion Public Methods

    }


}
