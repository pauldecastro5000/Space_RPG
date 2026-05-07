using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Models
{
    public class Rover
    {
        public List<Guid> Driver { get; set; }
        public List<Guid> Passenger { get; set; }
    }
}
