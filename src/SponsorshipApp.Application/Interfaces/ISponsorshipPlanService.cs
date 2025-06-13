using SponsorshipApp.Application.DTOs;
using SponsorshipApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SponsorshipApp.Application.Interfaces
{
    public interface ISponsorshipPlanService
    {
        SponsorshipPlan Create(SponsorshipPlanDto dto);
        List<SponsorshipPlan> GetByCustomer(string customerId);
        void ProcessRecurringPayments();
    }
}
