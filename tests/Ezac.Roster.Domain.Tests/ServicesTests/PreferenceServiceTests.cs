using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Domain.Models.Preference;
using Ezac.Roster.Domain.Services;
using Ezac.Roster.Infrastructure.Models.Result;
using Moq;
using Xunit;

namespace Ezac.Roster.UnitTests.Services
{
    public class PreferenceServiceTests
    {
        private readonly Mock<IPreferenceRepository> _preferenceRepositoryMock;
        private readonly PreferenceService _preferenceService;

        public PreferenceServiceTests()
        {
            _preferenceRepositoryMock = new Mock<IPreferenceRepository>();
            _preferenceService = new PreferenceService(_preferenceRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreatePreference_WhenValidRequest()
        {
            // Arrange
            var newPreferenceRequest = new PreferenceCreateRequestModel
            {
                MemberId = "1",
                ServiceId = 1,
                PartOfDayId = 1
            };

            _preferenceRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Preference>())).ReturnsAsync(true);

            // Act
            var result = await _preferenceService.CreateAsync(newPreferenceRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("1", result.Value.MemberId);
            Assert.Equal(1, result.Value.ServiceId);
            Assert.Equal(1, result.Value.PartOfDayId);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenInvalidRequest()
        {
            // Arrange
            var newPreferenceRequest = new PreferenceCreateRequestModel
            {
                MemberId = "1",
                ServiceId = 1,
                PartOfDayId = 1
            };

            _preferenceRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Preference>())).ReturnsAsync(false);

            // Act
            var result = await _preferenceService.CreateAsync(newPreferenceRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAllAsync_ShouldDeletePreferences_WhenValidRequest()
        {
            // Arrange
            var preferencesToDelete = new List<int> { 1, 2, 3 };
            var preferenceDeleteRequest = new PreferenceDeleteRequestModel { PreferenceIds = preferencesToDelete };

            _preferenceRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Preference());
            _preferenceRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Preference>())).ReturnsAsync(true);

            // Act
            var result = await _preferenceService.DeleteAllAsync(preferenceDeleteRequest);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAllAsync_ShouldReturnError_WhenPreferencesNotFound()
        {
            // Arrange
            var preferencesToDelete = new List<int> { 1, 2, 3 };
            var preferenceDeleteRequest = new PreferenceDeleteRequestModel { PreferenceIds = preferencesToDelete };

            _preferenceRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Preference)null);

            // Act
            var result = await _preferenceService.DeleteAllAsync(preferenceDeleteRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAllAsync_ShouldReturnError_WhenDeleteFails()
        {
            // Arrange
            var preferencesToDelete = new List<int> { 1, 2, 3 };
            var preferenceDeleteRequest = new PreferenceDeleteRequestModel { PreferenceIds = preferencesToDelete };

            _preferenceRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Preference());
            _preferenceRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Preference>())).ReturnsAsync(false);

            // Act
            var result = await _preferenceService.DeleteAllAsync(preferenceDeleteRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPreferences_WhenPreferencesExist()
        {
            // Arrange
            var preferences = new List<Preference>
            {
                new Preference { Id = 1, MemberId = "1", ServiceId = 1, PartOfDayId = 1 },
                new Preference { Id = 2, MemberId = "2", ServiceId = 2, PartOfDayId = 2 }
            };
            _preferenceRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(preferences);

            // Act
            var result = await _preferenceService.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenPreferencesDoNotExist()
        {
            // Arrange
            var preferences = new List<Preference>();
            _preferenceRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(preferences);

            // Act
            var result = await _preferenceService.GetAllAsync();

            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}
