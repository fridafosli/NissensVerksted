using Microsoft.EntityFrameworkCore;
using NissensVerksted.Models;
using System.Runtime.Intrinsics.X86;

namespace NissensVerksted.Data;

public class VerkstedDbContext : DbContext
{
    public VerkstedDbContext(DbContextOptions<VerkstedDbContext> options)
        : base(options)
    {
    }

    public DbSet<Leke> Leker { get; set; }
    public DbSet<Barn> Barn { get; set; }
    public DbSet<Alv> Alver { get; set; }
    public DbSet<Ønskeliste> Ønskelister { get; set; }
    public DbSet<Ønske> Ønsker { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Konfigurer Leke
        modelBuilder.Entity<Leke>(entity =>
        {
            entity.HasKey(l => l.LekeId);
            entity.Property(l => l.Navn).IsRequired();
            entity.HasIndex(l => l.Status);

            // En-til-mange: En Nisse kan ha mange Leker
            entity.HasOne(l => l.AnsvarligAlv)
                  .WithMany(n => n.Leker)
                  .HasForeignKey(l => l.AlvId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Konfigurer Barn
        modelBuilder.Entity<Barn>(entity =>
        {
            entity.HasKey(b => b.BarnId);
            entity.Property(b => b.Navn).IsRequired();
            entity.HasIndex(b => b.NiceScore);

            // En-til-en: Et barn har en ønskeliste
            entity.HasOne(b => b.Ønskeliste)
                  .WithOne(ø => ø.Barn)
                  .HasForeignKey<Barn>(b => b.ØnskelisteId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Konfigurer Nisse
        modelBuilder.Entity<Alv>(entity =>
        {
            entity.HasKey(n => n.AlvId);
            entity.Property(n => n.Navn).IsRequired();
            entity.HasIndex(n => n.Spesialitet);
        });

        // Konfigurer Ønskeliste
        modelBuilder.Entity<Ønskeliste>(entity =>
        {
            entity.HasKey(ø => ø.ØnskelisteId);

            // Relasjonen til Barn er definert i Barn-entiteten
        });

        // Konfigurer Ønske
        modelBuilder.Entity<Ønske>(entity =>
        {
            entity.HasKey(ø => ø.ØnskeId);

            // Relasjon til Ønskeliste
            entity.HasOne(ø => ø.Ønskeliste)
                  .WithMany(øl => øl.Ønsker)
                  .HasForeignKey(ø => ø.ØnskelisteId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Relasjon til Leke
            entity.HasOne(ø => ø.Leke)
                  .WithMany(l => l.Ønsker)
                  .HasForeignKey(ø => ø.LekeId)
                  .OnDelete(DeleteBehavior.Restrict); // Kan ikke slette leke hvis den er på ønskelister

            // Unikt indeks: Samme leke kan ikke være på samme ønskeliste flere ganger
            entity.HasIndex(ø => new { ø.ØnskelisteId, ø.LekeId }).IsUnique();
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Nisser
        modelBuilder.Entity<Alv>().HasData(
            new Alv { AlvId = 1, Navn = "Bjørnis Hammerhand", Spesialitet = "Tresnikring", Erfaring = 150, Avdeling = "Produksjon" },
            new Alv { AlvId = 2, Navn = "Elina Elektronikk", Spesialitet = "Elektronikk", Erfaring = 75, Avdeling = "Teknologi" },
            new Alv { AlvId = 3, Navn = "Turid Tøysterk", Spesialitet = "Tekstiler", Erfaring = 200, Avdeling = "Design" }
        );

        // Seed Leker
        modelBuilder.Entity<Leke>().HasData(
            new Leke { LekeId = 1, Navn = "Tredukke", Beskrivelse = "Håndlaget tredukke", Status = "Innpakket", Antall = 50, Aldersgruppe = "3-6 år", AlvId = 1 },
            new Leke { LekeId = 2, Navn = "Robotbil", Beskrivelse = "Fjernstyrt bil med lys", Status = "Testing", Antall = 30, Aldersgruppe = "7-12 år", AlvId = 2 },
            new Leke { LekeId = 3, Navn = "Kosedyr", Beskrivelse = "Myk bjørn", Status = "Montering", Antall = 100, Aldersgruppe = "0-3 år", AlvId = 3 },
            new Leke { LekeId = 4, Navn = "Puslespill", Beskrivelse = "1000 brikker", Status = "Design", Antall = 20, Aldersgruppe = "10+ år", AlvId = 1 },
            new Leke { LekeId = 5, Navn = "Sparkesykkel", Beskrivelse = "Rød sparkesykkel", Status = "Innpakket", Antall = 15, Aldersgruppe = "5-10 år", AlvId = 2 },
            new Leke { LekeId = 6, Navn = "Bok: Eventyr", Beskrivelse = "Samling med eventyr", Status = "Innpakket", Antall = 80, Aldersgruppe = "4-8 år", AlvId = 3 }
        );

        // Seed Ønskelister (først, uten BarnId siden det skaper circular reference)
        modelBuilder.Entity<Ønskeliste>().HasData(
            new Ønskeliste { ØnskelisteId = 1, Tittel = "Emma sin ønskeliste", OpprettetDato = new DateTime(2024, 11, 1) },
            new Ønskeliste { ØnskelisteId = 2, Tittel = "Oliver sin ønskeliste", OpprettetDato = new DateTime(2024, 11, 5) },
            new Ønskeliste { ØnskelisteId = 3, Tittel = "Sofie sin ønskeliste", OpprettetDato = new DateTime(2024, 10, 28) }
        );

        // Seed Barn (med referanse til ønskeliste)
        modelBuilder.Entity<Barn>().HasData(
            new Barn { BarnId = 1, Navn = "Emma Hansen", Adresse = "Storgata 1", Land = "Norge", Alder = 7, NiceScore = 85, ØnskelisteId = 1 },
            new Barn { BarnId = 2, Navn = "Oliver Johansen", Adresse = "Fjellveien 23", Land = "Norge", Alder = 5, NiceScore = 70, ØnskelisteId = 2 },
            new Barn { BarnId = 3, Navn = "Sofie Berg", Adresse = "Skogsveien 45", Land = "Norge", Alder = 10, NiceScore = 95, ØnskelisteId = 3 }
        );

        // Seed Ønsker
        modelBuilder.Entity<Ønske>().HasData(
            // Emma sin ønskeliste
            new Ønske { ØnskeId = 1, ØnskelisteId = 1, LekeId = 2, Prioritet = 1, Kommentar = "Ønsker meg blå robotbil!", ØnsketDato = new DateTime(2024, 11, 1) },
            new Ønske { ØnskeId = 2, ØnskelisteId = 1, LekeId = 6, Prioritet = 2, Kommentar = "Elsker å lese!", ØnsketDato = new DateTime(2024, 11, 2) },
            new Ønske { ØnskeId = 3, ØnskelisteId = 1, LekeId = 5, Prioritet = 3, ØnsketDato = new DateTime(2024, 11, 3) },

            // Oliver sin ønskeliste
            new Ønske { ØnskeId = 4, ØnskelisteId = 2, LekeId = 1, Prioritet = 1, Kommentar = "Vil ha tredukke!", ØnsketDato = new DateTime(2024, 11, 5) },
            new Ønske { ØnskeId = 5, ØnskelisteId = 2, LekeId = 3, Prioritet = 2, Kommentar = "Helst brun bjørn", ØnsketDato = new DateTime(2024, 11, 5) },

            // Sofie sin ønskeliste
            new Ønske { ØnskeId = 6, ØnskelisteId = 3, LekeId = 4, Prioritet = 1, ErOppfylt = true, ØnsketDato = new DateTime(2024, 10, 28) },
            new Ønske { ØnskeId = 7, ØnskelisteId = 3, LekeId = 2, Prioritet = 2, Kommentar = "Robotbil med lys!", ØnsketDato = new DateTime(2024, 10, 30) }
        );
    }
}