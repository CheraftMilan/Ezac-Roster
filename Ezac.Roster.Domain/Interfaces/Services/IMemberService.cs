using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Models.Member;
using Ezac.Roster.Infrastructure.Models.Member;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Interfaces
{
    public interface IMemberService
    {
        public Task<ResultModel<Member>> CreateAsync(MemberCreateRequestModel memberCreateRequestModel);
        public Task<ResultModel<Member>> AddQualification(MemberAddQualificationRequestModel memberAddQualificationRequestModel);
        public Task<ResultModel<Member>> DeleteAsync(string id);
        public Task<ResultModel<Member>> GetByIdAsync(string id);
        public Task<ResultModel<IEnumerable<Member>>> GetAllAsync();
        public Task<ResultModel<IEnumerable<Member>>> DeleteAllAsync();
        public Task<ResultModel<IEnumerable<Member>>> RemoveMemberByCalendarId(int calendarId);
        public Task<ResultModel<Member>> UpdateAsync(MemberUpdateRequestModel memberUpdateRequestModels);
    }
}
