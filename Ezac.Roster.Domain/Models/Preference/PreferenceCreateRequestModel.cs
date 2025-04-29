using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Models.Preference
{
    public class PreferenceCreateRequestModel
    {
        public int Id { get; set; }

        public string MemberId { get; set; }

        public int? ServiceId { get; set; }

        public int PartOfDayId { get; set; }
        public int CalendarId { get; set; }
    }
}
