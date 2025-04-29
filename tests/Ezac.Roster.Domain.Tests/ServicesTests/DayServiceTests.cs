using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Domain.Services;
using Ezac.Roster.Infrastructure.Models.Day;
using Ezac.Roster.Infrastructure.Models.Result;
using Moq;
using Xunit;

namespace Ezac.Roster.UnitTests.Services
{
    public class DayServiceTests
    {
        private readonly Mock<IDayRepository> _dayRepositoryMock;
        private readonly DayService _dayService;

        public DayServiceTests()
        {
            _dayRepositoryMock = new Mock<IDayRepository>();
            _dayService = new DayService(_dayRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateDay_WhenValidRequest()
        {
            // Arrange
            var newDayRequest = new DayCreateRequestModel
            {
                Description = "Test Description",
                Date = DateTime.Today,
                CalendarId = 1
            };

            _dayRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Day>())).ReturnsAsync(true);

            // Act
            var result = await _dayService.CreateAsync(newDayRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Test Description", result.Value.Description);
            Assert.Equal(DateTime.Today, result.Value.Date);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenExceptionThrown()
        {
            // Arrange
            var newDayRequest = new DayCreateRequestModel
            {
                Description = "Test Description",
                Date = DateTime.Today,
                CalendarId = 1
            };

            _dayRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Day>())).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _dayService.CreateAsync(newDayRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Test Exception", result.Errors);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenDayNotFound()
        {
            // Arrange
            var dayId = 1;
            _dayRepositoryMock.Setup(repo => repo.GetByIdAsync(dayId)).ReturnsAsync((Day)null);

            // Act
            var result = await _dayService.DeleteAsync(dayId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Deze dag werd niet gevonden.", result.Errors);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteDay_WhenValidRequest()
        {
            // Arrange
            var dayId = 1;
            var day = new Day { Id = dayId, Description = "Existing Day" };
            _dayRepositoryMock.Setup(repo => repo.GetByIdAsync(dayId)).ReturnsAsync(day);
            _dayRepositoryMock.Setup(repo => repo.DeleteAsync(day)).ReturnsAsync(true);

            // Act
            var result = await _dayService.DeleteAsync(dayId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_OnInvalidRequest()
        {
            // Arrange
            var dayId = 1;
            var day = new Day { Id = dayId, Description = "Existing Day" };
            _dayRepositoryMock.Setup(repo => repo.GetByIdAsync(dayId)).ReturnsAsync(day);
            _dayRepositoryMock.Setup(repo => repo.DeleteAsync(day)).ReturnsAsync(false);

            // Act
            var result = await _dayService.DeleteAsync(dayId);


            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Het verwijderen van deze dag is mislukt.", result.Errors);
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnDays_WhenDaysExist()
        {
            // Arrange
            var days = new List<Day>
            {
                new Day { Id = 1, Description = "Day 1" },
                new Day { Id = 2, Description = "Day 2" }
            };
            _dayRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(days);

            // Act
            var result = await _dayService.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenDaysDoNotExist()
        {
            // Arrange
            var days = new List<Day>();
            _dayRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(days);

            // Act
            var result = await _dayService.GetAllAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Er werden geen dagen gevonden.", result.Errors);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDay_WhenDayExists()
        {
            // Arrange
            var dayId = 1;
            var day = new Day { Id = dayId, Description = "Test Day" };
            _dayRepositoryMock.Setup(repo => repo.GetByIdAsync(dayId)).ReturnsAsync(day);

            // Act
            var result = await _dayService.GetByIdAsync(dayId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(day, result.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenDayNotFound()
        {
            // Arrange
            var dayId = 1;
            _dayRepositoryMock.Setup(repo => repo.GetByIdAsync(dayId)).ReturnsAsync((Day)null);

            // Act
            var result = await _dayService.GetByIdAsync(dayId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Deze dag werd niet gevonden.", result.Errors);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenDayNotFound()
        {
            // Arrange
            var dayUpdateRequest = new DayUpdateRequestModel
            {
                Id = 1,
                Description = "Updated Description",
                Date = DateTime.Today,
                CalendarId = 1
            };
            _dayRepositoryMock.Setup(repo => repo.GetByIdAsync(dayUpdateRequest.Id)).ReturnsAsync((Day)null);

            // Act
            var result = await _dayService.UpdateAsync(dayUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Deze dag werd niet gevonden.", result.Errors);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDay_WhenValidRequest()
        {
            // Arrange
            var dayId = 1;
            var existingDay = new Day { Id = dayId, Description = "Existing Description", Date = DateTime.Today.AddDays(-1), CalendarId = 1 };
            var dayUpdateRequest = new DayUpdateRequestModel
            {
                Id = dayId,
                Description = "Updated Description",
                Date = DateTime.Today,
                CalendarId = 1
            };
            _dayRepositoryMock.Setup(repo => repo.GetByIdAsync(dayId)).ReturnsAsync(existingDay);
            _dayRepositoryMock.Setup(repo => repo.UpdateAsync(existingDay)).ReturnsAsync(true);

            // Act
            var result = await _dayService.UpdateAsync(dayUpdateRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Updated Description", existingDay.Description);
            Assert.Equal(DateTime.Today, existingDay.Date);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenUpdateFails()
        {
            // Arrange
            var dayId = 1;
            var existingDay = new Day { Id = dayId, Description = "Existing Description", Date = DateTime.Today.AddDays(-1), CalendarId = 1 };
            var dayUpdateRequest = new DayUpdateRequestModel
            {
                Id = dayId,
                Description = "Updated Description",
                Date = DateTime.Today,
                CalendarId = 1
            };
            _dayRepositoryMock.Setup(repo => repo.GetByIdAsync(dayId)).ReturnsAsync(existingDay);
            _dayRepositoryMock.Setup(repo => repo.UpdateAsync(existingDay)).ReturnsAsync(false);

            // Act
            var result = await _dayService.UpdateAsync(dayUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Het updaten van deze dag is mislukt.", result.Errors);
        }
    }
}
