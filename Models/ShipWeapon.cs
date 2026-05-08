using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Models
{
    public enum WeaponType
    {
        Turret,
        LaserCannon,
        PlasmaCannon,
        ParticleBeam,
        Railgun
    }
    public class ShipWeapon
    {
        public WeaponType Type { get; set; }
        public double Health { get; set; }
        public double Damage { get; set; }

        // How about DPS??
        public ShipWeapon()
        {
            Health = 100.0;
            Damage = 10.0;
        }
    }
}
