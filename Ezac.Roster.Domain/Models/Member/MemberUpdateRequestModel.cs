using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Models.Member
{
    public class MemberUpdateRequestModel
    {
        public string MemberId { get; set; }
        public double Scaling { get; set; }
        public double TotalWeight { get; set; }
    }
}
