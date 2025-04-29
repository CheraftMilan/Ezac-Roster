using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Models.Service;
using Ezac.Roster.Infrastructure.Models.Result;
using Ezac.Roster.Infrastructure.Models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Interfaces
{
    public interface IServiceService
    {
        public Task<ResultModel<Service>> CreateAsync(ServiceCreateRequestModel serviceCreateRequestModel);
        public Task<ResultModel<Service>> DeleteAsync(int id);
        public Task<ResultModel<Service>> GetByIdAsync(int id);
        public Task<ResultModel<IEnumerable<Service>>> GetAllAsync();
        public Task<ResultModel<Service>> UpdateAsync(ServiceUpdateRequestModel serviceUpdateRequestModel);

    }
}
