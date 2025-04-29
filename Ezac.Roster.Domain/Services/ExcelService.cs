using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Services;
using Ezac.Roster.Domain.Models.Member;
using Ezac.Roster.Domain.Models.Preference;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.Member;
using Ezac.Roster.Infrastructure.Models.Result;
using OfficeOpenXml;

namespace Ezac.Roster.Domain.Services
{
    public class ExcelService(IMemberService memberService, IPreferenceService preferenceService, IPlannedServiceService plannedServiceService, IPartOfDayService partOfDayService, IServiceService serviceService, ICalendarService calendarService) : IExcelService
    {

        private readonly IMemberService _memberService = memberService;
        private readonly IPreferenceService _preferenceService = preferenceService;
        private readonly IPlannedServiceService _plannedServiceService = plannedServiceService;
        private readonly IPartOfDayService _partOfDayService = partOfDayService;
        private readonly IServiceService _serviceService = serviceService;
        private readonly ICalendarService _calendarService = calendarService;


        public async Task<ResultModel<IEnumerable<MemberAndQualificationRequestModel>>> ReadFileAsync(Stream stream)
        {
            var result = new ResultModel<IEnumerable<MemberAndQualificationRequestModel>>();
            try
            {
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset position to beginning
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(memoryStream);
                var worksheet = package.Workbook.Worksheets[0];
                var rows = worksheet.Dimension.End.Row;
                var members = new List<MemberAndQualificationRequestModel>();

                for (int row = 2; row <= rows; row++)
                {
                    var member = new MemberAndQualificationRequestModel()
                    {
                        OriginalId = worksheet.Cells[row, 1].Value?.ToString(),
                        Name = worksheet.Cells[row, 2].Value?.ToString(),
                        Email = worksheet.Cells[row, 3].Value?.ToString(),
                        InstructorLicense = int.Parse(worksheet.Cells[row, 4].Value?.ToString()),
                        LierLicense = int.Parse(worksheet.Cells[row, 5].Value?.ToString()),
                        StartingOfficerLicense = int.Parse(worksheet.Cells[row, 6].Value?.ToString()),
                        BarLicense = int.Parse(worksheet.Cells[row, 7].Value?.ToString()),
                        Scaling = double.Parse(worksheet.Cells[row, 8].Value?.ToString()),
                    };
                    members.Add(member);

                }

                result.Value = members;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Errors = new List<string> { ex.Message };
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<bool> SaveMembersAndQualificationsAsync(IEnumerable<MemberAndQualificationRequestModel> members)
        {

            var result = new ResultModel<IEnumerable<Member>>();
            var savedMembers = new List<Member>();
            var calendarId = members.FirstOrDefault().CalendarId;
            try
            {
                await _memberService.RemoveMemberByCalendarId(calendarId);
                foreach (var member in members)
                {
                    var request = new MemberCreateRequestModel
                    {
                        OriginalId = member.OriginalId,
                        Name = member.Name,
                        Email = member.Email,
                        CalendarId = member.CalendarId,
                        Scaling = member.Scaling
                    };
                    var isSaved = await _memberService.CreateAsync(request);
                    if (isSaved.IsSuccess)
                    {
                        var qualificationRequest = new MemberAddQualificationRequestModel
                        {
                            MemberId = isSaved.Value.Id,
                            InstructorLicense = member.InstructorLicense,
                            LierLicense = member.LierLicense,
                            StartingOfficerLicense = member.StartingOfficerLicense,
                            BarLicense = member.BarLicense
                        };
                        var isQualificationSaved = await _memberService.AddQualification(qualificationRequest);
                        if (isSaved.IsSuccess && isQualificationSaved.IsSuccess)
                        {
                            savedMembers.Add(isSaved.Value);
                        }
                    }
                }

                result.Value = savedMembers;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Errors = new List<string> { ex.Message };
                result.IsSuccess = false;
            }
            return true;
        }
        public async Task<string> ExportPlannedServicesAsync(int calendarId)
        {
            try
            {
                var calendarResult = await _calendarService.GetByIdAsync(calendarId);
                if (!calendarResult.IsSuccess || calendarResult.Value == null)
                {
                    return null;
                }

                var calendar = calendarResult.Value;
                var plannedServices = calendar.PlannedServices;

                var partOfDays = calendar.Days.SelectMany(d => d.PartOfDays).ToList();
                var services = calendar.Days.SelectMany(d => d.PartOfDays).SelectMany(p => p.Services).ToList();
                var members = calendar.Members.ToList();

                if (!plannedServices.Any())
                {
                    return null;
                }

                var days = calendar.Days.ToList();

                if (!days.Any())
                {
                    return null;
                }

                var startYear = days.Min(d => d.Date.Year);
                var endYear = days.Max(d => d.Date.Year);
                var yearRange = startYear == endYear ? startYear.ToString() : $"{startYear}-{endYear}";

                var partOfDayIds = plannedServices.Select(ps => ps.PartOfDayId).Distinct().ToList();
                var serviceIds = plannedServices.Select(ps => ps.ServiceId).Distinct().ToList();
                var memberIds = plannedServices.Select(ps => ps.MemberId).Distinct().ToList();

                var filteredPartOfDays = partOfDays.Where(p => partOfDayIds.Contains(p.Id)).ToList();
                var filteredServices = services.Where(s => serviceIds.Contains(s.Id)).ToList();
                var filteredMembers = members.Where(m => memberIds.Contains(m.Id)).ToList();

                var plannedServicesData = plannedServices
                    .Select(ps => new
                    {
                        PartOfDay = filteredPartOfDays.FirstOrDefault(p => p.Id == ps.PartOfDayId),
                        Service = filteredServices.FirstOrDefault(s => s.Id == ps.ServiceId),
                        Member = filteredMembers.FirstOrDefault(m => m.Id == ps.MemberId),
                    })
                    .ToList();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("PlannedServices");

                    var headers = new List<string> { $"Periode {yearRange}" };
                    var serviceHeaderDictionary = new Dictionary<string, int>();

                    foreach (var partOfDay in filteredPartOfDays)
                    {
                        foreach (var service in filteredServices.Where(s => s.PartOfDayId == partOfDay.Id))
                        {
                            var header = $"{service.Name} {partOfDay.Name}";
                            var count = plannedServicesData.Count(ps => ps.Service.Name == service.Name && ps.PartOfDay.Name == partOfDay.Name);
                            if (count > 0)
                            {
                                if (!serviceHeaderDictionary.ContainsKey(header))
                                {
                                    serviceHeaderDictionary[header] = count;
                                }
                            }
                        }
                    }

                    foreach (var kvp in serviceHeaderDictionary)
                    {
                        for (int i = 1; i <= kvp.Value; i++)
                        {
                            headers.Add($"{kvp.Key} {i}");
                        }
                    }

                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                    }

                    int row = 2;
                    foreach (var day in days)
                    {
                        var dayParts = plannedServicesData.Where(ps => ps.PartOfDay.DayId == day.Id).ToList();

                        if (dayParts.Any())
                        {
                            worksheet.Cells[row, 1].Value = $"{day.Date:dddd dd-MM-yyyy}";

                            var serviceCountDictionary = new Dictionary<string, int>();

                            for (int col = 1; col < headers.Count; col++)
                            {
                                var headerParts = headers[col].Split(' ');
                                if (headerParts.Length >= 2)
                                {
                                    var serviceName = headerParts[0];
                                    var partOfDayName = headerParts[1];

                                    var headerKey = $"{serviceName} {partOfDayName}";
                                    if (!serviceCountDictionary.ContainsKey(headerKey))
                                    {
                                        serviceCountDictionary[headerKey] = 1;
                                    }
                                    else
                                    {
                                        serviceCountDictionary[headerKey]++;
                                    }

                                    var instanceIndex = serviceCountDictionary[headerKey];
                                    var plannedService = dayParts
                                        .Where(ps => ps.Service.Name == serviceName && ps.PartOfDay.Name == partOfDayName)
                                        .Skip(instanceIndex - 1)
                                        .FirstOrDefault();

                                    if (plannedService != null)
                                    {
                                        worksheet.Cells[row, col + 1].Value = $"{plannedService.Member?.Name} ({plannedService.Member?.OriginalId})";
                                    }
                                }
                            }

                            row++;
                        }
                    }

                    for (int col = headers.Count; col >= 2; col--)
                    {
                        bool isEmptyColumn = true;
                        for (int r = 2; r <= row; r++)
                        {
                            if (worksheet.Cells[r, col].Value != null)
                            {
                                isEmptyColumn = false;
                                break;
                            }
                        }

                        if (isEmptyColumn)
                        {
                            worksheet.DeleteColumn(col);
                        }
                    }

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    var bytes = stream.ToArray();
                    return Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return null;
            }
        }




    }

}