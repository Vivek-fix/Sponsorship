using Moq;
using SponsorshipApp.Application.Services;
using SponsorshipApp.Domain.Models;
using SponsorshipApp.Infrastructure.Interfaces;

namespace SponsorshipApp.Tests.Services
{
    public class CommunityProjectServiceTests
    {
        private readonly Mock<ICommunityProjectRepository> _mockRepository;
        private readonly CommunityProjectService _service;

        public CommunityProjectServiceTests()
        {
            _mockRepository = new Mock<ICommunityProjectRepository>();
            _service = new CommunityProjectService(_mockRepository.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllProjects()
        {
            // Arrange
            var expectedProjects = new List<CommunityProject>
            {
                new CommunityProject
                {
                    Id = Guid.NewGuid(),
                    Name = "Education Project",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(6),
                    TotalRequired = 10000,
                    TotalRaised = 5000
                },
                new CommunityProject
                {
                    Id = Guid.NewGuid(),
                    Name = "Healthcare Project",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(12),
                    TotalRequired = 20000,
                    TotalRaised = 8000
                }
            };

            _mockRepository.Setup(repo => repo.GetAll())
                .Returns(expectedProjects);

            // Act
            var result = _service.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProjects.Count, result.Count);
            Assert.Equal(expectedProjects, result);
            _mockRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Fact]
        public void GetAll_WhenNoProjects_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<CommunityProject>();
            _mockRepository.Setup(repo => repo.GetAll())
                .Returns(emptyList);

            // Act
            var result = _service.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(repo => repo.GetAll(), Times.Once);
        }
    }
}
