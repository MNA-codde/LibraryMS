using MediatR;
using LibraryMSBackend.Applications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class StatsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("popularity")]
        public async Task<IActionResult> GetPopularityStats()
        {
            var result = await _mediator.Send(new GetBookPopularityStatsQuery());
            return Ok(result);
        }
    }
}