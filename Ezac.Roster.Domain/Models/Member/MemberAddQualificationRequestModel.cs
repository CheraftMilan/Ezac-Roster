using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Models.Member
{
    public class MemberAddQualificationRequestModel
    {
        public string MemberId { get; set; }
        public int InstructorLicense { get; set; }
        public int LierLicense { get; set; }
        public int StartingOfficerLicense { get; set; }
        public int BarLicense { get; set; }
    }
}
