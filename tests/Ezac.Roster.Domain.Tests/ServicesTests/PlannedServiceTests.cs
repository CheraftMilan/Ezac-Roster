using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Domain.Services;
using Ezac.Roster.Infrastructure.Models.PlannedService;
using Ezac.Roster.Infrastructure.Models.Result;
using Moq;
using Xunit;

namespace Ezac.Roster.UnitTests.Services
{
    public class PlannedServicesTests
    {
        private readonly Mock<IPlannedServiceRepository> _plannedServiceRepositoryMock;
        private readonly Mock<ICalendarRepository> _calendarRepositoryMock;
        private readonly PlannedServices _plannedServices;

        public PlannedServicesTests()
        {
            _plannedServiceRepositoryMock = new Mock<IPlannedServiceRepository>();
            _calendarRepositoryMock = new Mock<ICalendarRepository>();
            _plannedServices = new PlannedServices(_plannedServiceRepositoryMock.Object, _calendarRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreatePlannedService_WhenValidRequest()
        {
            // Arrange
            var newPlannedServiceRequest = new PlannedServiceCreateRequestModel
            {
                PartOfDayId = 1,
                ServiceId = 1,
                MemberId = "1",
                Weight = 1
            };

            _plannedServiceRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<PlannedService>())).ReturnsAsync(true);

            // Act
            var result = await _plannedServices.CreateAsync(newPlannedServiceRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.PartOfDayId);
            Assert.Equal(1, result.Value.ServiceId);
            Assert.Equal("1", result.Value.MemberId);
            Assert.Equal(1, result.Value.Weight);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenInvalidRequest()
        {
            // Arrange
            var newPlannedServiceRequest = new PlannedServiceCreateRequestModel
            {
                PartOfDayId = 1,
                ServiceId = 1,
                MemberId = "1",
                Weight = 1
            };

            _plannedServiceRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<PlannedService>())).ReturnsAsync(false);

            // Act
            var result = await _plannedServices.CreateAsync(newPlannedServiceRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenPlannedServiceNotFound()
        {
            // Arrange
            var plannedServiceId = 1;
            _plannedServiceRepositoryMock.Setup(repo => repo.GetByIdAsync(plannedServiceId)).ReturnsAsync((PlannedService)null);

            // Act
            var result = await _plannedServices.DeleteAsync(plannedServiceId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeletePlannedService_WhenValidRequest()
        {
            // Arrange
            var plannedServiceId = 1;
            var plannedService = new PlannedService { Id = plannedServiceId, PartOfDayId = 1, ServiceId = 1, MemberId = "1", Weight = 1 };
            _plannedServiceRepositoryMock.Setup(repo => repo.GetByIdAsync(plannedServiceId)).ReturnsAsync(plannedService);
            _plannedServiceRepositoryMock.Setup(repo => repo.DeleteAsync(plannedService)).ReturnsAsync(true);

            // Act
            var result = await _plannedServices.DeleteAsync(plannedServiceId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_OnInvalidRequest()
        {
            // Arrange
            var plannedServiceId = 1;
            var plannedService = new PlannedService { Id = plannedServiceId, PartOfDayId = 1, ServiceId = 1, MemberId = "1", Weight = 1 };
            _plannedServiceRepositoryMock.Setup(repo => repo.GetByIdAsync(plannedServiceId)).ReturnsAsync(plannedService);
            _plannedServiceRepositoryMock.Setup(repo => repo.DeleteAsync(plannedService)).ReturnsAsync(false);

            // Act
            var result = await _plannedServices.DeleteAsync(plannedServiceId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPlannedServices_WhenPlannedServicesExist()
        {
            // Arrange
            var plannedServices = new List<PlannedService>
            {
                new PlannedService { Id = 1, PartOfDayId = 1, ServiceId = 1, MemberId = "1", Weight = 1 },
                new PlannedService { Id = 2, PartOfDayId = 2, ServiceId = 2, MemberId = "2", Weight = 2 }
            };
            _plannedServiceRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(plannedServices);

            // Act
            var result = await _plannedServices.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenPlannedServicesDoNotExist()
        {
            // Arrange
            var plannedServices = new List<PlannedService>();
            _plannedServiceRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(plannedServices);

            // Act
            var result = await _plannedServices.GetAllAsync();

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPlannedService_WhenPlannedServiceExists()
        {
            // Arrange
            var plannedServiceId = 1;
            var plannedService = new PlannedService { Id = plannedServiceId, PartOfDayId = 1, ServiceId = 1, MemberId = "1", Weight = 1 };
            _plannedServiceRepositoryMock.Setup(repo => repo.GetByIdAsync(plannedServiceId)).ReturnsAsync(plannedService);

            // Act
            var result = await _plannedServices.GetByIdAsync(plannedServiceId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(plannedService, result.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenPlannedServiceNotFound()
        {
            // Arrange
            var plannedServiceId = 1;
            _plannedServiceRepositoryMock.Setup(repo => repo.GetByIdAsync(plannedServiceId)).ReturnsAsync((PlannedService)null);

            // Act
            var result = await _plannedServices.GetByIdAsync(plannedServiceId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenPlannedServiceNotFound()
        {
            // Arrange
            var plannedServiceUpdateRequest = new PlannedServiceUpdateRequestModel
            {
                Id = 1,
                PartOfDayId = 1,
                ServiceId = 1,
                MemberId = "1",
                Weight = 1
            };
            _plannedServiceRepositoryMock.Setup(repo => repo.GetByIdAsync(plannedServiceUpdateRequest.Id)).ReturnsAsync((PlannedService)null);

            // Act
            var result = await _plannedServices.UpdateAsync(plannedServiceUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePlannedService_WhenValidRequest()
        {
            // Arrange
            var plannedServiceId = 1;
            var existingPlannedService = new PlannedService { Id = plannedServiceId, PartOfDayId = 1, ServiceId = 1, MemberId = "1", Weight = 1 };
            var plannedServiceUpdateRequest = new PlannedServiceUpdateRequestModel
            {
                Id = plannedServiceId,
                PartOfDayId = 2,
                ServiceId = 2,
                MemberId = "2",
                Weight = 2
            };
            _plannedServiceRepositoryMock.Setup(repo => repo.GetByIdAsync(plannedServiceId)).ReturnsAsync(existingPlannedService);
            _plannedServiceRepositoryMock.Setup(repo => repo.UpdateAsync(existingPlannedService)).ReturnsAsync(true);

            // Act
            var result = await _plannedServices.UpdateAsync(plannedServiceUpdateRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, existingPlannedService.PartOfDayId);
            Assert.Equal(2, existingPlannedService.ServiceId);
            Assert.Equal("2", existingPlannedService.MemberId);
            Assert.Equal(2, existingPlannedService.Weight);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenUpdateFails()
        {
            // Arrange
            var plannedServiceId = 1;
            var existingPlannedService = new PlannedService { Id = plannedServiceId, PartOfDayId = 1, ServiceId = 1, MemberId = "1", Weight = 1 };
            var plannedServiceUpdateRequest = new PlannedServiceUpdateRequestModel
            {
                Id = plannedServiceId,
                PartOfDayId = 2,
                ServiceId = 2,
                MemberId = "2",
                Weight = 2
            };
            _plannedServiceRepositoryMock.Setup(repo => repo.GetByIdAsync(plannedServiceId)).ReturnsAsync(existingPlannedService);
            _plannedServiceRepositoryMock.Setup(repo => repo.UpdateAsync(existingPlannedService)).ReturnsAsync(false);

            // Act
            var result = await _plannedServices.UpdateAsync(plannedServiceUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}
