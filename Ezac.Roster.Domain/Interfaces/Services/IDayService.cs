using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Infrastructure.Models.Calendar;
using Ezac.Roster.Infrastructure.Models.Day;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Interfaces
{
    public interface IDayService
    {
        public Task<ResultModel<Day>> CreateAsync(DayCreateRequestModel dayCreateRequestModel);
        public Task<ResultModel<Day>> DeleteAsync(int id);
        public Task<ResultModel<Day>> GetByIdAsync(int id);
        public Task<ResultModel<IEnumerable<Day>>> GetAllAsync();
        public Task<ResultModel<Day>> UpdateAsync(DayUpdateRequestModel dayUpdateRequestModel);
    }
}
