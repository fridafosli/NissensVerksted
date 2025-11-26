using System.ComponentModel.DataAnnotations;
namespace NissensVerksted.Models;

public class Alv
{
    public int AlvId { get; set; }

    [Required(ErrorMessage = "Navn er påkrevd")]
    [StringLength(100)]
    public string Navn { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Spesialitet { get; set; } = string.Empty; // f.eks. "Tresnikring", "Elektronikk", "Tekstiler"

    [Range(0, 1000)]
    public int Erfaring { get; set; } // År i verkstedet

    [StringLength(50)]
    public string Avdeling { get; set; } = "Produksjon";

    public bool ErAktiv { get; set; } = true;

    // Relasjon til Leker (en-til-mange)
    public ICollection<Leke> Leker { get; set; } = [];
}