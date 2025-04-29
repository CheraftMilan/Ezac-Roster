using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Domain.Services;
using Ezac.Roster.Infrastructure.Models.Qualification;
using Ezac.Roster.Infrastructure.Models.Result;
using Moq;
using Xunit;

namespace Ezac.Roster.UnitTests.Services
{
    public class QualificationServiceTests
    {
        private readonly Mock<IQualificationRepository> _qualificationRepositoryMock;
        private readonly Mock<IServiceRepository> _serviceRepositoryMock;
        private readonly Mock<IMemberRepository> _memberRepositoryMock;
        private readonly QualificationService _qualificationService;

        public QualificationServiceTests()
        {
            _qualificationRepositoryMock = new Mock<IQualificationRepository>();
            _serviceRepositoryMock = new Mock<IServiceRepository>();
            _memberRepositoryMock = new Mock<IMemberRepository>();
            _qualificationService = new QualificationService(_qualificationRepositoryMock.Object, _serviceRepositoryMock.Object, _memberRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenQualificationNameAlreadyExists()
        {
            // Arrange
            var existingQualifications = new List<Qualification>
            {
                new Qualification { Id = 1, Name = "Existing Qualification" }
            };
            _qualificationRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingQualifications.AsQueryable());

            var newQualificationRequest = new QualificationCreateRequestModel
            {
                Name = "Existing Qualification"
            };

            // Act
            var result = await _qualificationService.CreateAsync(newQualificationRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateQualification_WhenValidRequest()
        {
            // Arrange
            var existingQualifications = new List<Qualification>();
            _qualificationRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingQualifications.AsQueryable());

            var existingServices = new List<Service>
            {
                new Service { Id = 1, Name = "Service1" },
                new Service { Id = 2, Name = "Service2" }
            };
            _serviceRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingServices.AsQueryable());

            var existingMembers = new List<Member>
            {
                new Member { Id = "1", Name = "Member1" }
            };
            _memberRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingMembers.AsQueryable());

            var newQualificationRequest = new QualificationCreateRequestModel
            {
                Name = "New Qualification",
                ServiceIds = new List<int> { 1, 2 },
                memberId = "1",
                Experience = 5
            };

            _qualificationRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Qualification>())).ReturnsAsync(true);

            // Act
            var result = await _qualificationService.CreateAsync(newQualificationRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("New Qualification", result.Value.Name);
        }


        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenInvalidRequest()
        {
            // Arrange
            var newQualificationRequest = new QualificationCreateRequestModel
            {
                Name = "New Qualification"
            };

            _qualificationRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Qualification>())).ReturnsAsync(false);

            // Act
            var result = await _qualificationService.CreateAsync(newQualificationRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenQualificationNotFound()
        {
            // Arrange
            var qualificationId = 1;
            _qualificationRepositoryMock.Setup(repo => repo.GetByIdAsync(qualificationId)).ReturnsAsync((Qualification)null);

            // Act
            var result = await _qualificationService.DeleteAsync(qualificationId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteQualification_WhenValidRequest()
        {
            // Arrange
            var qualificationId = 1;
            var qualification = new Qualification { Id = qualificationId, Name = "Existing Qualification" };
            _qualificationRepositoryMock.Setup(repo => repo.GetByIdAsync(qualificationId)).ReturnsAsync(qualification);
            _qualificationRepositoryMock.Setup(repo => repo.DeleteAsync(qualification)).ReturnsAsync(true);

            // Act
            var result = await _qualificationService.DeleteAsync(qualificationId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_OnInvalidRequest()
        {
            // Arrange
            var qualificationId = 1;
            var qualification = new Qualification { Id = qualificationId, Name = "Existing Qualification" };
            _qualificationRepositoryMock.Setup(repo => repo.GetByIdAsync(qualificationId)).ReturnsAsync(qualification);
            _qualificationRepositoryMock.Setup(repo => repo.DeleteAsync(qualification)).ReturnsAsync(false);

            // Act
            var result = await _qualificationService.DeleteAsync(qualificationId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnQualifications_WhenQualificationsExist()
        {
            // Arrange
            var qualifications = new List<Qualification>
            {
                new Qualification { Id = 1, Name = "Qualification 1" },
                new Qualification { Id = 2, Name = "Qualification 2" }
            };
            _qualificationRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(qualifications);

            // Act
            var result = await _qualificationService.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenQualificationsDoNotExist()
        {
            // Arrange
            var qualifications = new List<Qualification>();
            _qualificationRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(qualifications);

            // Act
            var result = await _qualificationService.GetAllAsync();

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnQualification_WhenQualificationExists()
        {
            // Arrange
            var qualificationId = 1;
            var qualification = new Qualification { Id = qualificationId, Name = "Test Qualification" };
            _qualificationRepositoryMock.Setup(repo => repo.GetByIdAsync(qualificationId)).ReturnsAsync(qualification);

            // Act
            var result = await _qualificationService.GetByIdAsync(qualificationId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(qualification, result.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenQualificationNotFound()
        {
            // Arrange
            var qualificationId = 1;
            _qualificationRepositoryMock.Setup(repo => repo.GetByIdAsync(qualificationId)).ReturnsAsync((Qualification)null);

            // Act
            var result = await _qualificationService.GetByIdAsync(qualificationId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenQualificationNotFound()
        {
            // Arrange
            var qualificationUpdateRequest = new QualificationUpdateRequestModel
            {
                Id = 1,
                Name = "Updated Qualification"
            };
            _qualificationRepositoryMock.Setup(repo => repo.GetByIdAsync(qualificationUpdateRequest.Id)).ReturnsAsync((Qualification)null);

            // Act
            var result = await _qualificationService.UpdateAsync(qualificationUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateQualification_WhenValidRequest()
        {
            // Arrange
            var qualificationId = 1;
            var existingQualification = new Qualification { Id = qualificationId, Name = "Existing Qualification" };
            var qualificationUpdateRequest = new QualificationUpdateRequestModel
            {
                Id = qualificationId,
                Name = "Updated Qualification"
            };
            _qualificationRepositoryMock.Setup(repo => repo.GetByIdAsync(qualificationId)).ReturnsAsync(existingQualification);
            _qualificationRepositoryMock.Setup(repo => repo.UpdateAsync(existingQualification)).ReturnsAsync(true);

            // Act
            var result = await _qualificationService.UpdateAsync(qualificationUpdateRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Updated Qualification", existingQualification.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenDuplicateNameExists()
        {
            // Arrange
            var qualificationId = 1;
            var existingQualifications = new List<Qualification>
            {
                new Qualification { Id = 1, Name = "Existing Qualification 1" },
                new Qualification { Id = 2, Name = "Updated Qualification" }
            };
            var qualificationUpdateRequest = new QualificationUpdateRequestModel
            {
                Id = qualificationId,
                Name = "Updated Qualification"
            };
            _qualificationRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingQualifications.AsQueryable());
            _qualificationRepositoryMock.Setup(repo => repo.GetByIdAsync(qualificationId)).ReturnsAsync(existingQualifications[0]);

            // Act
            var result = await _qualificationService.UpdateAsync(qualificationUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenUpdateFails()
        {
            // Arrange
            var qualificationId = 1;
            var existingQualification = new Qualification { Id = qualificationId, Name = "Existing Qualification" };
            var qualificationUpdateRequest = new QualificationUpdateRequestModel
            {
                Id = qualificationId,
                Name = "Updated Qualification"
            };
            _qualificationRepositoryMock.Setup(repo => repo.GetByIdAsync(qualificationId)).ReturnsAsync(existingQualification);
            _qualificationRepositoryMock.Setup(repo => repo.UpdateAsync(existingQualification)).ReturnsAsync(false);

            // Act
            var result = await _qualificationService.UpdateAsync(qualificationUpdateRequest);

            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}
