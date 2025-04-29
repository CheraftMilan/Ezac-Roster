using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Domain.Services;
using Ezac.Roster.Infrastructure.Models.Service;
using Ezac.Roster.Infrastructure.Models.Result;
using Moq;
using Xunit;
using Ezac.Roster.Infrastructure.Interfaces;

namespace Ezac.Roster.UnitTests.Services
{
    public class ServiceServiceTests
    {
        private readonly Mock<IServiceRepository> _serviceRepositoryMock;
        private readonly Mock<IPartOfDayRepository> _partOfDayRepositoryMock;
        private readonly IServiceService _serviceService;
        private readonly Mock<IQualificationRepository> _qualificationRepositoryMock;

        public ServiceServiceTests()
        {
            _serviceRepositoryMock = new Mock<IServiceRepository>();
            _partOfDayRepositoryMock = new Mock<IPartOfDayRepository>();
            _qualificationRepositoryMock = new Mock<IQualificationRepository>();
            
            _serviceService = new ServiceService(_serviceRepositoryMock.Object, _partOfDayRepositoryMock.Object, _qualificationRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenServiceNameAlreadyExists()
        {
            // Arrange
            var existingServices = new List<Service>
            {
                new Service { Id = 1, Name = "Existing Service" }
            };
            _serviceRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingServices.AsQueryable());

            var newServiceRequest = new ServiceCreateRequestModel
            {
                Name = "Existing Service"
            };

            // Act
            var result = await _serviceService.CreateAsync(newServiceRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateService_WhenValidRequest()
        {
            // Arrange
            var existingServices = new List<Service>();
            _serviceRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingServices.AsQueryable());

            var newServiceRequest = new ServiceCreateRequestModel
            {
                Name = "New Service"
            };

            _serviceRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Service>())).ReturnsAsync(true);

            // Act
            var result = await _serviceService.CreateAsync(newServiceRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("New Service", result.Value.Name);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenInvalidRequest()
        {
            // Arrange
            var newServiceRequest = new ServiceCreateRequestModel
            {
                Name = "New Service"
            };

            _serviceRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Service>())).ReturnsAsync(false);

            // Act
            var result = await _serviceService.CreateAsync(newServiceRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenServiceNotFound()
        {
            // Arrange
            var serviceId = 1;
            _serviceRepositoryMock.Setup(repo => repo.GetByIdAsync(serviceId)).ReturnsAsync((Service)null);

            // Act
            var result = await _serviceService.DeleteAsync(serviceId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteService_WhenValidRequest()
        {
            // Arrange
            var serviceId = 1;
            var service = new Service { Id = serviceId, Name = "Existing Service" };
            _serviceRepositoryMock.Setup(repo => repo.GetByIdAsync(serviceId)).ReturnsAsync(service);
            _serviceRepositoryMock.Setup(repo => repo.DeleteAsync(service)).ReturnsAsync(true);

            // Act
            var result = await _serviceService.DeleteAsync(serviceId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_OnInvalidRequest()
        {
            // Arrange
            var serviceId = 1;
            var service = new Service { Id = serviceId, Name = "Existing Service" };
            _serviceRepositoryMock.Setup(repo => repo.GetByIdAsync(serviceId)).ReturnsAsync(service);
            _serviceRepositoryMock.Setup(repo => repo.DeleteAsync(service)).ReturnsAsync(false);

            // Act
            var result = await _serviceService.DeleteAsync(serviceId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnServices_WhenServicesExist()
        {
            // Arrange
            var services = new List<Service>
            {
                new Service { Id = 1, Name = "Service 1" },
                new Service { Id = 2, Name = "Service 2" }
            };
            _serviceRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(services);

            // Act
            var result = await _serviceService.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenServicesDoNotExist()
        {
            // Arrange
            var services = new List<Service>();
            _serviceRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(services);

            // Act
            var result = await _serviceService.GetAllAsync();

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnService_WhenServiceExists()
        {
            // Arrange
            var serviceId = 1;
            var service = new Service { Id = serviceId, Name = "Test Service" };
            _serviceRepositoryMock.Setup(repo => repo.GetByIdAsync(serviceId)).ReturnsAsync(service);

            // Act
            var result = await _serviceService.GetByIdAsync(serviceId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(service, result.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenServiceNotFound()
        {
            // Arrange
            var serviceId = 1;
            _serviceRepositoryMock.Setup(repo => repo.GetByIdAsync(serviceId)).ReturnsAsync((Service)null);

            // Act
            var result = await _serviceService.GetByIdAsync(serviceId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenServiceNotFound()
        {
            // Arrange
            var serviceUpdateRequest = new ServiceUpdateRequestModel
            {
                Id = 1,
                Name = "Updated Service"
            };
            _serviceRepositoryMock.Setup(repo => repo.GetByIdAsync(serviceUpdateRequest.Id)).ReturnsAsync((Service)null);

            // Act
            var result = await _serviceService.UpdateAsync(serviceUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateService_WhenValidRequest()
        {
            // Arrange
            var serviceId = 1;
            var existingService = new Service { Id = serviceId, Name = "Existing Service" };
            var serviceUpdateRequest = new ServiceUpdateRequestModel
            {
                Id = serviceId,
                Name = "Updated Service"
            };
            _serviceRepositoryMock.Setup(repo => repo.GetByIdAsync(serviceId)).ReturnsAsync(existingService);
            _serviceRepositoryMock.Setup(repo => repo.UpdateAsync(existingService)).ReturnsAsync(true);

            // Act
            var result = await _serviceService.UpdateAsync(serviceUpdateRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Updated Service", existingService.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenDuplicateNameExists()
        {
            // Arrange
            var serviceId = 1;
            var existingServices = new List<Service>
            {
                new Service { Id = 1, Name = "Existing Service 1" },
                new Service { Id = 2, Name = "Updated Service" }
            };
            var serviceUpdateRequest = new ServiceUpdateRequestModel
            {
                Id = serviceId,
                Name = "Updated Service"
            };
            _serviceRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingServices.AsQueryable());
            _serviceRepositoryMock.Setup(repo => repo.GetByIdAsync(serviceId)).ReturnsAsync(existingServices[0]);

            // Act
            var result = await _serviceService.UpdateAsync(serviceUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenUpdateFails()
        {
            // Arrange
            var serviceId = 1;
            var existingService = new Service { Id = serviceId, Name = "Existing Service" };
            var serviceUpdateRequest = new ServiceUpdateRequestModel
            {
                Id = serviceId,
                Name = "Updated Service"
            };
            _serviceRepositoryMock.Setup(repo => repo.GetByIdAsync(serviceId)).ReturnsAsync(existingService);
            _serviceRepositoryMock.Setup(repo => repo.UpdateAsync(existingService)).ReturnsAsync(false);

            // Act
            var result = await _serviceService.UpdateAsync(serviceUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}
