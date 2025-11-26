using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NissensVerksted.Data;
using NissensVerksted.Models;

namespace NissensVerksted.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ØnskeController : ControllerBase
{
    private readonly VerkstedDbContext _context;

    public ØnskeController(VerkstedDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Ønske>> LeggTilØnske(Ønske ønske)
    {
        // Sjekk om ønskelisten eksisterer
        var ønskeliste = await _context.Ønskelister.FindAsync(ønske.ØnskelisteId);
        if (ønskeliste == null)
            return NotFound("Fant ikke ønskelisten");

        // Sjekk om leken eksisterer
        var leke = await _context.Leker.FindAsync(ønske.LekeId);
        if (leke == null)
            return NotFound("Fant ikke leken");

        // Sjekk om leken allerede er på listen
        var eksisterer = await _context.Ønsker
            .AnyAsync(ø => ø.ØnskelisteId == ønske.ØnskelisteId && ø.LekeId == ønske.LekeId);

        if (eksisterer)
            return BadRequest("Denne leken er allerede på ønskelisten");

        _context.Ønsker.Add(ønske);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetØnske), new { id = ønske.ØnskeId }, ønske);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ønske>> GetØnske(int id)
    {
        var ønske = await _context.Ønsker
            .Include(ø => ø.Leke)
            .Include(ø => ø.Ønskeliste)
            .FirstOrDefaultAsync(ø => ø.ØnskeId == id);

        if (ønske == null)
            return NotFound();

        return ønske;
    }

    [HttpPut("{id}/oppfylt")]
    public async Task<IActionResult> MarkerSomOppfylt(int id)
    {
        var ønske = await _context.Ønsker.FindAsync(id);
        if (ønske == null)
            return NotFound();

        ønske.ErOppfylt = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> SlettØnske(int id)
    {
        var ønske = await _context.Ønsker.FindAsync(id);
        if (ønske == null)
            return NotFound();

        _context.Ønsker.Remove(ønske);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}