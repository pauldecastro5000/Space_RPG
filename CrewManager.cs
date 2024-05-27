using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Space_RPG
{
    public class CrewManager : ViewModelBase
    {
        #region Public Members
        public enum CrewNameEnum
        {
            Liam,
            Noah,
            Oliver,
            James,
            Elijah,
            William,
            Henry,
            Lucas,
            Benjamin,
            Theodore,
            Mateo,
            Levi,
            Sebastian,
            Daniel,
            Jack,
            Michael,
            Alexander,
            Owen,
            Asher,
            Samuel,
            Olivia,
            Emma,
            Charlotte,
            Amelia,
            Sophia,
            Mia,
            Isabella,
            Ava,
            Evelyn,
            Luna,
            Harper,
            Sofia,
            Camila,
            Eleanor,
            Elizabeth,
            Violet,
            Scarlett,
            Emily,
            Hazel,
            Lily,
            Gianna,
            Aurora,
            Penelope,
            Aria,
            Chloe,
            Ellie,
            Mila,
            Layla,
            Abigail,
            Ella,
            Eliana,
            Nova,
            Madison,
            Zoe,
            Ivy,
            Grace,
            Lucy,
            Emilia,
            Riley,
            Naomi,
            Victoria,
            Stella,
            Elena,
            Hannah,
            Valentina,
            Maya,
            Zoey,
            Delilah,
            Leah,
            Lainey,
            Lillian,
            Madelyn,
            Sophie,
            Natalie,
            Josephine,
            Alice,
            Ruby,
            Claire
        }
        #endregion Public Members

        #region Public Properties
        private ObservableCollection<Crew> _crews = new ObservableCollection<Crew>();
        public ObservableCollection<Crew> Crews
        {
            get { return _crews; }
            set { _crews = value; OnPropertyChanged(); }
        }
        #endregion Public Properties

        #region Private Variables
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        private double hungerRatio = 0.125;
        #endregion Private Variables

        #region Constructor
        public CrewManager()
        {
            MainWindow.UniverseTime.UniverseTickPerMin += UniverseTime_UniverseTickPerMin;
        }

        private void UniverseTime_UniverseTickPerMin(object sender, EventArgs e)
        {
            foreach (Crew c in MainWindow.Crew.Crews)
            {
                c.Hunger -= hungerRatio;
            }
        }
        #endregion Constructor

        #region Public Methods
        public void AddRandomCrew()
        {
            var name = String.Empty;
            do
            {
                name = ((CrewNameEnum)RandomNumber(0, 77)).ToString();
            } while (CrewNameExist(name));
            var aiming = RandomNumber(0, 100);
            var pilot = RandomNumber(0, 100);
            var engineRepair = RandomNumber(0, 100);
            var weaponsRepair = RandomNumber(0, 100);
            var job = (Crew.CrewJob)RandomNumber(1, 3);

            var newCrew = new Crew()
            {
                Name = name,
                Job = job,
                Hunger = 90,
                Cash = 500,
                Skills = new Crew.skills()
            };

            Crews.Add(newCrew);
        }
        public void AddCaptain()
        {
            var newCrew = new Crew()
            {
                Name = "Paul",
                Job = Crew.CrewJob.Captain,
                Hunger = 100,
                Cash = 100000,
                Skills = new Crew.skills()
                {
                    Piloting = 90,
                    Aiming = 90,
                    EngineRepair = 90,
                    WeaponsRepair = 90,
                },
            };
            Crews.Add(newCrew);
        }

        #endregion Public Methods

        #region Private Methods
        private bool CrewNameExist(string name)
        {
            if (Crews == null)
                return false;

            var crew = Crews.FirstOrDefault(x => x.Name == name);
            if (crew == null)
                return false;
            else
                return true;
        }
        private static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
        #endregion Private Methods
    }
}
