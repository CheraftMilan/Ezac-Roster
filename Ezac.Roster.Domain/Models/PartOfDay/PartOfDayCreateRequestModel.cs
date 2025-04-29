using Ezac.Roster.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Models.PartOfDay
{
    public class PartOfDayCreateRequestModel
    {
        public int DayId { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }

        public string EndTime { get; set; }
    }
}
