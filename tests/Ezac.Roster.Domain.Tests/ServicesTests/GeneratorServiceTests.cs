using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Services;
using Ezac.Roster.Domain.Services;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.PlannedService;
using Ezac.Roster.Infrastructure.Models.Result;
using Moq;
using Xunit;

namespace Ezac.Roster.UnitTests.Services
{
    public class GeneratorServiceTests
    {
        private readonly Mock<ICalendarService> _calendarServiceMock;
        private readonly Mock<IPreferenceService> _preferenceServiceMock;
        private readonly Mock<IPlannedServiceService> _plannedServiceServiceMock;
        private readonly Mock<IServiceService> _serviceServiceMock;
        private readonly Mock<IPartOfDayService> _partOfDayServiceMock;
        private readonly Mock<IMemberService> _memberServiceMock;
        private readonly GeneratorService _generatorService;

        public GeneratorServiceTests()
        {
            _calendarServiceMock = new Mock<ICalendarService>();
            _preferenceServiceMock = new Mock<IPreferenceService>();
            _plannedServiceServiceMock = new Mock<IPlannedServiceService>();
            _serviceServiceMock = new Mock<IServiceService>();
            _partOfDayServiceMock = new Mock<IPartOfDayService>();
            _memberServiceMock = new Mock<IMemberService>();
            _generatorService = new GeneratorService(
                _calendarServiceMock.Object,
                _preferenceServiceMock.Object,
                _plannedServiceServiceMock.Object,
                _serviceServiceMock.Object,
                _partOfDayServiceMock.Object,
                _memberServiceMock.Object
            );
        }

        [Fact]
        public async Task GenerateCalendar_ShouldReturnError_WhenCalendarNotFound()
        {
            // Arrange
            var calendarId = 1;
            _calendarServiceMock.Setup(service => service.GetByIdAsync(calendarId))
                .ReturnsAsync(new ResultModel<Calendar> { IsSuccess = false });

            // Act
            var result = await _generatorService.GenerateCalendar(calendarId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }
        [Fact]
        public async Task GenerateCalendar_ShouldGeneratePlannedServices_WhenValidRequest()
        {
            // Arrange
            var calendarId = 1;
            var calendar = new Calendar
            {
                Id = calendarId,
                Days = new List<Day>
                {
                    new Day
                    {
                        Id = 1,
                        PartOfDays = new List<PartOfDay>
                        {
                            new PartOfDay
                            {
                                Id = 1,
                                Services = new List<Service>
                                {
                                    new Service { Id = 1, Weight = 100 }
                                }
                            }
                        }
                    }
                },
                Members = new List<Member>
                {
                    new Member { Id = "1", Scaling = 1, TotalWeight = 0, Preferences = new List<Preference>() }
                }
            };

            var members = new List<Member>
            {
                new Member { Id = "1", Scaling = 1, TotalWeight = 0, Preferences = new List<Preference>() }
            };

            var partOfDay = new PartOfDay
            {
                Id = 1,
                Services = new List<Service>
                {
                    new Service { Id = 1, Weight = 100 }
                }
            };

            var service = new Service
            {
                Id = 1,
                Weight = 100,
                RequiredQualifications = new List<Qualification>()
            };

            var preferences = new List<Preference>()
            {
                new Preference
                {
                    MemberId = "1",
                    PartOfDayId = 1,
                    ServiceId = 1,
                    CalendarId = 1
                }
            };

            _calendarServiceMock.Setup(service => service.GetByIdAsync(calendarId))
                .ReturnsAsync(new ResultModel<Calendar> { IsSuccess = true, Value = calendar });

            _memberServiceMock.Setup(service => service.GetAllAsync())
                .ReturnsAsync(new ResultModel<IEnumerable<Member>> { IsSuccess = true, Value = members });

            _plannedServiceServiceMock.Setup(service => service.CreateAsync(It.IsAny<PlannedServiceCreateRequestModel>()))
                .ReturnsAsync(new ResultModel<PlannedService> { IsSuccess = true });

            _partOfDayServiceMock.Setup(service => service.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new ResultModel<PartOfDay> { IsSuccess = true, Value = partOfDay });

            _serviceServiceMock.Setup(service => service.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new ResultModel<Service> { IsSuccess = true, Value = service });

            _preferenceServiceMock.Setup(service => service.GetAllAsync())
                .ReturnsAsync(new ResultModel<IEnumerable<Preference>> { IsSuccess = true, Value = preferences });

            // Act
            var result = await _generatorService.GenerateCalendar(calendarId);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}

