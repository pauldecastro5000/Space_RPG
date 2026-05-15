using Space_RPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Helpers
{
    public static class CrewHelper
    {
        public static InteriorObjectType GetWorkstationType(Job job)
        {
            switch (job)
            {
                case Job.Pilot:
                    return InteriorObjectType.Cockpit;

                case Job.TurretGunner:
                    return InteriorObjectType.WeaponsConsole;

                case Job.Engineer:
                    return InteriorObjectType.Workbench;

                case Job.Medic:
                    return InteriorObjectType.MedicalConsole;

                default:
                    return InteriorObjectType.None;
            }
        }
    }
}
