using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Domain.Models.Member;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.Member;
using Ezac.Roster.Infrastructure.Models.Qualification;
using Ezac.Roster.Infrastructure.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ICalendarRepository _calendarRepository;
        private readonly IQualificationRepository _qualificationRepository;
        private readonly IServiceRepository _serviceRepository;

        public MemberService(
            IMemberRepository memberRepository,
            ICalendarRepository calendarRepository,
            IQualificationRepository qualificationRepository,
            IServiceRepository serviceRepository)
        {
            _memberRepository = memberRepository;
            _calendarRepository = calendarRepository;
            _qualificationRepository = qualificationRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<ResultModel<Member>> AddQualification(MemberAddQualificationRequestModel memberAddQualificationRequestModel)
        {
            //check if user exists
            var member = await _memberRepository.GetByIdAsync(memberAddQualificationRequestModel.MemberId);
            if (member == null)
            {
                return new ResultModel<Member>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze gebruiker werd niet gevonden." }
                };
            }
            //add qualifications to member
            List<Qualification> qualifications = new()
            {
                new Qualification
                {
                    Name = "Lierist",
                    Experience = memberAddQualificationRequestModel.LierLicense,
                    Services = _serviceRepository.GetAll().Where(s => s.Name == "Lieren").ToList(),
                    MemberId = memberAddQualificationRequestModel.MemberId
                },
                new Qualification
                {
                    Name = "Startofficier",
                    Experience = memberAddQualificationRequestModel.StartingOfficerLicense,
                    Services = _serviceRepository.GetAll().Where(s => s.Name == "Startofficier").ToList(),
                    MemberId = memberAddQualificationRequestModel.MemberId
                },
                new Qualification
                {
                    Name = "Instructeur",
                    Experience = memberAddQualificationRequestModel.InstructorLicense,
                    Services = _serviceRepository.GetAll().Where(s => s.Name == "Instructeur" || s.Name == "DDI Instructeur").ToList(),
                    MemberId = memberAddQualificationRequestModel.MemberId
                },
                new Qualification
                {
                    Name = "Bar",
                    Experience = memberAddQualificationRequestModel.BarLicense,
                    Services = _serviceRepository.GetAll().Where(s => s.Name == "Bar").ToList(),
                    MemberId = memberAddQualificationRequestModel.MemberId
                }
            };
            foreach (var qualification in qualifications)
            {
                if (!await _qualificationRepository.AddAsync(qualification))
                {
                    return new ResultModel<Member>
                    {
                        IsSuccess = false,
                    };
                }
                else
                {
                    member.Qualifications.Add(qualification);
                }
            }
            return new ResultModel<Member>
            {
                IsSuccess = true,
                Value = member
            };
        }

        public async Task<ResultModel<Member>> CreateAsync(MemberCreateRequestModel memberCreateRequestModel)
        {
            // Check if calendar exists
            var calendar = await _calendarRepository.GetByIdAsync(memberCreateRequestModel.CalendarId);
            if (calendar == null)
            {
                return new ResultModel<Member>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kalender werd niet gevonden." }
                };
            }
            //check if scaling is between 0 and 1
            if (memberCreateRequestModel.Scaling < 0 || memberCreateRequestModel.Scaling > 1)
            {
                return new ResultModel<Member>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "De inschaling moet tussen 0 en 1 liggen." }
                };
            }
            Member member = new()
            {
                OriginalId = memberCreateRequestModel.OriginalId,
                Id = Guid.NewGuid().ToString(),
                Name = memberCreateRequestModel.Name,
                Email = memberCreateRequestModel.Email,
                Scaling = memberCreateRequestModel.Scaling,
                CalendarId = memberCreateRequestModel.CalendarId
            };

            if (await _memberRepository.AddAsync(member))
            {
                return new ResultModel<Member>
                {
                    IsSuccess = true,
                    Value = member
                };
            }

            return new ResultModel<Member>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het aanmaken van deze gebruiker is mislukt." }
            };
        }

        public async Task<ResultModel<IEnumerable<Member>>> DeleteAllAsync()
        {
            // Check if there are members
            var members = await _memberRepository.GetAllAsync();
            if (!members.Any())
            {
                return new ResultModel<IEnumerable<Member>>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Er zijn geen gebruikers om te verwijderen." }
                };
            }

            var result = await _memberRepository.DeleteAllAsync();
            if (result)
            {
                return new ResultModel<IEnumerable<Member>>
                {
                    IsSuccess = true
                };
            }

            return new ResultModel<IEnumerable<Member>>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het verwijderen van alle gebruikers is mislukt." }
            };
        }

        public async Task<ResultModel<Member>> DeleteAsync(string id)
        {
            var user = await _memberRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new ResultModel<Member>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze gebruiker werd niet gevonden." }
                };
            }

            if (await _memberRepository.DeleteAsync(user))
            {
                return new ResultModel<Member>
                {
                    IsSuccess = true,
                    Value = user
                };
            }

            return new ResultModel<Member>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het verwijderen van deze gebruiker is mislukt." }
            };
        }

        public async Task<ResultModel<IEnumerable<Member>>> GetAllAsync()
        {
            var members = await _memberRepository.GetAllAsync();
            if (members.Any())
            {
                return new ResultModel<IEnumerable<Member>>
                {
                    IsSuccess = true,
                    Value = members
                };
            }

            return new ResultModel<IEnumerable<Member>>
            {
                IsSuccess = false,
                Errors = new List<string> { "Er werden geen gebruikers gevonden." }
            };
        }

        public async Task<ResultModel<Member>> GetByIdAsync(string id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
            {
                return new ResultModel<Member>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze gebruiker werd niet gevonden." }
                };
            }

            return new ResultModel<Member>
            {
                IsSuccess = true,
                Value = member
            };
        }

        public async Task<ResultModel<IEnumerable<Member>>> RemoveMemberByCalendarId(int calendarId)
        {
            // Check if calendar exists
            var calendar = await _calendarRepository.GetByIdAsync(calendarId);
            if (calendar == null)
            {
                return new ResultModel<IEnumerable<Member>>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kalender werd niet gevonden." }
                };
            }

            // Check if there are members in this calendar
            var members = await _memberRepository.GetAllAsync();
            var membersInCalendar = members.Where(m => m.CalendarId == calendarId);
            if (!membersInCalendar.Any())
            {
                return new ResultModel<IEnumerable<Member>>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze kalender heeft geen leden." }
                };
            }

            var result = await _memberRepository.RemoveMemberByCalendarId(calendarId);
            if (result)
            {
                return new ResultModel<IEnumerable<Member>>
                {
                    IsSuccess = true
                };
            }

            return new ResultModel<IEnumerable<Member>>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het verwijderen van deze gebruiker is mislukt." }
            };
        }

        public async Task<ResultModel<Member>> UpdateAsync(MemberUpdateRequestModel memberUpdateRequestModel)
        {
            var member = await _memberRepository.GetByIdAsync(memberUpdateRequestModel.MemberId);
            if (member == null)
            {
                return new ResultModel<Member>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Deze gebruiker werd niet gevonden." }
                };
            }
            member.Scaling = memberUpdateRequestModel.Scaling;
            member.TotalWeight = memberUpdateRequestModel.TotalWeight;
            if (await _memberRepository.UpdateAsync(member))
            {
                return new ResultModel<Member>
                {
                    IsSuccess = true,
                    Value = member
                };
            }

            return new ResultModel<Member>
            {
                IsSuccess = false,
                Errors = new List<string> { "Het wijzigen van deze gebruiker is gefaald." }
            };
        }
    }
}

