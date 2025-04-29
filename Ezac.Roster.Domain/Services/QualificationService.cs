using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.Qualification;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Services
{
    public class QualificationService(IQualificationRepository qualificationRepository,IServiceRepository serviceRepository,IMemberRepository memberRepository) : IQualificationService
    {
        private readonly IQualificationRepository _qualificationRepository = qualificationRepository;
        private readonly IServiceRepository _serviceRepository = serviceRepository;
        private readonly IMemberRepository _memberRepository = memberRepository;

        public async Task<ResultModel<Qualification>> CreateAsync(QualificationCreateRequestModel qualificationCreateRequestModel)
        {
            //check if qualification already exists
            if(_qualificationRepository.GetAll().Any(q => q.Name.ToUpper().Equals(qualificationCreateRequestModel.Name.ToUpper())))
            {
                return new ResultModel<Qualification>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kwalificatie bestaat al." }
                };
            }
            //check if services exist
            if(qualificationCreateRequestModel.ServiceIds == null || !qualificationCreateRequestModel.ServiceIds.Any())
            {
                return new ResultModel<Qualification>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Er werden geen diensten gevonden." }
                };
            }
            var services = _serviceRepository.GetAll().Where(s => qualificationCreateRequestModel.ServiceIds.Contains(s.Id)).ToList();
            if(services == null || !services.Any())
            {
                return new ResultModel<Qualification>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Er werden geen diensten gevonden." }
                };
            }
            //check if member exists
            var member = _memberRepository.GetAll().FirstOrDefault(m => m.Id == qualificationCreateRequestModel.memberId);
            if(member == null)
            {
                return new ResultModel<Qualification>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze gebruiker werd niet gevonden." }
                };
            }
            //check if experience is valid not under 0 and not above 10
            if(qualificationCreateRequestModel.Experience < 0 || qualificationCreateRequestModel.Experience > 10)
            {
                return new ResultModel<Qualification>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Ervaring moet tussen 0 en 10 liggen." }
                };
            }
            var qualification = new Qualification
            {
                Name = qualificationCreateRequestModel.Name,
                MemberId = qualificationCreateRequestModel.memberId,
                Services = _serviceRepository.GetAll().Where(s => qualificationCreateRequestModel.ServiceIds.Contains(s.Id)).ToList(),
                Experience = qualificationCreateRequestModel.Experience
            };
            var result = await _qualificationRepository.AddAsync(qualification);
            if (result)
            {
                return new ResultModel<Qualification>
                {
                    IsSuccess = true,
                    Value = qualification
                };
            }
            return new ResultModel<Qualification>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het aanmaken van deze kwalificatie is mislukt." }
            };
        }

        public async Task<ResultModel<Qualification>> DeleteAsync(int id)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
            {
                return new ResultModel<Qualification>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kwalificatie werd niet gevonden." }
                };
            }
            if(await _qualificationRepository.DeleteAsync(qualification))
            {
                return new ResultModel<Qualification>
                {
                    IsSuccess = true,
                };
            }
            return new ResultModel<Qualification>
            {
                IsSuccess = false,
                Errors = new List<string> { "Deze kwalificatie kon niet worden verwijderd." }
            };
        }

        public async Task<ResultModel<IEnumerable<Qualification>>> GetAllAsync()
        {
            var qualifications = await _qualificationRepository.GetAllAsync();
            if (qualifications.Any())
            {
                return new ResultModel<IEnumerable<Qualification>>
                {
                    IsSuccess = true,
                    Value = qualifications
                };
            }
            return new ResultModel<IEnumerable<Qualification>>
            {
                IsSuccess = false,
                Errors = new List<string> { "Er werden geen kwalificaties gevonden." }
            };
        }

        public async Task<ResultModel<Qualification>> GetByIdAsync(int id)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
            {
                return new ResultModel<Qualification>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kwalificatie werd niet gevonden." }
                };
            }
            return new ResultModel<Qualification>
            {
                IsSuccess = true,
                Value = qualification
            };
        }

        public async Task<ResultModel<Qualification>> UpdateAsync(QualificationUpdateRequestModel qualificationUpdateRequestModel)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(qualificationUpdateRequestModel.Id);
            if (qualification == null)
            {
                return new ResultModel<Qualification>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kwalificatie werd niet gevonden." }
                };
            }
            //check if name is unique
            if(qualification.Name != qualificationUpdateRequestModel.Name)
            {
                if(_qualificationRepository.GetAll().Any(q => q.Name.ToUpper().Equals(qualificationUpdateRequestModel.Name.ToUpper())))
                {
                    return new ResultModel<Qualification>
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Deze kwalificatie bestaat al." }
                    };
                }
            }
            qualification.Name = qualificationUpdateRequestModel.Name;
            if(await _qualificationRepository.UpdateAsync(qualification))
            {
                return new ResultModel<Qualification>
                {
                    IsSuccess = true,
                    Value = qualification
                };
            }
            return new ResultModel<Qualification>
            {
                IsSuccess = false,
                Errors = new List<string> { "Deze kwalificatie kon niet worden gewijzigd." }
            };
        }
    }
}
