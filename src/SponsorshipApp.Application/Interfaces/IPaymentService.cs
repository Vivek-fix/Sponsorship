using SponsorshipApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SponsorshipApp.Application.Interfaces
{
    public interface IPaymentService
    {
        Payment ProcessNow(Guid planId);
        List<Payment> GetPayments(Guid planId);
    }
}
