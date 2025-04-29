using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Models.Qualification
{
    public class QualificationCreateRequestModel
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }
        public int Experience { get; set; }
        public IEnumerable<int> ServiceIds { get; set; }
        public string memberId { get; set; }
    }
}
