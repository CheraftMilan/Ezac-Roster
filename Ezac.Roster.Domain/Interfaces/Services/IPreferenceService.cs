using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Models.Preference;
using Ezac.Roster.Infrastructure.Models.PlannedService;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Interfaces.Services
{
    public interface IPreferenceService
    {
        public Task<ResultModel<Preference>> CreateAsync(PreferenceCreateRequestModel preferenceCreateRequestModel);
        Task<ResultModel<Preference>> DeleteAllAsync(PreferenceDeleteRequestModel preferenceDeleteRequestModel);
        Task<ResultModel<IEnumerable<Preference>>> GetAllAsync();
    }
}
