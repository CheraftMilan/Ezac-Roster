using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Models.Service
{
    public class ServiceCreateRequestModel
    {
        public string Name { get; set; }
        public IEnumerable<int> RequeredQualifications { get; set; }
        public int RequeredExperience { get; set; }
        public int Weight { get; set; }
        public int PartOfDayId { get; set; }
    }
}
