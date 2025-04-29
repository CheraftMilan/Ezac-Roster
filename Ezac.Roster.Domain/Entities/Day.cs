using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Entities
{
    public class Day : BaseEntity
    {
        public string Description { get; set; }

        public DateTime Date { get; set; }

        public ICollection<PartOfDay> PartOfDays { get; set; }
        public Calendar Calendar { get; set; }
        public int CalendarId { get; set; }

    }
}
