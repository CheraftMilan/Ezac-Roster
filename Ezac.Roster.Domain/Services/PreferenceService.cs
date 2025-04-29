using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Domain.Interfaces.Services;
using Ezac.Roster.Domain.Models.Preference;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Services
{
    public class PreferenceService(IPreferenceRepository preferenceRepository) : IPreferenceService
    {
        private readonly IPreferenceRepository _preferenceRepository = preferenceRepository;

        public async Task<ResultModel<Preference>> CreateAsync(PreferenceCreateRequestModel preferenceCreateRequestModel)
        {
            Preference preference;
            if(preferenceCreateRequestModel.ServiceId == null)
            {
                preference = new()
                {
                    MemberId = preferenceCreateRequestModel.MemberId,
                    PartOfDayId = preferenceCreateRequestModel.PartOfDayId,
                    CalendarId = preferenceCreateRequestModel.CalendarId,
                };
            }
            else
            {
                preference = new()
                {
                    MemberId = preferenceCreateRequestModel.MemberId,
                    ServiceId = (int)preferenceCreateRequestModel.ServiceId,
                    PartOfDayId = preferenceCreateRequestModel.PartOfDayId,
                    CalendarId= preferenceCreateRequestModel.CalendarId,
                };
            }

            var result = await _preferenceRepository.AddAsync(preference);
            if (result)
            {
                return new ResultModel<Preference>
                {
                    IsSuccess = true,
                    Value = preference
                };
            }
            return new ResultModel<Preference>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het aanmaken van deze voorkeur is mislukt." }
            };
        }
        public async Task<ResultModel<IEnumerable<Preference>>> GetAllAsync()
        {
            var result = await _preferenceRepository.GetAllAsync();

            return new ResultModel<IEnumerable<Preference>>
            {
                IsSuccess = true,
                Value = result
            };
        }

        public async Task<ResultModel<Preference>> DeleteAllAsync(PreferenceDeleteRequestModel preferenceDeleteRequestModel)
        {

            var preferencesToDelete = preferenceDeleteRequestModel.PreferenceIds;

            if (preferencesToDelete == null)
            {
                return new ResultModel<Preference>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Er werden geen voorkeuren gevonden om te verwijderen." }
                };
            }
            foreach (var preferenceId in preferencesToDelete)
            {
                var toDelete = await _preferenceRepository.GetByIdAsync(preferenceId);

                if (toDelete == null)
                {
                    return new ResultModel<Preference>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Er werden geen voorkeuren gevonden om te verwijderen." }
                    };
                }
                
               var deleted = await _preferenceRepository.DeleteAsync(toDelete);

                if (!deleted)
                {
                    return new ResultModel<Preference>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Het verwijderen van enkele voorkeuren is mislukt." }
                    };
                }
            }
            return new ResultModel<Preference>
            {
                IsSuccess = true
            };
            
        }

    }
}
