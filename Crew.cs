using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG
{
    public class Crew : ViewModelBase
    {
        public enum CrewJob
        {
            None,
            Captain,
            Pilot,
            Weapons,
            Engineer
        }

        public class skills : ViewModelBase
        {
            private double _piloting = 0.0;
            public double Piloting
            {
                get { return _piloting; }
                set { _piloting = value; OnPropertyChanged(); }
            }
            private double _aiming = 0.0;
            public double Aiming
            {
                get { return _aiming; }
                set { _aiming = value; OnPropertyChanged(); }
            }
            private double _engineRepair = 0.0;
            public double EngineRepair
            {
                get { return _engineRepair; }
                set { _engineRepair = value; OnPropertyChanged(); }
            }
            private double _weaponsRepair = 0.0;
            public double WeaponsRepair
            {
                get { return _weaponsRepair; }
                set { _weaponsRepair = value; OnPropertyChanged(); }
            }
        }

        #region Public Properties
        private string _name = "unknown";
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }
        private CrewJob _job = CrewJob.None;
        public CrewJob Job
        {
            get { return _job; }
            set { _job = value; OnPropertyChanged(); }
        }
        private double _hunger = 100.0;
        public double Hunger
        {
            get { return _hunger; }
            set { _hunger = value; OnPropertyChanged(); }
        }
        private skills _skills;
        public skills Skills
        {
            get { return _skills; }
            set { _skills = value; OnPropertyChanged(); }
        }
        private int _cash = 0;
        public int Cash
        {
            get { return _cash; }
            set { _cash = value; OnPropertyChanged(); }
        }
        #endregion Public Properties

        #region Public Methods

        public void DecreaseHunger()
        {
            Hunger--;
        }
        public Crew GetCrew()
        {
            return this;
        }
        #endregion Public Methods

    }


}
