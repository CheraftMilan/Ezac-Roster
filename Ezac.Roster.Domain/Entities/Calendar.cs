using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Entities
{
    public class Calendar : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Day> Days { get; set; }
        public ICollection<Preference> Preferences { get; set; }
        public ICollection<Member> Members { get; set; }
        public ICollection<PlannedService> PlannedServices { get; set; }

    }
}
