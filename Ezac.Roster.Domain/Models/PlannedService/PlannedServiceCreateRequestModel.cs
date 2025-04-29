using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Models.PlannedService
{
    public class PlannedServiceCreateRequestModel
    {
        public int Id { get; set; }

        public int DayId { get; set; }

        public int PartOfDayId { get; set; }

        public int ServiceId { get; set; }

        public string MemberId { get; set; }

        public int Weight { get; set; }
        public int CalendarId { get; set; }
    }
}
