using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Infrastructure.Models.Qualification;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Interfaces
{
    public interface IQualificationService
    {
        public Task<ResultModel<Qualification>> CreateAsync(QualificationCreateRequestModel qualificationCreateRequestModel);
        public Task<ResultModel<Qualification>> DeleteAsync(int id);
        public Task<ResultModel<Qualification>> GetByIdAsync(int id);
        public Task<ResultModel<IEnumerable<Qualification>>> GetAllAsync();
        public Task<ResultModel<Qualification>> UpdateAsync(QualificationUpdateRequestModel qualificationUpdateRequestModel);
    }
}
