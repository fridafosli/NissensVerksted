using System.ComponentModel.DataAnnotations;
namespace NissensVerksted.Models;

public class Barn
{
    public int BarnId { get; set; }

    [Required]
    public string Navn { get; set; } = string.Empty;

    [Required]
    public string Adresse { get; set; } = string.Empty;

    public string? Land { get; set; }

    [Range(0, 18)]
    public int Alder { get; set; }

    [Range(0, 100)]
    public int NiceScore { get; set; } = 10;

    public bool HarVærtSnill => NiceScore >= 50;

    public int? ØnskelisteId { get; set; }
    public Ønskeliste? Ønskeliste { get; set; }
}