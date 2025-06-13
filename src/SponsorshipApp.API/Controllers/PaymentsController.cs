using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SponsorshipApp.Application.Interfaces;
using SponsorshipApp.Domain.Models;

namespace SponsorshipApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentsController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("{planId}/process-now")]
        public ActionResult<Payment> ProcessNow(Guid planId)
        {
            var payment = _service.ProcessNow(planId);
            if (payment == null)
                return NotFound("Sponsorship plan not found.");

            return Ok(payment);
        }

        [HttpGet("{planId}")]
        public ActionResult<List<Payment>> GetPayments(Guid planId)
        {
            return Ok(_service.GetPayments(planId));
        }
    }
}
