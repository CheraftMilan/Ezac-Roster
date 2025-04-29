using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Models.Preference
{
    public class PreferenceDeleteRequestModel
    {
        public string MemberId { get; set; }

        public IEnumerable<int> PreferenceIds { get; set; }
    }
}
