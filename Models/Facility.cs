using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Models
{
  

    public class Facility
    {
       

        public FacilityType Type { get; set; }

        public ObservableCollection<Guid> CrewIds { get; set; }
     = new ObservableCollection<Guid>();

        public Facility()
        {

        }

    }
}
