using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.Service;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ezac.Roster.Domain.Models.Service;
using Microsoft.EntityFrameworkCore;

namespace Ezac.Roster.Domain.Services
{
    public class ServiceService(IServiceRepository serviceRepository, IPartOfDayRepository partOfDayRepository, IQualificationRepository qualificationRepository) : IServiceService
    {
        private readonly IServiceRepository _serviceRepository = serviceRepository;
        private readonly IPartOfDayRepository _partOfDayRepository = partOfDayRepository;
        private readonly IQualificationRepository _qualificationRepository = qualificationRepository ;
        public async Task<ResultModel<Service>> CreateAsync(ServiceCreateRequestModel serviceCreateRequestModel)
        {
            List<Qualification> requiredQualifications = new();
            foreach(var id in serviceCreateRequestModel.RequeredQualifications)
            {
                var qualificationResult = await _qualificationRepository.GetByIdAsync(id);
                if(qualificationResult != null)
                {
                    requiredQualifications.Add(qualificationResult);
                }
                else
                {
                    return new ResultModel<Service>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Kwalificatie niet gevonden!" }
                    };
                }
            }

            var service = new Service
            {
                Name = serviceCreateRequestModel.Name,
                RequeredExperience = serviceCreateRequestModel.RequeredExperience,
                Weight = 5,
                RequiredQualifications = requiredQualifications,
                PartOfDayId = serviceCreateRequestModel.PartOfDayId
            };
            var result = await _serviceRepository.AddAsync(service);
            if (result)
            {
                return new ResultModel<Service>
                {
                    IsSuccess = true,
                    Value = service
                };
            }
            return new ResultModel<Service>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het aanmaken van deze dienst is mislukt." }
            };
        }


        public async Task<ResultModel<Service>> DeleteAsync(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return new ResultModel<Service>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze dienst werd niet gevonden." }
                };
            }
            if (await _serviceRepository.DeleteAsync(service))
            {
                return new ResultModel<Service>
                {
                    IsSuccess = true,
                };
            }
            return new ResultModel<Service>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het verwijderen van deze dienst is mislukt." }
            };
        }

        public async Task<ResultModel<IEnumerable<Service>>> GetAllAsync()
        {
            var services = await _serviceRepository.GetAllAsync();
            if (services.Any())
            {
                return new ResultModel<IEnumerable<Service>>
                {
                    IsSuccess = true,
                    Value = services
                };
            }
            return new ResultModel<IEnumerable<Service>>
            {
                IsSuccess = false,
                Errors = new List<string> { "Er werden geen diensten gevonden." }
            };
        }

        public async Task<ResultModel<Service>> GetByIdAsync(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return new ResultModel<Service>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze dienst werd niet gevonden." }
                };
            }
            return new ResultModel<Service>
            {
                IsSuccess = true,
                Value = service
            };
        }
        
        public async Task<ResultModel<Service>> UpdateAsync(ServiceUpdateRequestModel serviceUpdateRequestModel)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceUpdateRequestModel.Id);
            if (service == null)
            {
                return new ResultModel<Service>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze dienst werd niet gevonden." }
                };
            }
            
            service.Weight = serviceUpdateRequestModel.Weight;
            service.RequeredExperience = serviceUpdateRequestModel.RequeredExperience;


            if (await _serviceRepository.UpdateAsync(service))
            {
                return new ResultModel<Service>
                {
                    IsSuccess = true,
                    Value = service
                };
            }
            return new ResultModel<Service>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het updaten van deze dienst is mislukt." }
            };
        }
    }
}
