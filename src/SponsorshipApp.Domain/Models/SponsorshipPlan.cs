using SponsorshipApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SponsorshipApp.Domain.Models
{
    public class SponsorshipPlan
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerId { get; set; }
        public Guid ProjectId { get; set; }
        public FundSourceType SourceType { get; set; }
        public string SourceValue { get; set; }
        public decimal Amount { get; set; }
        public PaymentFrequency Frequency { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? LastPaymentDate { get; set; }
        public List<Payment> Payments { get; set; } = new();
    }
}
