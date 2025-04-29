using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Infrastructure.Models.Calendar;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Interfaces
{
    public interface ICalendarService
    {
        public Task<ResultModel<IEnumerable<Calendar>>> GetAllAsync();
        public Task<ResultModel<Calendar>> GetByIdAsync(int id);
        public Task<ResultModel<Calendar>> CreateAsync(CalendarCreateRequestModel calendarCreateRequestModel);
        public Task<ResultModel<Calendar>> DeleteAsync(int id);
        public Task<ResultModel<Calendar>> UpdateAsync(CalendarUpdateRequestModel calendarUpdateRequestModel);
    }
}
