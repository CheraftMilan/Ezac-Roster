using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ezac.Roster.Domain.Entities;
using Ezac.Roster.Domain.Interfaces.Repositories;
using Ezac.Roster.Domain.Models.Member;
using Ezac.Roster.Domain.Services;
using Ezac.Roster.Infrastructure.Interfaces;
using Ezac.Roster.Infrastructure.Models.Member;
using Ezac.Roster.Infrastructure.Models.Result;
using Moq;
using Xunit;

namespace Ezac.Roster.UnitTests.Services
{
    public class MemberServiceTests
    {
        private readonly Mock<IMemberRepository> _memberRepositoryMock;
        private readonly Mock<ICalendarRepository> _calendarRepositoryMock;
        private readonly Mock<IQualificationRepository> _qualificationRepositoryMock;
        private readonly Mock<IServiceRepository> _serviceRepositoryMock;
        private readonly IMemberService _memberService;

        public MemberServiceTests()
        {
            _memberRepositoryMock = new Mock<IMemberRepository>();
            _qualificationRepositoryMock = new Mock<IQualificationRepository>();
            _calendarRepositoryMock = new Mock<ICalendarRepository>();
            _serviceRepositoryMock = new Mock<IServiceRepository>();
            _memberService = new MemberService(_memberRepositoryMock.Object, _calendarRepositoryMock.Object, _qualificationRepositoryMock.Object, _serviceRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenMemberNameAlreadyExists()
        {
            //Arrange
            var existingMembers = new List<Member>
            {
                new Member { Id = "1", Name = "Existing Member" }
            };
            _memberRepositoryMock.Setup(repo => repo.GetAll()).Returns(existingMembers.AsQueryable());

            var newMemberRequest = new MemberCreateRequestModel
            {
                Name = "Existing Member",
                Email = "test@example.com",
                Scaling = 1
            };

            //Act
            var result = await _memberService.CreateAsync(newMemberRequest);

            //Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateMember_WhenValidRequest()
        {
            //Arrange
            var calendar = new Calendar { Id = 1 };
            _calendarRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(calendar);

            var newMemberRequest = new MemberCreateRequestModel
            {
                Name = "New Member",
                Email = "test@example.com",
                Scaling = 1,
                CalendarId = 1
            };

            _memberRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Member>())).ReturnsAsync(true);

            // Act
            var result = await _memberService.CreateAsync(newMemberRequest);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("New Member", result.Value.Name);
            Assert.Equal("test@example.com", result.Value.Email);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenInvalidRequest()
        {
            // Arrange
            var newMemberRequest = new MemberCreateRequestModel
            {
                Name = "New Member",
                Email = "test@example.com",
                Scaling = 1
            };

            _memberRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Member>())).ReturnsAsync(false);

            // Act
            var result = await _memberService.CreateAsync(newMemberRequest);

            //Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenMemberNotFound()
        {
            //Arrange
            var memberId = "1";
            _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync((Member)null);

            // Act
            var result = await _memberService.DeleteAsync(memberId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteMember_WhenValidRequest()
        {
            // Arrange
            var memberId = "1";
            var member = new Member { Id = memberId, Name = "Existing Member" };
            _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(member);
            _memberRepositoryMock.Setup(repo => repo.DeleteAsync(member)).ReturnsAsync(true);

            //Act
            var result = await _memberService.DeleteAsync(memberId);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_OnInvalidRequest()
        {
            // Arrange
            var memberId = "1";
            var member = new Member { Id = memberId, Name = "Existing Member" };
            _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(member);
            _memberRepositoryMock.Setup(repo => repo.DeleteAsync(member)).ReturnsAsync(false);

            // Act
            var result = await _memberService.DeleteAsync(memberId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMembers_WhenMembersExist()
        {
            // Arrange
            var members = new List<Member>
            {
                new Member { Id = "1", Name = "Member 1" },
                new Member { Id = "2", Name = "Member 2" }
            };
            _memberRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(members);

            // Act
            var result = await _memberService.GetAllAsync();

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnError_WhenMembersDoNotExist()
        {
            // Arrange
            var members = new List<Member>();
            _memberRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(members);

            // Act
            var result = await _memberService.GetAllAsync();

            //Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMember_WhenMemberExists()
        {
            //  Arrange
            var memberId = "1";
            var member = new Member { Id = memberId, Name = "Test Member" };
            _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(member);

            // Act
            var result = await _memberService.GetByIdAsync(memberId);

            //            Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(member, result.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenMemberNotFound()
        {
            //          Arrange
            var memberId = "1";
            _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync((Member)null);

            //        Act
            var result = await _memberService.GetByIdAsync(memberId);

            //      Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenMemberNotFound()
        {
            //    Arrange
            var memberUpdateRequest = new MemberUpdateRequestModel
            {
                MemberId = "1",
                Scaling = 2
            };
            _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(memberUpdateRequest.MemberId)).ReturnsAsync((Member)null);

            //  Act
            var result = await _memberService.UpdateAsync(memberUpdateRequest);

            //Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateMember_WhenValidRequest()
        {
            // Arrange
            var memberId = "1";
            var existingMember = new Member { Id = memberId, Name = "Existing Member", Scaling = 1 };
            var memberUpdateRequest = new MemberUpdateRequestModel
            {
                MemberId = memberId,
                Scaling = 2
            };
            _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(existingMember);
            _memberRepositoryMock.Setup(repo => repo.UpdateAsync(existingMember)).ReturnsAsync(true);

            // Act
            var result = await _memberService.UpdateAsync(memberUpdateRequest);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, existingMember.Scaling);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenUpdateFails()
        {
            // Arrange
            var memberId = "1";
            var existingMember = new Member { Id = memberId, Name = "Existing Member", Scaling = 1 };
            var memberUpdateRequest = new MemberUpdateRequestModel
            {
                MemberId = memberId,
                Scaling = 2
            };
            _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(existingMember);
            _memberRepositoryMock.Setup(repo => repo.UpdateAsync(existingMember)).ReturnsAsync(false);

            //  Act
            var result = await _memberService.UpdateAsync(memberUpdateRequest);

            //Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task AddQualification_ShouldReturnError_WhenMemberNotFound()
        {
            var memberId = "1";
            var addQualificationRequest = new MemberAddQualificationRequestModel
            {
                MemberId = memberId,
                InstructorLicense = 1,
            };
            _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync((Member)null);

            var result = await _memberService.AddQualification(addQualificationRequest);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task AddQualification_ShouldAddQualification_WhenValidRequest()
        {
            var memberId = "1";
            var member = new Member { Id = memberId, Name = "Test Member", Qualifications = new List<Qualification>() };
            var addQualificationRequest = new MemberAddQualificationRequestModel
            {
                MemberId = memberId,
                InstructorLicense = 1,
                LierLicense = 1,
                StartingOfficerLicense = 1,
                BarLicense = 1
            };

            var services = new List<Service>
            {
                new Service { Id = 1, Name = "Service 1" },
                new Service { Id = 2, Name = "Service 2" },
                new Service { Id = 3, Name = "Service 3" },
                new Service { Id = 4, Name = "Service 4" },
                new Service { Id = 5, Name = "Service 5" }
            };

            _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(member);
            _serviceRepositoryMock.Setup(repo => repo.GetAll()).Returns(services.AsQueryable());
            _qualificationRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Qualification>())).ReturnsAsync(true);

            var result = await _memberService.AddQualification(addQualificationRequest);

            Assert.True(result.IsSuccess);
            Assert.Equal(4, member.Qualifications.Count);
        }

        [Fact]
        public async Task AddQualification_ShouldReturnError_WhenQualificationAddFails()
        {
            var memberId = "1";
            var member = new Member { Id = memberId, Name = "Test Member" };
            var addQualificationRequest = new MemberAddQualificationRequestModel
            {
                MemberId = memberId,
                InstructorLicense = 1
            };

            _memberRepositoryMock.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(member);
            _qualificationRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Qualification>())).ReturnsAsync(false);

            var result = await _memberService.AddQualification(addQualificationRequest);

            Assert.False(result.IsSuccess);
        }
    }
}
