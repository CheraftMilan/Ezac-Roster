using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Entities
{
    public class Qualification : BaseEntity
    {
        public string Name { get; set; }
        public int Experience { get; set; }
        public Member Member { get; set; }
        public string MemberId { get; set; }
        public ICollection<Service> Services { get; set; }
    }
}
