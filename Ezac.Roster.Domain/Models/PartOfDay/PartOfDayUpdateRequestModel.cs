using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Models.PartOfDay
{
    public class PartOfDayUpdateRequestModel : PartOfDayCreateRequestModel
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }
        public int? ServicePreferenceId { get; set; }
    }
}
