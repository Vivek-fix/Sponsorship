using SponsorshipApp.Domain.Models;
using SponsorshipApp.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SponsorshipApp.Infrastructure.Repositories
{
    public class InMemorySponsorshipPlanRepository : ISponsorshipPlanRepository
    {
        private readonly List<SponsorshipPlan> _plans = new();

        public SponsorshipPlan Add(SponsorshipPlan plan)
        {
            _plans.Add(plan);
            return plan;
        }

        public List<SponsorshipPlan> GetByCustomer(string customerId) =>
            _plans.Where(p => p.CustomerId == customerId).ToList();

        public SponsorshipPlan GetById(Guid id) => _plans.FirstOrDefault(p => p.Id == id);

        public void Update(SponsorshipPlan plan)
        {
            var index = _plans.FindIndex(p => p.Id == plan.Id);
            if (index >= 0) _plans[index] = plan;
        }

        public List<SponsorshipPlan> GetAll() => _plans;
    }

}
