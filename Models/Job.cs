using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Models
{
    public enum Job
    {
        None,
        All,
        Captain,
        Pilot,    // Navigate Ship
        TurretGunners,  // 
        Engineer,
        Expolorer,
        ExpeditionGuard,
        Miners,
        Driver,
        Medic
    }

    public enum JobStatus
    { 
        Pending = 0, 
        Reserved = 1, 
        InProgress = 2, 
        Completed = 3, 
        Cancelled = 4 
    }

    public enum Activity
    {
        None = 0,
        Working,
        Walking,
        Eating
    }
}
