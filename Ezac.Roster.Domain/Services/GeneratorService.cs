using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Services;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.Member;
using Ezac.Roster.Infrastructure.Models.PlannedService;
using Ezac.Roster.Infrastructure.Models.Result;
using System.Drawing.Text;

namespace Ezac.Roster.Domain.Services
{
    public class GeneratorService(ICalendarService calendarService, IPreferenceService preferenceService, IPlannedServiceService plannedServiceService, IServiceService serviceService, IPartOfDayService partOfDayService, IMemberService memberService) : IGenerateService
    {
        private readonly ICalendarService _calendarService = calendarService;
        private readonly IPreferenceService _preferenceService = preferenceService;
        private readonly IPlannedServiceService _plannedServiceService = plannedServiceService;
        private readonly IServiceService _serviceService = serviceService;
        private readonly IPartOfDayService _partOfDayService = partOfDayService;
        private readonly IMemberService _memberService = memberService;


        public async Task<ResultModel<IEnumerable<PlannedService>>> GenerateCalendar(int calendarId)
        {
            var calendarResult = await _calendarService.GetByIdAsync(calendarId);

            if (calendarResult.Value == null)
            {
                return FailureResult("Kalender niet gevonden!");
            }

            var calendar = calendarResult.Value;
            var allservices = await _serviceService.GetAllAsync();
            var servicesWeights = allservices.Value.Where(s => s.PartOfDayId != null && s.PartOfDay.Day.CalendarId == calendarId).Select(s => s.Weight).ToList();

            int totalWeight = 0;
            foreach (var weight in servicesWeights)
            {
                totalWeight += weight;
            }
            await SetTotalWeightOfMemberOnZero(calendarId);
            await _plannedServiceService.DeletePlannedServiceByCalendarId(calendarId);
            var preferencesResult = await _preferenceService.GetAllAsync();
            if (!preferencesResult.IsSuccess)
            {
                return FailureResult("Voorkeuren niet gevonden!");
            }
            List<Preference> preferences = new();
            if (preferencesResult.Value != null)
            {
                preferences = preferencesResult.Value.Where(p => p.CalendarId == calendarId).ToList();
            }

            var result = new List<PlannedService>();
            foreach (var day in calendar.Days)
            {
                foreach (var partOfDay in day.PartOfDays)
                {
                    var partOfDayResult = await _partOfDayService.GetByIdAsync(partOfDay.Id);
                    if (!partOfDayResult.IsSuccess || partOfDayResult.Value == null)
                    {
                        continue;
                    }
                    var partOfDayWithServices = partOfDayResult.Value;
                    foreach (var service in partOfDayWithServices.Services)
                    {

                        var plannedServiceResult = await PlanServiceForPartOfDay(calendarId, day, partOfDay, service, preferences, totalWeight);
                        if (plannedServiceResult == null)
                        {
                            return FailureResult("Er is iets foutgegaan");
                        }
                        result.AddRange(plannedServiceResult);
                    }
                }
            }
            return SuccessResult(result);
        }

        private async Task SetTotalWeightOfMemberOnZero(int calendarId)
        {
            var allmembers = await _memberService.GetAllAsync();
            var AllMembersFromCalendar = allmembers.Value.Where(m => m.CalendarId == calendarId).ToList();
            foreach (var member in AllMembersFromCalendar)
            {
                member.TotalWeight = 0;
                 await UpdateMemberWeight(member);
            }
        }

        public async Task<IEnumerable<PlannedService>> PlanServiceForPartOfDay(int calendarId, Day day, PartOfDay partOfDay, Service service, List<Preference> preferences, int totalWeight)
        {
            var result = new List<PlannedService>();
            var allmembers = await _memberService.GetAllAsync();
            var membersFromCalendar = allmembers.Value.Where(m => m.CalendarId == calendarId).ToList();
            double totalScaling = 0;
            foreach (var member in membersFromCalendar)
            {
                totalScaling += member.Scaling;
            }
            var MaxWeightPerMember = totalWeight / totalScaling;
            if(MaxWeightPerMember < 10)
            {
                MaxWeightPerMember = 10;
            }
            List<Member> qualifiedMembers = new();

            foreach (var member in membersFromCalendar)
            {
                var memberQualifications = member.Qualifications.Where(q => q.Services != null);
                foreach (var qualification in memberQualifications)
                {
                    if (qualification.Services.Any(s => s.Id == service.Id) && qualification.Experience >= service.RequeredExperience)
                    {
                        qualifiedMembers.Add(member);
                    }
                }
            }



            var filteredMembers = FilterMembers(qualifiedMembers, preferences, partOfDay.Id, service.Id, service.Weight, MaxWeightPerMember);
            if (!filteredMembers.Any())
            {
                filteredMembers = GetAvailableMembers(qualifiedMembers, partOfDay.Id, service.Weight, MaxWeightPerMember);
            }
            if (filteredMembers.Any())
            {
                var member = filteredMembers.First();
                var newPlannedService = new PlannedServiceCreateRequestModel
                {
                    MemberId = member.Id,
                    PartOfDayId = partOfDay.Id,
                    ServiceId = service.Id,
                    Weight = service.Weight,
                    DayId = day.Id,
                    CalendarId = calendarId,
                };

                var createdResult = await _plannedServiceService.CreateAsync(newPlannedService);
                if (!createdResult.IsSuccess)
                {
                    return null;
                }

                member.TotalWeight += service.Weight;
                var updateMemberResult = await UpdateMemberWeight(member);
                if (!updateMemberResult.IsSuccess)
                {
                    return null;
                }

                result.Add(createdResult.Value);
            }
            else
            {
                var newPlannedService = new PlannedServiceCreateRequestModel
                {
                    PartOfDayId = partOfDay.Id,
                    ServiceId = service.Id,
                    Weight = service.Weight,
                    DayId = day.Id,
                    CalendarId = calendarId,
                };

                var createdResult = await _plannedServiceService.CreateAsync(newPlannedService);
                if (!createdResult.IsSuccess)
                {
                    return null;
                }
                result.Add(createdResult.Value);
            }

            return result;
        }

        public async Task<ResultModel<Member>> UpdateMemberWeight(Member member)
        {
            var model = new MemberUpdateRequestModel
            {
                MemberId = member.Id,
                Scaling = member.Scaling,
                TotalWeight = member.TotalWeight,
            };
            var result = await _memberService.UpdateAsync(model);

            if (!result.IsSuccess)
            {
                return new ResultModel<Member>
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Fout bij het updaten van lid" }
                };
            }

            return result;
        }


        public List<Member> FilterMembers(IEnumerable<Member> qualifiedMembers, List<Preference> preferences, int partOfDayId, int serviceId, int serviceWeight, double maxWeightPerMember)
        {
            if (qualifiedMembers == null)
            {
                throw new ArgumentNullException(nameof(qualifiedMembers), "De gekwalificeerde members kunnen niet null zijn");
            }

            var preferredMemberIds = preferences
                .Where(p => p.PartOfDayId == partOfDayId && p.ServiceId == serviceId)
                .Select(p => p.MemberId)
                .ToList();

            return qualifiedMembers
                .Where(m => preferredMemberIds.Contains(m.Id) && m.TotalWeight + serviceWeight <= maxWeightPerMember * m.Scaling)
                .OrderBy(m => m.TotalWeight)
                .ToList();
        }

        public List<Member> GetAvailableMembers(IEnumerable<Member> qualifiedMembers, int partOfDayId, int serviceWeight, double maxWeightPerMember)
        {
            return qualifiedMembers.Where(m =>
                (m.Preferences.Count() == 0 || m.Preferences == null || m.Preferences.Any(p => p.PartOfDayId == partOfDayId)) &&
                m.TotalWeight + serviceWeight <= maxWeightPerMember * m.Scaling)
                .OrderBy(m => m.TotalWeight).ToList();
        }

        private ResultModel<IEnumerable<PlannedService>> FailureResult(string error)
        {
            return new ResultModel<IEnumerable<PlannedService>>
            {
                IsSuccess = false,
                Errors = new List<string> { error }
            };
        }

        private ResultModel<IEnumerable<PlannedService>> SuccessResult(IEnumerable<PlannedService> plannedServices)
        {
            return new ResultModel<IEnumerable<PlannedService>>
            {
                IsSuccess = true,
                Value = plannedServices
            };
        }
    }
}
