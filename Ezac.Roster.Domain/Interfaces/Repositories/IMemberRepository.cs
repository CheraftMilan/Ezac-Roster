using Ezac.Roster.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Interfaces.Repositories
{
    public interface IMemberRepository : IBaseRepository<Member>
    {
        Task<Member> GetByIdAsync(string id);
        Task<bool> GiveQualificationToMember(string memberId, int qualificationId);
        Task<bool> DeleteAllAsync();
        Task<bool> RemoveMemberByCalendarId(int calendarId);
    }
}
