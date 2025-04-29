using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Infrastructure.Models.PartOfDay;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Interfaces
{
    public interface IPartOfDayService
    {
        public Task<ResultModel<PartOfDay>> CreateAsync(PartOfDayCreateRequestModel partOfDayCreateRequestModel);
        public Task<ResultModel<PartOfDay>> DeleteAsync(int id);
        public Task<ResultModel<PartOfDay>> GetByIdAsync(int id);
        public Task<ResultModel<IEnumerable<PartOfDay>>> GetAllAsync();
        public Task<ResultModel<PartOfDay>> UpdateAsync(PartOfDayUpdateRequestModel partOfDayUpdateRequestModel);
    }
}
