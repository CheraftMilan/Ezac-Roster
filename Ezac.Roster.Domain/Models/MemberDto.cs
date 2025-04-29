using Ezac.Roster.Domain.Entities;

namespace Ezac.Roster.Web.Models
{
    public class MemberDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }
        public string UniqueLink { get; set; }
        public double Scaling { get; set; }
    }
}
