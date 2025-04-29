using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.Member;
using Ezac.Roster.Infrastructure.Models.PlannedService;
using Ezac.Roster.Infrastructure.Models.Qualification;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Services
{
    public class PlannedServices(IPlannedServiceRepository plannedServiceRepository,ICalendarRepository calendarRepository) : IPlannedServiceService
    {
        private readonly IPlannedServiceRepository _plannedServiceRepository = plannedServiceRepository;
        private readonly ICalendarRepository _calendarRepository = calendarRepository;

        public async Task<ResultModel<PlannedService>> CreateAsync(PlannedServiceCreateRequestModel plannedServiceCreateRequestModel)
        {
            // Controleer of het invoermodel geldig is
            if (plannedServiceCreateRequestModel == null)
            {
                return new ResultModel<PlannedService>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze geplande service kon niet worden aangemaakt." }
                };
            }

            PlannedService plannedService = new()
            {
                PartOfDayId = plannedServiceCreateRequestModel.PartOfDayId,
                ServiceId = plannedServiceCreateRequestModel.ServiceId,
                MemberId = plannedServiceCreateRequestModel.MemberId,
                Weight = plannedServiceCreateRequestModel.Weight,
                CalendarId = plannedServiceCreateRequestModel.CalendarId,
            };

            if (await _plannedServiceRepository.AddAsync(plannedService))
            {
                return new ResultModel<PlannedService>()
                {
                    IsSuccess = true,
                    Value = plannedService
                };
            }
            else
            {
                return new ResultModel<PlannedService>()
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze geplande service kon niet worden aangemaakt." }
                };
            }
        }


        public async Task<ResultModel<PlannedService>> DeleteAsync(int id)
        {
            var toDelete = await _plannedServiceRepository.GetByIdAsync(id);

            if (toDelete != null)
            {
                if (await _plannedServiceRepository.DeleteAsync(toDelete))
                {
                    return new ResultModel<PlannedService> { IsSuccess = true };
                }
                else
                {
                    return new ResultModel<PlannedService>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Deze geplande service kon niet worden verwijderd." }
                    };
                }
            }
            else
            {
                return new ResultModel<PlannedService>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze geplande service werd niet gevonden." }
                };
            }
        }


        public async Task<ResultModel<IEnumerable<PlannedService>>> GetAllAsync()
        {
            var plannedServices = await _plannedServiceRepository.GetAllAsync();
            if (plannedServices.Any())
            {
                return new ResultModel<IEnumerable<PlannedService>>
                {
                    IsSuccess = true,
                    Value = plannedServices
                };
            }
            return new ResultModel<IEnumerable<PlannedService>>
            {
                IsSuccess = false,
                Errors = new List<string> { "Er werden geen geplande services gevonden." }
            };
        }
        public async Task<ResultModel<IEnumerable<PlannedService>>> DeleteByCalendarIdAsync(int calendarId)
        {
            var calendar = await _calendarRepository.GetByIdAsync(calendarId);
            if (calendar == null)
            {
                return new ResultModel<IEnumerable<PlannedService>>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kalender werd niet gevonden." }
                };
            }
            var plannedServices = await _plannedServiceRepository.GetAllAsync();
            var plannedServicesInCalendar = plannedServices.Where(m => m.CalendarId == calendarId);
            if (!plannedServicesInCalendar.Any())
            {
                return new ResultModel<IEnumerable<PlannedService>>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kalender heeft geen services gepland." }
                };
            }
            var result = await _plannedServiceRepository.DeleteByCalendarIdAsync(calendarId);
            if(result)
            {
                return new ResultModel<IEnumerable<PlannedService>>
                {
                    IsSuccess = true,
                };
            }
            return new ResultModel<IEnumerable<PlannedService>>
            {
                IsSuccess = false,
                Errors = new List<string> { "Deze geplande services konden niet worden verwijderd." }
            };
        }

        public async Task<ResultModel<PlannedService>> GetByIdAsync(int id)
        {
            var plannedService = await _plannedServiceRepository.GetByIdAsync(id);
            if (plannedService == null)
            {
                return new ResultModel<PlannedService>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze geplande service werd niet gevonden." }
                };
            }
            return new ResultModel<PlannedService>
            {
                IsSuccess = true,
                Value = plannedService,
            };
        }
        public async Task<ResultModel<IEnumerable<PlannedService>>> DeletePlannedServiceByCalendarId(int calendarId)
        {
            var plannedServices = await _plannedServiceRepository.GetAllAsync();
            var plannedServicesToDelete = plannedServices.Where(p => p.CalendarId == calendarId);
            if (plannedServicesToDelete.Any())
            {
                foreach (var plannedService in plannedServicesToDelete)
                {
                    await _plannedServiceRepository.DeleteAsync(plannedService);
                }
                return new ResultModel<IEnumerable<PlannedService>>
                {
                    IsSuccess = true,
                };
            }
            return new ResultModel<IEnumerable<PlannedService>>
            {
                IsSuccess = false,
                Errors = new List<string> { "Er werden geen geplande diensten gevonden." }
            };
        }
        public async Task<ResultModel<PlannedService>> UpdateAsync(PlannedServiceUpdateRequestModel plannedServiceUpdateRequestModel)
        {
            var plannedService = await _plannedServiceRepository.GetByIdAsync(plannedServiceUpdateRequestModel.Id);
            if (plannedService == null)
            {
                return new ResultModel<PlannedService>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze geplande service werd niet gevonden." }
                };
            }

            plannedService.PartOfDayId = plannedServiceUpdateRequestModel.PartOfDayId;
            plannedService.ServiceId = plannedServiceUpdateRequestModel.ServiceId;
            plannedService.MemberId = plannedServiceUpdateRequestModel.MemberId;
            plannedService.Weight = plannedServiceUpdateRequestModel.Weight;

            if (await _plannedServiceRepository.UpdateAsync(plannedService))
            {
                return new ResultModel<PlannedService>
                {
                    IsSuccess = true,
                    Value = plannedService
                };
            }
            return new ResultModel<PlannedService>
            {
                IsSuccess = false,
                Errors = new List<string> { "Deze geplande service kon niet worden gewijzigd." }
            };
        }
    }
}
