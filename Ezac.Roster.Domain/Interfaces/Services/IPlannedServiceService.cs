using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Infrastructure.Models.PlannedService;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Interfaces
{
    public interface IPlannedServiceService
    {
        public Task<ResultModel<PlannedService>> CreateAsync(PlannedServiceCreateRequestModel plannedServiceCreateRequestModel);
        public Task<ResultModel<PlannedService>> DeleteAsync(int id);
        public Task<ResultModel<PlannedService>> GetByIdAsync(int id);
        public Task<ResultModel<IEnumerable<PlannedService>>> GetAllAsync();
        public Task<ResultModel<PlannedService>> UpdateAsync (PlannedServiceUpdateRequestModel plannedServiceUpdateRequestModel);
        public Task<ResultModel<IEnumerable<PlannedService>>> DeleteByCalendarIdAsync(int calendarId);
        public Task<ResultModel<IEnumerable<PlannedService>>> DeletePlannedServiceByCalendarId(int calendarId);
    }
}
