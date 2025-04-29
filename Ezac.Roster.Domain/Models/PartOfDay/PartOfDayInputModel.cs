using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Models.PartOfDay
{
    public class PartOfDayInputModel
    {
        public int PartOfDayId { get; set; }
        public bool IsAvailable { get; set; }
        public string ServicePreference { get; set; }
    }
}
