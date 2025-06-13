using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SponsorshipApp.Application.DTOs;
using SponsorshipApp.Application.Interfaces;

namespace SponsorshipApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SponsorshipPlansController : ControllerBase
    {
        private readonly ISponsorshipPlanService _planService;
        private readonly IPaymentService _paymentService;

        public SponsorshipPlansController(ISponsorshipPlanService planService, IPaymentService paymentService)
        {
            _planService = planService;
            _paymentService = paymentService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] SponsorshipPlanDto dto)
        {
            var plan = _planService.Create(dto);
            return Ok(plan);
        }

        [HttpGet("customer/{customerId}")]
        public IActionResult GetPlans(string customerId)
        {
            var plans = _planService.GetByCustomer(customerId);
            return Ok(plans);
        }

        [HttpPost("{planId}/pay")]
        public IActionResult PayNow(Guid planId)
        {
            var payment = _paymentService.ProcessNow(planId);
            return Ok(payment);
        }

        [HttpGet("{planId}/payments")]
        public IActionResult GetPayments(Guid planId)
        {
            var payments = _paymentService.GetPayments(planId);
            return Ok(payments);
        }
    }
}
