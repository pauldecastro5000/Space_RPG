using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Space_RPG
{
    public class CrewManager
    {
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

        public List<Crew> Crews { get; set; } = new List<Crew>();
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

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
                Hunger = 100,
                Money = 500,
                skills = new Crew.Skills()
                {
                    Pilot = pilot,
                    EngineRepair = engineRepair,
                    WeaponsRepair = weaponsRepair,
                    Aiming = aiming,
                },
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
                Money = 100000,
                skills = new Crew.Skills()
                {
                    Pilot = 90,
                    EngineRepair = 90,
                    WeaponsRepair = 90,
                    Aiming = 90,
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
