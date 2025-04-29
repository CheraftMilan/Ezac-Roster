using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Entities
{
    public class PlannedService : BaseEntity
    {

        public int PartOfDayId { get; set;}

        public Service Service { get; set; }

        public int ServiceId { get; set; }

        public Member Member { get; set; }

        public string MemberId { get; set; }

        public int? Weight { get; set; }
        public Calendar Calendar { get; set; }
        public int CalendarId { get; set; }


    }
}
