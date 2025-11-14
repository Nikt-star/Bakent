using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto3.Infrastructure.Data;
using Proyecto3.Core.Entities;
using Proyecto3.Core.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Proyecto3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly TalentoInternoDbContext _context;

        public SkillsController(TalentoInternoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var skills = await _context.Skills.ToListAsync();
            return Ok(skills);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            return Ok(skill);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SkillCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.NombreSkill)) return BadRequest();

            var existing = await _context.Skills.FirstOrDefaultAsync(s => s.NombreSkill.ToLower() == dto.NombreSkill.ToLower());
            if (existing != null) return Conflict(new { message = "Skill already exists", skillId = existing.SkillID });

            var skill = new Skill { NombreSkill = dto.NombreSkill, TipoSkill = dto.TipoSkill };
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = skill.SkillID }, skill);
        }
    }
}
