using SponsorshipApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SponsorshipApp.Infrastructure.Interfaces
{
    public interface ISponsorshipPlanRepository
    {
        SponsorshipPlan Add(SponsorshipPlan plan);
        List<SponsorshipPlan> GetByCustomer(string customerId);
        SponsorshipPlan GetById(Guid id);
        void Update(SponsorshipPlan plan);
        List<SponsorshipPlan> GetAll();
    }
}
