using System.ComponentModel.DataAnnotations;
namespace NissensVerksted.Models;

public class Ønskeliste
{
    public int ØnskelisteId { get; set; }

    [Required]
    public string Tittel { get; set; } = "Min Ønskeliste";

    public DateTime OpprettetDato { get; set; } = new DateTime(2025, 11, 1);

    // En ønskeliste tilhører ett barn
    public int BarnId { get; set; }
    public Barn Barn { get; set; } = null!;

    // En ønskeliste har mange ønsker
    public ICollection<Ønske> Ønsker { get; set; } = new List<Ønske>();
}
