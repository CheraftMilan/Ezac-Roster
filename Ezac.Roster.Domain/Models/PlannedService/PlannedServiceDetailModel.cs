using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Models.PlannedService
{
    public class PlannedServiceDetailModel
    {
        public string MemberName { get; set; }
        public string ServiceName { get; set; }
        public DateTime CalendarDate { get; set; }
    }
}
