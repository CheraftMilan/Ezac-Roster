using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.Calendar;
using Ezac.Roster.Infrastructure.Models.Qualification;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Services
{
    public class CalendarService(ICalendarRepository calendarRepository) : ICalendarService
    {
        private readonly ICalendarRepository _calendarRepository = calendarRepository;

        public async Task<ResultModel<Entities.Calendar>> CreateAsync(CalendarCreateRequestModel calendarCreateRequestModel)
        {
            //check if name already exists
            if(_calendarRepository.GetAll().Any(q => q.Name.ToUpper().Equals(calendarCreateRequestModel.Name.ToUpper())))
            {
                return new ResultModel<Entities.Calendar>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kalender bestaat al." }
                };
            }
            var calendar = new Entities.Calendar
            {
                Name = calendarCreateRequestModel.Name,
                Description = calendarCreateRequestModel.Description,
                Days = new List<Day>()
            };
            var result = await _calendarRepository.AddAsync(calendar);
            if (result)
            {
                return new ResultModel<Entities.Calendar>
                {
                    IsSuccess = true,
                    Value = calendar
                };
            }
            return new ResultModel<Entities.Calendar>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het aanmaken van de kalender is mislukt." }
            };
        }

        public async Task<ResultModel<Entities.Calendar>> DeleteAsync(int id)
        {
            var calendar = await _calendarRepository.GetByIdAsync(id);
            if (calendar == null)
            {
                return new ResultModel<Entities.Calendar>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kalender werd niet gevonden." }
                };
            }
            if (await _calendarRepository.DeleteAsync(calendar))
            {
                return new ResultModel<Entities.Calendar>
                {
                    IsSuccess = true,
                };
            }
            return new ResultModel<Entities.Calendar>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het verwijderen van deze kalender is mislukt." }
            };
        }

        public async Task<ResultModel<IEnumerable<Entities.Calendar>>> GetAllAsync()
        {
            var calendars = await _calendarRepository.GetAllAsync();
            if (calendars.Any())
            {
                return new ResultModel<IEnumerable<Entities.Calendar>>
                {
                    IsSuccess = true,
                    Value = calendars
                };
            }
            return new ResultModel<IEnumerable<Entities.Calendar>>
            {
                IsSuccess = false,
                Errors = new List<string> { "Er werden geen kalenders gevonden." }
            };
        }

        public async Task<ResultModel<Entities.Calendar>> GetByIdAsync(int id)
        {
            var calendar = await _calendarRepository.GetByIdAsync(id);
            if (calendar == null)
            {
                return new ResultModel<Entities.Calendar>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kalender werd niet gevonden." }
                };
            }
            return new ResultModel<Entities.Calendar>
            {
                IsSuccess = true,
                Value = calendar
            };
        }

        public async Task<ResultModel<Entities.Calendar>> UpdateAsync(CalendarUpdateRequestModel calendarUpdateRequestModel)
        {
            var calendar = await _calendarRepository.GetByIdAsync(calendarUpdateRequestModel.Id);
            if(calendar == null)
            {
                return new ResultModel<Entities.Calendar>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kalender werd niet gevonden." }
                };
            }
            if(_calendarRepository.GetAll().Any(q => q.Name.ToUpper().Equals(calendarUpdateRequestModel.Name.ToUpper()) && q.Id != calendarUpdateRequestModel.Id))
            {
                return new ResultModel<Entities.Calendar>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kalender bestaat al." }
                };
            }
            calendar.Name = calendarUpdateRequestModel.Name;
            calendar.Description = calendarUpdateRequestModel.Description;
            if(await _calendarRepository.UpdateAsync(calendar))
            {
                return new ResultModel<Entities.Calendar>
                {
                    IsSuccess = true,
                    Value = calendar
                };
            }
            return new ResultModel<Entities.Calendar>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het updaten van de kalender is mislukt." }
            };
        }
        
    }
}
