using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Domain.Services;
using Ezac.Roster.Infrastructure.Models.Calendar;
using Ezac.Roster.Infrastructure.Models.Result;
using Moq;
using Xunit;

namespace Ezac.Roster.UnitTests.Services
{
    public class CalendarServiceTests
    {
        private readonly Mock<ICalendarRepository> _calendarRepositoryMock;
        private readonly CalendarService _calendarService;

        public CalendarServiceTests()
        {
            _calendarRepositoryMock = new Mock<ICalendarRepository>();
            _calendarService = new CalendarService(_calendarRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenCalendarNameAlreadyExists()
        {
            // Arrange
            var existingCalendars = new List<Calendar>
            {
                new Calendar { Id = 1, Name = "Existing Calendar" }
            };
            _calendarRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingCalendars.AsQueryable());

            var newCalendarRequest = new CalendarCreateRequestModel
            {
                Name = "Existing Calendar",
                Description = "Test Description"
            };


            // Act
            var result = await _calendarService.CreateAsync(newCalendarRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Deze kalender bestaat al.", result.Errors);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateCalendar_WhenValidRequest()
        {
            // Arrange
            var existingCalendars = new List<Calendar>();
            _calendarRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingCalendars.AsQueryable());

            var newCalendarRequest = new CalendarCreateRequestModel
            {
                Name = "New Calendar",
                Description = "Test Description"
            };

            _calendarRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Calendar>())).ReturnsAsync(true);

            // Act
            var result = await _calendarService.CreateAsync(newCalendarRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("New Calendar", result.Value.Name);
            Assert.Equal("Test Description", result.Value.Description);
        }

        [Fact]

        public async Task CreateAsync_ShouldReturnError_WhenInvalidRequest()
        {
            // Arrange
            var newCalendarRequest = new CalendarCreateRequestModel
            {
                Name = "New Calendar",
                Description = "Test Description"
            };

            _calendarRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Calendar>())).ReturnsAsync(false);

            // Act
            var result = await _calendarService.CreateAsync(newCalendarRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Het aanmaken van de kalender is mislukt.", result.Errors);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenCalendarNotFound()
        {
            // Arrange
            var calendarId = 1;
            _calendarRepositoryMock.Setup(repo => repo.GetByIdAsync(calendarId)).ReturnsAsync((Calendar)null);

            // Act
            var result = await _calendarService.DeleteAsync(calendarId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Deze kalender werd niet gevonden.", result.Errors);
        }

        [Fact]

        public async Task DeleteAsync_ShouldDeleteCalendar_WhenValidRequest()
        {
            // Arrange
            var calendarId = 1;
            var calendar = new Calendar { Id = calendarId, Name = "Existing Calendar" };
            _calendarRepositoryMock.Setup(repo => repo.GetByIdAsync(calendarId)).ReturnsAsync(calendar);
            _calendarRepositoryMock.Setup(repo => repo.DeleteAsync(calendar)).ReturnsAsync(true);

            // Act
            var result = await _calendarService.DeleteAsync(calendarId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]

        public async Task DeleteAsync_ShouldReturnError_OnInvalidRequest()
        {
            // Arrange
            var calendarId = 1;
            var calendar = new Calendar { Id = calendarId, Name = "Existing Calendar" };
            _calendarRepositoryMock.Setup(repo => repo.GetByIdAsync(calendarId)).ReturnsAsync(calendar);
            _calendarRepositoryMock.Setup(repo => repo.DeleteAsync(calendar)).ReturnsAsync(false);

            // Act
            var result = await _calendarService.DeleteAsync(calendarId);

            Assert.False(result.IsSuccess);
        }

        [Fact]

        public async Task GetAllAsync_ShouldReturnCalendars_WhenCalendarsExist()
        {

            //arrange

            var calendars = new List<Calendar>
            {
                new Calendar { Id = 1, Name = "Calendar 1" },
                new Calendar { Id = 2, Name = "Calendar 2" }
            };
            _calendarRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(calendars);

            //act

            var result = await _calendarService.GetAllAsync();

            //assert

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());

        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenCalendarsDoNotExist()
        {
            // Arrange
            var calendars = new List<Calendar>();
            _calendarRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(calendars);

            // Act
            var result = await _calendarService.GetAllAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Er werden geen kalenders gevonden.", result.Errors);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCalendar_WhenCalendarExists()
        {
            // Arrange
            var calendarId = 1;
            var calendar = new Calendar { Id = calendarId, Name = "Test Calendar" };
            _calendarRepositoryMock.Setup(repo => repo.GetByIdAsync(calendarId)).ReturnsAsync(calendar);

            // Act
            var result = await _calendarService.GetByIdAsync(calendarId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(calendar, result.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenCalendarNotFound()
        {
            // Arrange
            var calendarId = 1;
            _calendarRepositoryMock.Setup(repo => repo.GetByIdAsync(calendarId)).ReturnsAsync((Calendar)null);

            // Act
            var result = await _calendarService.GetByIdAsync(calendarId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Deze kalender werd niet gevonden.", result.Errors);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenCalendarNotFound()
        {
            // Arrange
            var calendarUpdateRequest = new CalendarUpdateRequestModel
            {
                Id = 1,
                Name = "Updated Calendar",
                Description = "Updated Description"
            };
            _calendarRepositoryMock.Setup(repo => repo.GetByIdAsync(calendarUpdateRequest.Id)).ReturnsAsync((Calendar)null);

            // Act
            var result = await _calendarService.UpdateAsync(calendarUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Deze kalender werd niet gevonden.", result.Errors);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCalendar_WhenValidRequest()
        {
            // Arrange
            var calendarId = 1;
            var existingCalendar = new Calendar { Id = calendarId, Name = "Existing Calendar", Description = "Existing Description" };
            var calendarUpdateRequest = new CalendarUpdateRequestModel
            {
                Id = calendarId,
                Name = "Updated Calendar",
                Description = "Updated Description"
            };
            _calendarRepositoryMock.Setup(repo => repo.GetByIdAsync(calendarId)).ReturnsAsync(existingCalendar);
            _calendarRepositoryMock.Setup(repo => repo.UpdateAsync(existingCalendar)).ReturnsAsync(true);

            // Act
            var result = await _calendarService.UpdateAsync(calendarUpdateRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Updated Calendar", existingCalendar.Name);
            Assert.Equal("Updated Description", existingCalendar.Description);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenDuplicateNameExists()
        {
            // Arrange
            var calendarId = 1;
            var existingCalendars = new List<Calendar>
            {
                new Calendar { Id = 1, Name = "Existing Calendar 1" },
                new Calendar { Id = 2, Name = "Updated Calendar" }
            };
            var calendarUpdateRequest = new CalendarUpdateRequestModel
            {
                Id = calendarId,
                Name = "Updated Calendar",
                Description = "Updated Description"
            };
            _calendarRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingCalendars.AsQueryable());
            _calendarRepositoryMock.Setup(repo => repo.GetByIdAsync(calendarId)).ReturnsAsync(existingCalendars[0]);

            // Act
            var result = await _calendarService.UpdateAsync(calendarUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Deze kalender bestaat al.", result.Errors);
        }
    }
}

