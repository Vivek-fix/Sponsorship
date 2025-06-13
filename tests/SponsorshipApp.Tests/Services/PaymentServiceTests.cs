using Moq;
using SponsorshipApp.Application.Services;
using SponsorshipApp.Domain.Models;
using SponsorshipApp.Infrastructure.Interfaces;

namespace SponsorshipApp.Tests.Services
{
    public class PaymentServiceTests
    {
        private readonly Mock<ISponsorshipPlanRepository> _mockPlanRepo;
        private readonly Mock<ICommunityProjectRepository> _mockProjectRepo;
        private readonly PaymentService _service;

        public PaymentServiceTests()
        {
            _mockPlanRepo = new Mock<ISponsorshipPlanRepository>();
            _mockProjectRepo = new Mock<ICommunityProjectRepository>();
            _service = new PaymentService(_mockPlanRepo.Object, _mockProjectRepo.Object);
        }

        [Fact]
        public void ProcessNow_ValidPlan_ShouldCreatePayment()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var plan = new SponsorshipPlan
            {
                Id = planId,
                ProjectId = projectId,
                Amount = 100.00m,
                Payments = new List<Payment>()
            };

            var project = new CommunityProject
            {
                Id = projectId,
                TotalRaised = 1000.00m
            };

            _mockPlanRepo.Setup(repo => repo.GetById(planId))
                .Returns(plan);
            _mockProjectRepo.Setup(repo => repo.GetById(projectId))
                .Returns(project);

            // Act
            var result = _service.ProcessNow(planId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(planId, result.PlanId);
            Assert.Equal(plan.Amount, result.Amount);
            Assert.Equal(DateTime.UtcNow.Date, result.Date.Date);
            
            _mockProjectRepo.Verify(repo => repo.Update(It.Is<CommunityProject>(p => p.TotalRaised == 1100.00m)), Times.Once);
            _mockPlanRepo.Verify(repo => repo.Update(plan), Times.Once);
        }

        [Fact]
        public void ProcessNow_DuplicatePaymentSameDay_ShouldThrowException()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var plan = new SponsorshipPlan
            {
                Id = planId,
                Amount = 100.00m,
                Payments = new List<Payment>
                {
                    new Payment { PlanId = planId, Amount = 100.00m, Date = DateTime.UtcNow }
                }
            };

            _mockPlanRepo.Setup(repo => repo.GetById(planId))
                .Returns(plan);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.ProcessNow(planId));
            Assert.Equal("Duplicate payment not allowed on the same day.", exception.Message);
            
            _mockProjectRepo.Verify(repo => repo.Update(It.IsAny<CommunityProject>()), Times.Never);
            _mockPlanRepo.Verify(repo => repo.Update(It.IsAny<SponsorshipPlan>()), Times.Never);
        }

        [Fact]
        public void ProcessNow_NonExistentPlan_ShouldThrowException()
        {
            // Arrange
            var planId = Guid.NewGuid();
            _mockPlanRepo.Setup(repo => repo.GetById(planId))
                .Returns(() => null!);

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() => _service.ProcessNow(planId));
            Assert.Equal("Sponsorship plan not found.", exception.Message);
            
            _mockProjectRepo.Verify(repo => repo.Update(It.IsAny<CommunityProject>()), Times.Never);
            _mockPlanRepo.Verify(repo => repo.Update(It.IsAny<SponsorshipPlan>()), Times.Never);
        }

        [Fact]
        public void GetPayments_ValidPlan_ShouldReturnPayments()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var expectedPayments = new List<Payment>
            {
                new Payment { PlanId = planId, Amount = 100.00m, Date = DateTime.UtcNow.AddDays(-2) },
                new Payment { PlanId = planId, Amount = 100.00m, Date = DateTime.UtcNow.AddDays(-1) }
            };

            var plan = new SponsorshipPlan
            {
                Id = planId,
                Payments = expectedPayments
            };

            _mockPlanRepo.Setup(repo => repo.GetById(planId))
                .Returns(plan);

            // Act
            var result = _service.GetPayments(planId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPayments.Count, result.Count);
            Assert.Equal(expectedPayments, result);
        }

        [Fact]
        public void GetPayments_NonExistentPlan_ShouldThrowException()
        {
            // Arrange
            var planId = Guid.NewGuid();
            _mockPlanRepo.Setup(repo => repo.GetById(planId))
                .Returns(() => null!);

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() => _service.GetPayments(planId));
            Assert.Equal("Sponsorship plan not found.", exception.Message);
        }
    }
}
