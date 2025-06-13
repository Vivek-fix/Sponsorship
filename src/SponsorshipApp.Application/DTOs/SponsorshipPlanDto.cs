using SponsorshipApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SponsorshipApp.Application.DTOs
{
    public class SponsorshipPlanDto
    {
        public string CustomerId { get; set; }
        public Guid ProjectId { get; set; }
        public FundSourceType SourceType { get; set; }
        public string SourceValue { get; set; }
        public decimal Amount { get; set; }
        public PaymentFrequency Frequency { get; set; }
    }
}
