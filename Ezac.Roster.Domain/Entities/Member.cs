using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Entities
{
    public class Member 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OriginalId { get; set; }
        public string Email { get; set; }
        public double Scaling { get; set; }
        public double TotalWeight { get; set; }
        public ICollection<Qualification> Qualifications { get; set; }

        public ICollection<PlannedService> PlannedServices { get; set; }

        public Calendar Calendar { get; set; }
        public int CalendarId { get; set; }

        public ICollection<Preference> Preferences { get; set; }   
    }
}
