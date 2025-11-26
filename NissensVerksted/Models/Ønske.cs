using System.ComponentModel.DataAnnotations;
namespace NissensVerksted.Models;

public class Ønske
{
    public int ØnskeId { get; set; }

    // Tilhører en ønskeliste
    public int ØnskelisteId { get; set; }
    public Ønskeliste Ønskeliste { get; set; } = null!;

    // Refererer til en leke
    public int LekeId { get; set; }
    public Leke Leke { get; set; } = null!;

    [Range(1, 10)]
    public int Prioritet { get; set; } = 1;

    public string? Kommentar { get; set; }
    public bool ErOppfylt { get; set; } = false;
    public DateTime ØnsketDato { get; set; } = new DateTime(2025, 11, 1);
}
