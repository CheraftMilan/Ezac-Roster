using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.Day;
using Ezac.Roster.Infrastructure.Models.Result;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Services
{
    public class DayService(IDayRepository dayRepository) : IDayService
    {
        private readonly IDayRepository _dayRepository = dayRepository;

        public async Task<ResultModel<Day>> CreateAsync(DayCreateRequestModel dayCreateRequestModel)
        {
            try
            {
                var day = new Day
                {
                    Description = dayCreateRequestModel.Description,
                    Date = dayCreateRequestModel.Date,
                    CalendarId = dayCreateRequestModel.CalendarId
                };

                var result = await _dayRepository.AddAsync(day);

                return new ResultModel<Day>
                {
                    IsSuccess = true,
                    Value = day,
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<Day>
                {
                    IsSuccess = false,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResultModel<Day>> DeleteAsync(int id)
        {
            try
            {
                var day = await _dayRepository.GetByIdAsync(id);

                if (day == null)
                {
                    return new ResultModel<Day>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Deze dag werd niet gevonden." },
                    };
                }

                var deleteResult = await _dayRepository.DeleteAsync(day);
                if (!deleteResult)
                {
                    return new ResultModel<Day>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Het verwijderen van deze dag is mislukt." },
                    };
                }

                return new ResultModel<Day>
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<Day>
                {
                    IsSuccess = false,
                    Errors = new List<string> { ex.Message },
                };
            }
        }


        public async Task<ResultModel<IEnumerable<Day>>> GetAllAsync()
        {
            try
            {
                var days = await _dayRepository.GetAllAsync();

                if (!days.Any())
                {
                    return new ResultModel<IEnumerable<Day>>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Er werden geen dagen gevonden." }
                    };
                }

                return new ResultModel<IEnumerable<Day>>
                {
                    IsSuccess = true,
                    Value = days
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<IEnumerable<Day>>
                {
                    IsSuccess = false,
                    Errors = new List<string> { ex.Message },
                };
            }
        }


        public async Task<ResultModel<Day>> GetByIdAsync(int id)
        {
            try
            {
                var day = await _dayRepository.GetByIdAsync(id);

                if (day == null)
                {
                    return new ResultModel<Day>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Deze dag werd niet gevonden." }
                    };
                }

                return new ResultModel<Day>
                {
                    IsSuccess = true,
                    Value = day
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<Day>
                {
                    IsSuccess = false,
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResultModel<Day>> UpdateAsync(DayUpdateRequestModel dayUpdateRequestModel)
        {
            try
            {
                var day = await _dayRepository.GetByIdAsync(dayUpdateRequestModel.Id);

                if (day == null)
                {
                    return new ResultModel<Day>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Deze dag werd niet gevonden." }
                    };
                }

                day.Description = dayUpdateRequestModel.Description;
                day.Date = dayUpdateRequestModel.Date;
                day.CalendarId = dayUpdateRequestModel.CalendarId;

                var updateResult = await _dayRepository.UpdateAsync(day);
                if (!updateResult)
                {
                    return new ResultModel<Day>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Het updaten van deze dag is mislukt." }
                    };
                }

                return new ResultModel<Day>
                {
                    IsSuccess = true,
                    Value = day
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<Day>
                {
                    IsSuccess = false,
                    Errors = new List<string> { ex.Message },
                };
            }
        }

    }
}