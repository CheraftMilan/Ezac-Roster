using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Domain.Services;
using Ezac.Roster.Infrastructure.Models.PartOfDay;
using Ezac.Roster.Infrastructure.Models.Result;
using Moq;
using Xunit;

namespace Ezac.Roster.UnitTests.Services
{
    public class PartOfDayServiceTests
    {
        private readonly Mock<IPartOfDayRepository> _partOfDayRepositoryMock;
        private readonly PartOfDayService _partOfDayService;

        public PartOfDayServiceTests()
        {
            _partOfDayRepositoryMock = new Mock<IPartOfDayRepository>();
            _partOfDayService = new PartOfDayService(_partOfDayRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreatePartOfDay_WhenValidRequest()
        {
            // Arrange
            var newPartOfDayRequest = new PartOfDayCreateRequestModel
            {
                DayId = 1,
                Name = "Morning",
                StartTime = "08:00",
                EndTime = "12:00"
            };

            _partOfDayRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<PartOfDay>())).ReturnsAsync(true);

            // Act
            var result = await _partOfDayService.CreateAsync(newPartOfDayRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Morning", result.Value.Name);
            Assert.Equal("08:00", result.Value.StartTime);
            Assert.Equal("12:00", result.Value.EndTime);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenInvalidRequest()
        {
            // Arrange
            var newPartOfDayRequest = new PartOfDayCreateRequestModel
            {
                DayId = 1,
                Name = "Morning",
                StartTime = "08:00",
                EndTime = "12:00"
            };

            _partOfDayRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<PartOfDay>())).ReturnsAsync(false);

            // Act
            var result = await _partOfDayService.CreateAsync(newPartOfDayRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenPartOfDayNotFound()
        {
            // Arrange
            var partOfDayId = 1;
            _partOfDayRepositoryMock.Setup(repo => repo.GetByIdAsync(partOfDayId)).ReturnsAsync((PartOfDay)null);

            // Act
            var result = await _partOfDayService.DeleteAsync(partOfDayId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeletePartOfDay_WhenValidRequest()
        {
            // Arrange
            var partOfDayId = 1;
            var partOfDay = new PartOfDay { Id = partOfDayId, Name = "Morning" };
            _partOfDayRepositoryMock.Setup(repo => repo.GetByIdAsync(partOfDayId)).ReturnsAsync(partOfDay);
            _partOfDayRepositoryMock.Setup(repo => repo.DeleteAsync(partOfDay)).ReturnsAsync(true);

            // Act
            var result = await _partOfDayService.DeleteAsync(partOfDayId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_OnInvalidRequest()
        {
            // Arrange
            var partOfDayId = 1;
            var partOfDay = new PartOfDay { Id = partOfDayId, Name = "Morning" };
            _partOfDayRepositoryMock.Setup(repo => repo.GetByIdAsync(partOfDayId)).ReturnsAsync(partOfDay);
            _partOfDayRepositoryMock.Setup(repo => repo.DeleteAsync(partOfDay)).ReturnsAsync(false);

            // Act
            var result = await _partOfDayService.DeleteAsync(partOfDayId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPartOfDays_WhenPartOfDaysExist()
        {
            // Arrange
            var partOfDays = new List<PartOfDay>
            {
                new PartOfDay { Id = 1, Name = "Morning" },
                new PartOfDay { Id = 2, Name = "Afternoon" }
            };
            _partOfDayRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(partOfDays);

            // Act
            var result = await _partOfDayService.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenPartOfDaysDoNotExist()
        {
            // Arrange
            var partOfDays = new List<PartOfDay>();
            _partOfDayRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(partOfDays);

            // Act
            var result = await _partOfDayService.GetAllAsync();

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPartOfDay_WhenPartOfDayExists()
        {
            // Arrange
            var partOfDayId = 1;
            var partOfDay = new PartOfDay { Id = partOfDayId, Name = "Test PartOfDay" };
            _partOfDayRepositoryMock.Setup(repo => repo.GetByIdAsync(partOfDayId)).ReturnsAsync(partOfDay);

            // Act
            var result = await _partOfDayService.GetByIdAsync(partOfDayId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(partOfDay, result.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenPartOfDayNotFound()
        {
            // Arrange
            var partOfDayId = 1;
            _partOfDayRepositoryMock.Setup(repo => repo.GetByIdAsync(partOfDayId)).ReturnsAsync((PartOfDay)null);

            // Act
            var result = await _partOfDayService.GetByIdAsync(partOfDayId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenPartOfDayNotFound()
        {
            // Arrange
            var partOfDayUpdateRequest = new PartOfDayUpdateRequestModel
            {
                Id = 1,
                Name = "Updated PartOfDay",
                StartTime = "08:00",
                EndTime = "12:00",
                IsAvailable = true,
                ServicePreferenceId = 1
            };
            _partOfDayRepositoryMock.Setup(repo => repo.GetByIdAsync(partOfDayUpdateRequest.Id)).ReturnsAsync((PartOfDay)null);

            // Act
            var result = await _partOfDayService.UpdateAsync(partOfDayUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePartOfDay_WhenValidRequest()
        {
            // Arrange
            var partOfDayId = 1;
            var existingPartOfDay = new PartOfDay { Id = partOfDayId, Name = "Existing PartOfDay", StartTime = "08:00", EndTime = "12:00", IsAvailable = false, ServicePreferenceId = 0 };
            var partOfDayUpdateRequest = new PartOfDayUpdateRequestModel
            {
                Id = partOfDayId,
                Name = "Updated PartOfDay",
                StartTime = "08:00",
                EndTime = "12:00",
                IsAvailable = true,
                ServicePreferenceId = 1
            };
            _partOfDayRepositoryMock.Setup(repo => repo.GetByIdAsync(partOfDayId)).ReturnsAsync(existingPartOfDay);
            _partOfDayRepositoryMock.Setup(repo => repo.UpdateAsync(existingPartOfDay)).ReturnsAsync(true);

            // Act
            var result = await _partOfDayService.UpdateAsync(partOfDayUpdateRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Updated PartOfDay", existingPartOfDay.Name);
            Assert.Equal("08:00", existingPartOfDay.StartTime);
            Assert.Equal("12:00", existingPartOfDay.EndTime);
            Assert.True(existingPartOfDay.IsAvailable);
            Assert.Equal(1, existingPartOfDay.ServicePreferenceId);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenUpdateFails()
        {
            // Arrange
            var partOfDayId = 1;
            var existingPartOfDay = new PartOfDay { Id = partOfDayId, Name = "Existing PartOfDay", StartTime = "08:00", EndTime = "12:00", IsAvailable = false, ServicePreferenceId = 0 };
            var partOfDayUpdateRequest = new PartOfDayUpdateRequestModel
            {
                Id = partOfDayId,
                Name = "Updated PartOfDay",
                StartTime = "08:00",
                EndTime = "12:00",
                IsAvailable = true,
                ServicePreferenceId = 1
            };
            _partOfDayRepositoryMock.Setup(repo => repo.GetByIdAsync(partOfDayId)).ReturnsAsync(existingPartOfDay);
            _partOfDayRepositoryMock.Setup(repo => repo.UpdateAsync(existingPartOfDay)).ReturnsAsync(false);

            // Act
            var result = await _partOfDayService.UpdateAsync(partOfDayUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}
