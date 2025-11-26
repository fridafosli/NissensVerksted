using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NissensVerksted.Data;
using NissensVerksted.Models;

namespace NissensVerksted.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LekeController(VerkstedDbContext context) : ControllerBase
{
    private readonly VerkstedDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Leke>>> GetAlleLeker()
    {
        return await _context.Leker
            .Include(l => l.AnsvarligAlv)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Leke>> GetLeke(int id)
    {
        var leke = await _context.Leker
            .Include(l => l.AnsvarligAlv)
            .FirstOrDefaultAsync(l => l.LekeId == id);

        if (leke == null)
            return NotFound();

        return leke;
    }

    [HttpPost]
    public async Task<ActionResult<Leke>> OpprettLeke(Leke leke)
    {
        _context.Leker.Add(leke);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLeke), new { id = leke.LekeId }, leke);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> OppdaterLeke(int id, Leke leke)
    {
        if (id != leke.LekeId)
            return BadRequest();

        _context.Entry(leke).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Leker.Any(l => l.LekeId == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }
}