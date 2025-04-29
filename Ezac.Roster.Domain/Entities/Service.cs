using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Entities
{
    public class Service : BaseEntity
    {
        public string Name { get; set; }
        public int Weight { get; set; }
        public int RequeredExperience { get; set; }
        public ICollection<Qualification> RequiredQualifications { get; set; }
        public PartOfDay PartOfDay { get; set; }
        public int? PartOfDayId { get; set; }
        public ICollection<Preference> Preferences { get; set; }

    }
}
