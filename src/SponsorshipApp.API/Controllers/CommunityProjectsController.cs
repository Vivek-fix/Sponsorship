using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SponsorshipApp.Application.Interfaces;

namespace SponsorshipApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityProjectsController : ControllerBase
    {
        private readonly ICommunityProjectService _service;

        public CommunityProjectsController(ICommunityProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetProjects()
        {
            var projects = _service.GetAll();
            return Ok(projects);
        }
    }
}
