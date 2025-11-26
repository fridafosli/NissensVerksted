using System.ComponentModel.DataAnnotations;

namespace NissensVerksted.Models;

public class Leke
{
    public int LekeId { get; set; }
    [Required]
    [StringLength(100)]
    public string Navn { get; set; } = string.Empty;
    [StringLength(500)]
    public string? Beskrivelse { get; set; }
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Design";
    [Range(0, 10000)]
    public int Antall { get; set; }
    [Required]
    [StringLength(50)]
    public string Aldersgruppe { get; set; } = "Alle";
    public DateTime OpprettetDato { get; set; } = new DateTime(2025,11,1);
    public int? AlvId { get; set; }
    public Alv? AnsvarligAlv { get; set; }
    public ICollection<Ønske>? Ønsker { get; set; }
}