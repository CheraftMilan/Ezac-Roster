using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Services;
using Ezac.Roster.Domain.Models.Member;
using Ezac.Roster.Domain.Services;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.Member;
using Ezac.Roster.Infrastructure.Models.Result;
using Moq;
using OfficeOpenXml;
using Xunit;

namespace Ezac.Roster.UnitTests.Services
{
    public class ExcelServiceTests
    {
        private readonly Mock<IMemberService> _memberServiceMock;
        private readonly Mock<IPreferenceService> _preferenceServiceMock;
        private readonly Mock<IPlannedServiceService> _plannedServiceMock;
        private readonly Mock<IPartOfDayService> _partOfDayServiceMock;
        private readonly Mock<IServiceService> _serviceServiceMock;
        private readonly Mock<ICalendarService> _calendarServiceMock;
        private readonly ExcelService _excelService;

        public ExcelServiceTests()
        {
            _memberServiceMock = new Mock<IMemberService>();
            _preferenceServiceMock = new Mock<IPreferenceService>();
            _excelService = new ExcelService(_memberServiceMock.Object, _preferenceServiceMock.Object, _plannedServiceMock.Object, _partOfDayServiceMock.Object, _serviceServiceMock.Object, _calendarServiceMock.Object);
        }

        [Fact]
        public async Task ReadFileAsync_ShouldReturnMembers_WhenValidFile()
        {
            // Arrange
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Email";
                worksheet.Cells[1, 3].Value = "InstructorLicense";
                worksheet.Cells[1, 4].Value = "LierLicense";
                worksheet.Cells[1, 5].Value = "StartingOfficerLicense";
                worksheet.Cells[1, 6].Value = "BarLicense";
                worksheet.Cells[1, 7].Value = "Scaling";

                worksheet.Cells[2, 1].Value = "Test Name";
                worksheet.Cells[2, 2].Value = "test@example.com";
                worksheet.Cells[2, 3].Value = 1;
                worksheet.Cells[2, 4].Value = 2;
                worksheet.Cells[2, 5].Value = 3;
                worksheet.Cells[2, 6].Value = 4;
                worksheet.Cells[2, 7].Value = 0.5;

                package.Save();
                stream.Position = 0;
            }

            // Act
            var result = await _excelService.ReadFileAsync(stream);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
            var member = result.Value.First();
            Assert.Equal("Test Name", member.Name);
            Assert.Equal("test@example.com", member.Email);
            Assert.Equal(1, member.InstructorLicense);
            Assert.Equal(2, member.LierLicense);
            Assert.Equal(3, member.StartingOfficerLicense);
            Assert.Equal(4, member.BarLicense);
            Assert.Equal(0.5, member.Scaling);
        }

        [Fact]
        public async Task ReadFileAsync_ShouldReturnError_WhenInvalidFile()
        {
            // Arrange
            var stream = new MemoryStream();

            // Act
            var result = await _excelService.ReadFileAsync(stream);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task SaveMembersAndQualificationsAsync_ShouldSaveMembers_WhenValidRequest()
        {
            // Arrange
            var members = new List<MemberAndQualificationRequestModel>
            {
                new MemberAndQualificationRequestModel
                {
                    Name = "Test Name",
                    Email = "test@example.com",
                    CalendarId = 1,
                    InstructorLicense = 1,
                    LierLicense = 2,
                    StartingOfficerLicense = 3,
                    BarLicense = 4,
                    Scaling = 0.5
                }
            };

            _memberServiceMock.Setup(service => service.RemoveMemberByCalendarId(1)).ReturnsAsync(new ResultModel<IEnumerable<Member>> { IsSuccess = true });
            _memberServiceMock.Setup(service => service.CreateAsync(It.IsAny<MemberCreateRequestModel>())).ReturnsAsync(new ResultModel<Member> { IsSuccess = true, Value = new Member { Id = "1" } });
            _memberServiceMock.Setup(service => service.AddQualification(It.IsAny<MemberAddQualificationRequestModel>())).ReturnsAsync(new ResultModel<Member> { IsSuccess = true });

            // Act
            var result = await _excelService.SaveMembersAndQualificationsAsync(members);

            // Assert
            Assert.True(result);
            _memberServiceMock.Verify(service => service.RemoveMemberByCalendarId(1), Times.Once);
            _memberServiceMock.Verify(service => service.CreateAsync(It.IsAny<MemberCreateRequestModel>()), Times.Once);
            _memberServiceMock.Verify(service => service.AddQualification(It.IsAny<MemberAddQualificationRequestModel>()), Times.Once);
        }
    }
}
