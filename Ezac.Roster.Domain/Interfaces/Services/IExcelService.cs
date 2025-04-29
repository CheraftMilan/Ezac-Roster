using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Models.Member;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Interfaces.Services
{
    public interface IExcelService
    {
        Task<string> ExportPlannedServicesAsync(int calendarId);
        Task<ResultModel<IEnumerable<MemberAndQualificationRequestModel>>> ReadFileAsync(Stream stream);
        Task<bool> SaveMembersAndQualificationsAsync(IEnumerable<MemberAndQualificationRequestModel> members);
    }
}
