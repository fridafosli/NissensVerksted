using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NissensVerksted.Data;
using NissensVerksted.Models;

namespace NissensVerksted.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarnController : ControllerBase
{
    private readonly VerkstedDbContext _context;

    public BarnController(VerkstedDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Barn>>> GetAlleBarn()
    {
        return await _context.Barn
            .Include(b => b.Ønskeliste)
                .ThenInclude(ø => ø!.Ønsker)
                    .ThenInclude(ø => ø.Leke)
            .ToListAsync();
    }

    [HttpGet("sql")]
    public async Task<ActionResult<IEnumerable<object>>> GetAlleBarnMedRåSql()
    {
        // Rå SQL versjon - returnerer anonyme objekter
        var sql = @"
            SELECT 
                b.BarnId, 
                b.Navn AS BarnNavn, 
                b.Adresse, 
                b.Land, 
                b.Alder, 
                b.NiceScore,
                øl.ØnskelisteId,
                øl.Tittel AS ØnskelisteTittel,
                øl.OpprettetDato AS ØnskelisteOpprettet,
                ø.ØnskeId,
                ø.Prioritet,
                ø.Kommentar,
                ø.ErOppfylt,
                l.LekeId,
                l.Navn AS LekeNavn,
                l.Beskrivelse AS LekeBeskrivelse,
                l.Status AS LekeStatus
            FROM Barn b
            LEFT JOIN Ønskeliste øl ON b.ØnskelisteId = øl.ØnskelisteId
            LEFT JOIN Ønske ø ON øl.ØnskelisteId = ø.ØnskelisteId
            LEFT JOIN Leke l ON ø.LekeId = l.LekeId
            ORDER BY b.BarnId, ø.Prioritet";

        var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        var result = new List<object>();

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                result.Add(new
                {
                    BarnId = reader.GetInt32(0),
                    BarnNavn = reader.GetString(1),
                    Adresse = reader.GetString(2),
                    Land = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Alder = reader.GetInt32(4),
                    NiceScore = reader.GetInt32(5),
                    ØnskelisteId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                    ØnskelisteTittel = reader.IsDBNull(7) ? null : reader.GetString(7),
                    ØnskeId = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                    Prioritet = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                    LekeNavn = reader.IsDBNull(14) ? null : reader.GetString(14)
                });
            }
        }

        return Ok(result);
    }

    [HttpGet("{barnId}/ansvarlige-alver")]
    public async Task<ActionResult<IEnumerable<Alv>>> GetAlverForBarnetsOppfylteLeker(int barnId)
    {
        var alver = await _context.Barn
            .Where(b => b.BarnId == barnId)
            .SelectMany(b => b.Ønskeliste!.Ønsker)           // Alle ønsker
            .Where(ø => ø.ErOppfylt)                         // KUN oppfylte ønsker!
            .Select(ø => ø.Leke)                             // Alle leker
            .Select(l => l.AnsvarligAlv)                    // Hent alvene
            .ToListAsync();

        if (!alver.Any())
            return NotFound("Fant ingen ansvarlige alver for dette barnets oppfylte ønsker");

        return Ok(alver);
    }

    [HttpGet("{barnId}/ansvarlige-alver-sql")]
    public async Task<ActionResult<IEnumerable<Alv>>> GetAlverForBarnetsOppfylteLekerMedSql(int barnId)
    {
        var sql = @"
        SELECT 
            a.AlvId, 
            a.Navn, 
            a.Spesialitet,
            a.Erfaring,
            a.Avdeling,
            a.ErAktiv
        FROM Barn b
        INNER JOIN Ønskeliste øl ON b.ØnskelisteId = øl.ØnskelisteId
        INNER JOIN Ønske ø ON øl.ØnskelisteId = ø.ØnskelisteId
        INNER JOIN Leke l ON ø.LekeId = l.LekeId
        INNER JOIN Alv a ON l.AlvId = a.AlvId
        WHERE b.BarnId = @BarnId 
          AND ø.ErOppfylt = 1
        ORDER BY a.Navn";

        var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();

        var alver = new List<Alv>();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = sql;

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@BarnId";
            parameter.Value = barnId;
            command.Parameters.Add(parameter);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                alver.Add(new Alv
                {
                    AlvId = reader.GetInt32(0),
                    Navn = reader.GetString(1),
                    Spesialitet = reader.GetString(2),
                    Erfaring = reader.GetInt32(3),
                    Avdeling = reader.GetString(4),
                    ErAktiv = reader.GetBoolean(5)
                });
            }
        }

        if (!alver.Any())
            return NotFound("Fant ingen ansvarlige alver for dette barnets oppfylte ønsker");

        return Ok(alver);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Barn>> GetBarn(int id)
    {
        var barn = await _context.Barn
            .Include(b => b.Ønskeliste)
                .ThenInclude(ø => ø!.Ønsker)
                    .ThenInclude(ø => ø.Leke)
            .FirstOrDefaultAsync(b => b.BarnId == id);

        if (barn == null)
            return NotFound();

        return barn;
    }

    [HttpGet("{id}/ønskeliste")]
    public async Task<ActionResult<Ønskeliste>> GetØnskeliste(int id)
    {
        var ønskeliste = await _context.Ønskelister
            .Include(ø => ø.Barn)
            .Include(ø => ø.Ønsker)
                .ThenInclude(ø => ø.Leke)
            .FirstOrDefaultAsync(ø => ø.BarnId == id);

        if (ønskeliste == null)
            return NotFound("Barnet har ingen ønskeliste ennå");

        return ønskeliste;
    }

    [HttpPost]
    public async Task<ActionResult<Barn>> OpprettBarn(Barn barn)
    {
        _context.Barn.Add(barn);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBarn), new { id = barn.BarnId }, barn);
    }
}