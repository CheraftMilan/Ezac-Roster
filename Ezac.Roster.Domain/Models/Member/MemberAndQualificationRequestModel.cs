using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Models.Member
{
    public class MemberAndQualificationRequestModel
    {
        public string Name { get; set; }
        public string OriginalId { get; set; }
        public string Email { get; set; }
        public int InstructorLicense { get; set; }
        public int LierLicense { get; set; }
        public int StartingOfficerLicense { get; set; }
        public int BarLicense { get; set; }
        public double Scaling { get; set; }
        public int CalendarId { get; set; }
    }
}
