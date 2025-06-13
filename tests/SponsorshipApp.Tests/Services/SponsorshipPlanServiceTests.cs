using Moq;
using SponsorshipApp.Application.DTOs;
using SponsorshipApp.Application.Services;
using SponsorshipApp.Domain.Enums;
using SponsorshipApp.Domain.Models;
using SponsorshipApp.Infrastructure.Interfaces;

namespace SponsorshipApp.Tests.Services
{
    public class SponsorshipPlanServiceTests
    {
        private readonly Mock<ISponsorshipPlanRepository> _mockPlanRepo;
        private readonly Mock<ICommunityProjectRepository> _mockProjectRepo;
        private readonly SponsorshipPlanService _service;

        public SponsorshipPlanServiceTests()
        {
            _mockPlanRepo = new Mock<ISponsorshipPlanRepository>();
            _mockProjectRepo = new Mock<ICommunityProjectRepository>();
            _service = new SponsorshipPlanService(_mockPlanRepo.Object, _mockProjectRepo.Object);
        }

        [Fact]
        public void Create_ValidDto_ShouldCreatePlan()
        {
            // Arrange
            var dto = new SponsorshipPlanDto
            {
                CustomerId = "customer123",
                ProjectId = Guid.NewGuid(),
                SourceType = FundSourceType.Card,
                SourceValue = "4111111111111111",
                Amount = 100.00m,
                Frequency = PaymentFrequency.Monthly
            };

            var expectedPlan = new SponsorshipPlan
            {
                CustomerId = dto.CustomerId,
                ProjectId = dto.ProjectId,
                SourceType = dto.SourceType,
                SourceValue = dto.SourceValue,
                Amount = dto.Amount,
                Frequency = dto.Frequency
            };

            _mockPlanRepo.Setup(repo => repo.Add(It.IsAny<SponsorshipPlan>()))
                .Returns((SponsorshipPlan plan) => plan);

            // Act
            var result = _service.Create(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.CustomerId, result.CustomerId);
            Assert.Equal(dto.ProjectId, result.ProjectId);
            Assert.Equal(dto.SourceType, result.SourceType);
            Assert.Equal(dto.SourceValue, result.SourceValue);
            Assert.Equal(dto.Amount, result.Amount);
            Assert.Equal(dto.Frequency, result.Frequency);
            _mockPlanRepo.Verify(repo => repo.Add(It.IsAny<SponsorshipPlan>()), Times.Once);
        }

        [Fact]
        public void GetByCustomer_ShouldReturnCustomerPlans()
        {
            // Arrange
            var customerId = "customer123";
            var expectedPlans = new List<SponsorshipPlan>
            {
                new SponsorshipPlan
                {
                    CustomerId = customerId,
                    ProjectId = Guid.NewGuid(),
                    SourceType = FundSourceType.Card,
                    Amount = 100.00m,
                    Frequency = PaymentFrequency.Monthly
                },
                new SponsorshipPlan
                {
                    CustomerId = customerId,
                    ProjectId = Guid.NewGuid(),
                    SourceType = FundSourceType.Account,
                    Amount = 50.00m,
                    Frequency = PaymentFrequency.Weekly
                }
            };

            _mockPlanRepo.Setup(repo => repo.GetByCustomer(customerId))
                .Returns(expectedPlans);

            // Act
            var result = _service.GetByCustomer(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPlans.Count, result.Count);
            Assert.Equal(expectedPlans, result);
            _mockPlanRepo.Verify(repo => repo.GetByCustomer(customerId), Times.Once);
        }

        [Fact]
        public void GetByCustomer_NoPlans_ShouldReturnEmptyList()
        {
            // Arrange
            var customerId = "customer123";
            _mockPlanRepo.Setup(repo => repo.GetByCustomer(customerId))
                .Returns(new List<SponsorshipPlan>());

            // Act
            var result = _service.GetByCustomer(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockPlanRepo.Verify(repo => repo.GetByCustomer(customerId), Times.Once);
        }

        [Fact]
        public void ProcessRecurringPayments_WeeklyPayment_ShouldProcessPayment()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var plan = new SponsorshipPlan
            {
                Id = Guid.NewGuid(),
                CustomerId = "customer123",
                ProjectId = projectId,
                Amount = 100.00m,
                Frequency = PaymentFrequency.Weekly,
                CreatedOn = DateTime.UtcNow.AddDays(-8),
                LastPaymentDate = DateTime.UtcNow.AddDays(-7),
                Payments = new List<Payment>()
            };

            var project = new CommunityProject
            {
                Id = projectId,
                TotalRaised = 1000.00m
            };

            _mockPlanRepo.Setup(repo => repo.GetAll())
                .Returns(new List<SponsorshipPlan> { plan });

            _mockProjectRepo.Setup(repo => repo.GetById(projectId))
                .Returns(project);

            // Act
            _service.ProcessRecurringPayments();

            // Assert
            Assert.Single(plan.Payments);
            Assert.Equal(plan.Amount, plan.Payments.First().Amount);
            Assert.Equal(plan.Id, plan.Payments.First().PlanId);
            _mockProjectRepo.Verify(repo => repo.Update(It.Is<CommunityProject>(p => p.TotalRaised == 1100.00m)), Times.Once);
            _mockPlanRepo.Verify(repo => repo.Update(plan), Times.Once);
        }

        [Fact]
        public void ProcessRecurringPayments_MonthlyPayment_ShouldProcessPayment()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var plan = new SponsorshipPlan
            {
                Id = Guid.NewGuid(),
                CustomerId = "customer123",
                ProjectId = projectId,
                Amount = 100.00m,
                Frequency = PaymentFrequency.Monthly,
                CreatedOn = DateTime.UtcNow.AddDays(-31),
                LastPaymentDate = DateTime.UtcNow.AddDays(-30),
                Payments = new List<Payment>()
            };

            var project = new CommunityProject
            {
                Id = projectId,
                TotalRaised = 1000.00m
            };

            _mockPlanRepo.Setup(repo => repo.GetAll())
                .Returns(new List<SponsorshipPlan> { plan });

            _mockProjectRepo.Setup(repo => repo.GetById(projectId))
                .Returns(project);

            // Act
            _service.ProcessRecurringPayments();

            // Assert
            Assert.Single(plan.Payments);
            Assert.Equal(plan.Amount, plan.Payments.First().Amount);
            Assert.Equal(plan.Id, plan.Payments.First().PlanId);
            _mockProjectRepo.Verify(repo => repo.Update(It.Is<CommunityProject>(p => p.TotalRaised == 1100.00m)), Times.Once);
            _mockPlanRepo.Verify(repo => repo.Update(plan), Times.Once);
        }

        [Fact]
        public void ProcessRecurringPayments_OnceOffAlreadyPaid_ShouldNotProcessPayment()
        {
            // Arrange
            var plan = new SponsorshipPlan
            {
                Id = Guid.NewGuid(),
                CustomerId = "customer123",
                ProjectId = Guid.NewGuid(),
                Amount = 100.00m,
                Frequency = PaymentFrequency.OnceOff,
                CreatedOn = DateTime.UtcNow.AddDays(-1),
                Payments = new List<Payment>
                {
                    new Payment { Id = Guid.NewGuid(), Amount = 100.00m, Date = DateTime.UtcNow.AddDays(-1) }
                }
            };

            _mockPlanRepo.Setup(repo => repo.GetAll())
                .Returns(new List<SponsorshipPlan> { plan });

            // Act
            _service.ProcessRecurringPayments();

            // Assert
            Assert.Single(plan.Payments);
            _mockProjectRepo.Verify(repo => repo.Update(It.IsAny<CommunityProject>()), Times.Never);
            _mockPlanRepo.Verify(repo => repo.Update(It.IsAny<SponsorshipPlan>()), Times.Never);
        }

        [Fact]
        public void ProcessRecurringPayments_PaymentNotDueYet_ShouldNotProcessPayment()
        {
            // Arrange
            var plan = new SponsorshipPlan
            {
                Id = Guid.NewGuid(),
                CustomerId = "customer123",
                ProjectId = Guid.NewGuid(),
                Amount = 100.00m,
                Frequency = PaymentFrequency.Monthly,
                CreatedOn = DateTime.UtcNow.AddDays(-15),
                LastPaymentDate = DateTime.UtcNow.AddDays(-15),
                Payments = new List<Payment>()
            };

            _mockPlanRepo.Setup(repo => repo.GetAll())
                .Returns(new List<SponsorshipPlan> { plan });

            // Act
            _service.ProcessRecurringPayments();

            // Assert
            Assert.Empty(plan.Payments);
            _mockProjectRepo.Verify(repo => repo.Update(It.IsAny<CommunityProject>()), Times.Never);
            _mockPlanRepo.Verify(repo => repo.Update(It.IsAny<SponsorshipPlan>()), Times.Never);
        }
    }
}
