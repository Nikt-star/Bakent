using Microsoft.AspNetCore.Mvc;
using Proyecto3.Infrastructure.Data;
using Proyecto3.Infrastructure.Seed;

namespace Proyecto3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly TalentoInternoDbContext _context;

        public SeedController(TalentoInternoDbContext context)
        {
            _context = context;
        }

        [HttpPost("run")] // POST api/Seed/run
        public async Task<IActionResult> RunSeed()
        {
            await DataSeeder.SeedAsync(_context);
            return Ok(new { seeded = true });
        }
    }
}
