using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Entities
{
    public class PartOfDay : BaseEntity
    {

       public string Name { get; set; }

       public ICollection<Service> Services { get; set; }

       public string StartTime { get; set; }

       public string EndTime { get; set; }

        public Day Day { get; set; }

        public int DayId { get; set; }

        public bool IsAvailable { get; set; }

        public int?  ServicePreferenceId { get; set; }

        public ICollection<Preference> Preferences { get; set; }


    }
}
