using SponsorshipApp.Application.DTOs;
using SponsorshipApp.Application.Interfaces;
using SponsorshipApp.Domain.Enums;
using SponsorshipApp.Domain.Models;
using SponsorshipApp.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SponsorshipApp.Application.Services
{
    public class SponsorshipPlanService : ISponsorshipPlanService
    {
        private readonly ISponsorshipPlanRepository _planRepo;
        private readonly ICommunityProjectRepository _projectRepo;

        public SponsorshipPlanService(ISponsorshipPlanRepository planRepo, ICommunityProjectRepository projectRepo)
        {
            _planRepo = planRepo;
            _projectRepo = projectRepo;
        }

        public SponsorshipPlan Create(SponsorshipPlanDto dto)
        {
            var plan = new SponsorshipPlan
            {
                CustomerId = dto.CustomerId,
                ProjectId = dto.ProjectId,
                SourceType = dto.SourceType,
                SourceValue = dto.SourceValue,
                Amount = dto.Amount,
                Frequency = dto.Frequency
            };
            return _planRepo.Add(plan);
        }

        public List<SponsorshipPlan> GetByCustomer(string customerId) => _planRepo.GetByCustomer(customerId);

        public void ProcessRecurringPayments()
        {
            var plans = _planRepo.GetAll().ToList(); // Snapshot plans

            foreach (var plan in plans)
            {
                // Take a snapshot of Payments list to avoid modification during enumeration
                var existingPayments = plan.Payments?.ToList() ?? new List<Payment>();

                if (plan.Frequency == PaymentFrequency.OnceOff && existingPayments.Any())
                    continue;

                var nextDue = plan.LastPaymentDate?.AddDays(plan.Frequency switch
                {
                    PaymentFrequency.Weekly => 7,
                    PaymentFrequency.Monthly => 30,
                    _ => 0
                }) ?? plan.CreatedOn;

                if (DateTime.UtcNow.Date >= nextDue.Date && !plan.Payments.Any(p => p.Date.Date == DateTime.UtcNow.Date))
                {
                    var payment = new Payment { PlanId = plan.Id, Amount = plan.Amount };

                    // Add new payment
                    plan.Payments.Add(payment);  // now modifying original list, not during enumeration

                    plan.LastPaymentDate = DateTime.UtcNow;

                    var project = _projectRepo.GetById(plan.ProjectId);
                    if (project != null)
                    {
                        project.TotalRaised += plan.Amount;
                        _projectRepo.Update(project);
                    }

                    _planRepo.Update(plan);
                }
            }
        }

    }
}