using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NissensVerksted.Data;
using NissensVerksted.Models;

namespace NissensVerksted.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlvController(VerkstedDbContext context) : ControllerBase
{
    private readonly VerkstedDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Alv>>> GetAlleAlver()
    {
        return await _context.Alver
            .Include(a => a.Leker)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Alv>> GetAlv(int id)
    {
        var alv = await _context.Alver
            .Include(a => a.Leker)
            .FirstOrDefaultAsync(a => a.AlvId == id);

        if (alv == null)
            return NotFound();

        return alv;
    }

    [HttpPost]
    public async Task<ActionResult<Alv>> OpprettAlv(Alv alv)
    {
        _context.Alver.Add(alv);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAlv), new { id = alv.AlvId }, alv);
    }
}