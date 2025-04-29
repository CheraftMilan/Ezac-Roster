using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Infrastructure.Models.Qualification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Models.Member
{
    public class MemberAddSingleMemberRequestModel
    {
        public string Id { get; set; }

        public string OriginalId { get; set; }
        public string Name { get; set; }
        public int CalendarId { get; set; }
        public string Email { get; set; }
        public List<QualificationCreateRequestModel> Qualifications { get; set; }
        public double Scaling { get; set; }
    }
}
