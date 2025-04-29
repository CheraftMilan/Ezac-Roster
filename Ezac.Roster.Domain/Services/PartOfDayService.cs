using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.PartOfDay;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Services
{
    public class PartOfDayService(IPartOfDayRepository partOfDayRepository) : IPartOfDayService
    {
        private readonly IPartOfDayRepository _partOfDayRepository = partOfDayRepository;

        public async Task<ResultModel<PartOfDay>> CreateAsync(PartOfDayCreateRequestModel partOfDayCreateRequestModel)
        {
            var partOfDay = new PartOfDay
            {
                DayId = partOfDayCreateRequestModel.DayId,
                Name = partOfDayCreateRequestModel.Name,
                StartTime = partOfDayCreateRequestModel.StartTime,
                EndTime = partOfDayCreateRequestModel.EndTime
            };
            var result = await _partOfDayRepository.AddAsync(partOfDay);
            if (result)
            {
                return new ResultModel<PartOfDay>
                {
                    IsSuccess = true,
                    Value = partOfDay
                };
            }
            return new ResultModel<PartOfDay>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het aanmaken van deze dagperiode is mislukt." }
            };
        }

        public async Task<ResultModel<PartOfDay>> DeleteAsync(int id)
        {
            var partOfDay = await _partOfDayRepository.GetByIdAsync(id);
            if (partOfDay == null)
            {
                return new ResultModel<PartOfDay>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze dagperiode werd niet gevonden." }
                };
            }
            if (await _partOfDayRepository.DeleteAsync(partOfDay))
            {
                return new ResultModel<PartOfDay>
                {
                    IsSuccess = true,
                };
            }
            return new ResultModel<PartOfDay>
            {
                IsSuccess = false,
                Errors = new List<string> { "Deze dagperiode verwijderen is mislukt." }
            };
        }

        public async Task<ResultModel<IEnumerable<PartOfDay>>> GetAllAsync()
        {
            var partOfDays = await _partOfDayRepository.GetAllAsync();
            if (partOfDays.Any())
            {
                return new ResultModel<IEnumerable<PartOfDay>>
                {
                    IsSuccess = true,
                    Value = partOfDays
                };
            }
            return new ResultModel<IEnumerable<PartOfDay>>
            {
                IsSuccess = false,
                Errors = new List<string> { "Er werden geen dagperiodes gevonden." }
            };
        }

        public async Task<ResultModel<PartOfDay>> GetByIdAsync(int id)
        {
            var partOfDay = await _partOfDayRepository.GetByIdAsync(id);
            if (partOfDay == null)
            {
                return new ResultModel<PartOfDay>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze dagperiode werd niet gevonden." }
                };
            }
            return new ResultModel<PartOfDay>
            {
                IsSuccess = true,
                Value = partOfDay
            };
        }

        public async Task<ResultModel<PartOfDay>> UpdateAsync(PartOfDayUpdateRequestModel partOfDayUpdateRequestModel)
        {
            var partOfDay = await _partOfDayRepository.GetByIdAsync(partOfDayUpdateRequestModel.Id);
            if(partOfDay == null)
            {
                return new ResultModel<PartOfDay>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze dagperiode werd niet gevonden." }
                };
            }
            partOfDay.Name = partOfDayUpdateRequestModel.Name;
            partOfDay.StartTime = partOfDayUpdateRequestModel.StartTime;
            partOfDay.EndTime = partOfDayUpdateRequestModel.EndTime;
            partOfDay.IsAvailable = partOfDayUpdateRequestModel.IsAvailable;
            partOfDay.ServicePreferenceId = partOfDayUpdateRequestModel.ServicePreferenceId;
            if (await _partOfDayRepository.UpdateAsync(partOfDay))
            {
                return new ResultModel<PartOfDay>
                {
                    IsSuccess = true,
                    Value = partOfDay
                };
            }
            return new ResultModel<PartOfDay>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het updaten van deze dagperiode is mislukt." }
            };
        }
    }
}
