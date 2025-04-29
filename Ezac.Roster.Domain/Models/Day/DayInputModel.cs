using Ezac.Roster.Domain.Models.PartOfDay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Models.Day
{
    public class DayInputModel
    {
        public int DayId { get; set; }
        public List<PartOfDayInputModel> PartOfDays { get; set; }
    }
}
