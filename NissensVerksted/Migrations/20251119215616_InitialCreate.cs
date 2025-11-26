using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NissensVerksted.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alver",
                columns: table => new
                {
                    AlvId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Navn = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Spesialitet = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Erfaring = table.Column<int>(type: "INTEGER", nullable: false),
                    Avdeling = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ErAktiv = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alver", x => x.AlvId);
                });

            migrationBuilder.CreateTable(
                name: "Ønskelister",
                columns: table => new
                {
                    ØnskelisteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tittel = table.Column<string>(type: "TEXT", nullable: false),
                    OpprettetDato = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BarnId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ønskelister", x => x.ØnskelisteId);
                });

            migrationBuilder.CreateTable(
                name: "Leker",
                columns: table => new
                {
                    LekeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Navn = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Beskrivelse = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Antall = table.Column<int>(type: "INTEGER", nullable: false),
                    Aldersgruppe = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    OpprettetDato = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlvId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leker", x => x.LekeId);
                    table.ForeignKey(
                        name: "FK_Leker_Alver_AlvId",
                        column: x => x.AlvId,
                        principalTable: "Alver",
                        principalColumn: "AlvId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Barn",
                columns: table => new
                {
                    BarnId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Navn = table.Column<string>(type: "TEXT", nullable: false),
                    Adresse = table.Column<string>(type: "TEXT", nullable: false),
                    Land = table.Column<string>(type: "TEXT", nullable: true),
                    Alder = table.Column<int>(type: "INTEGER", nullable: false),
                    NiceScore = table.Column<int>(type: "INTEGER", nullable: false),
                    ØnskelisteId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barn", x => x.BarnId);
                    table.ForeignKey(
                        name: "FK_Barn_Ønskelister_ØnskelisteId",
                        column: x => x.ØnskelisteId,
                        principalTable: "Ønskelister",
                        principalColumn: "ØnskelisteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ønsker",
                columns: table => new
                {
                    ØnskeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ØnskelisteId = table.Column<int>(type: "INTEGER", nullable: false),
                    LekeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Prioritet = table.Column<int>(type: "INTEGER", nullable: false),
                    Kommentar = table.Column<string>(type: "TEXT", nullable: true),
                    ErOppfylt = table.Column<bool>(type: "INTEGER", nullable: false),
                    ØnsketDato = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ønsker", x => x.ØnskeId);
                    table.ForeignKey(
                        name: "FK_Ønsker_Leker_LekeId",
                        column: x => x.LekeId,
                        principalTable: "Leker",
                        principalColumn: "LekeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ønsker_Ønskelister_ØnskelisteId",
                        column: x => x.ØnskelisteId,
                        principalTable: "Ønskelister",
                        principalColumn: "ØnskelisteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Alver",
                columns: new[] { "AlvId", "Avdeling", "ErAktiv", "Erfaring", "Navn", "Spesialitet" },
                values: new object[,]
                {
                    { 1, "Produksjon", true, 150, "Bjørnis Hammerhand", "Tresnikring" },
                    { 2, "Teknologi", true, 75, "Elina Elektronikk", "Elektronikk" },
                    { 3, "Design", true, 200, "Turid Tøysterk", "Tekstiler" }
                });

            migrationBuilder.InsertData(
                table: "Ønskelister",
                columns: new[] { "ØnskelisteId", "BarnId", "OpprettetDato", "Tittel" },
                values: new object[,]
                {
                    { 1, 0, new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emma sin ønskeliste" },
                    { 2, 0, new DateTime(2024, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Oliver sin ønskeliste" },
                    { 3, 0, new DateTime(2024, 10, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sofie sin ønskeliste" }
                });

            migrationBuilder.InsertData(
                table: "Barn",
                columns: new[] { "BarnId", "Adresse", "Alder", "Land", "Navn", "NiceScore", "ØnskelisteId" },
                values: new object[,]
                {
                    { 1, "Storgata 1", 7, "Norge", "Emma Hansen", 85, 1 },
                    { 2, "Fjellveien 23", 5, "Norge", "Oliver Johansen", 70, 2 },
                    { 3, "Skogsveien 45", 10, "Norge", "Sofie Berg", 95, 3 }
                });

            migrationBuilder.InsertData(
                table: "Leker",
                columns: new[] { "LekeId", "Aldersgruppe", "AlvId", "Antall", "Beskrivelse", "Navn", "OpprettetDato", "Status" },
                values: new object[,]
                {
                    { 1, "3-6 år", 1, 50, "Håndlaget tredukke", "Tredukke", new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Innpakket" },
                    { 2, "7-12 år", 2, 30, "Fjernstyrt bil med lys", "Robotbil", new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Testing" },
                    { 3, "0-3 år", 3, 100, "Myk bjørn", "Kosedyr", new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Montering" },
                    { 4, "10+ år", 1, 20, "1000 brikker", "Puslespill", new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Design" },
                    { 5, "5-10 år", 2, 15, "Rød sparkesykkel", "Sparkesykkel", new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Innpakket" },
                    { 6, "4-8 år", 3, 80, "Samling med eventyr", "Bok: Eventyr", new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Innpakket" }
                });

            migrationBuilder.InsertData(
                table: "Ønsker",
                columns: new[] { "ØnskeId", "ErOppfylt", "Kommentar", "LekeId", "Prioritet", "ØnskelisteId", "ØnsketDato" },
                values: new object[,]
                {
                    { 1, false, "Ønsker meg blå robotbil!", 2, 1, 1, new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, false, "Elsker å lese!", 6, 2, 1, new DateTime(2024, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, false, null, 5, 3, 1, new DateTime(2024, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, false, "Vil ha tredukke!", 1, 1, 2, new DateTime(2024, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, false, "Helst brun bjørn", 3, 2, 2, new DateTime(2024, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, true, null, 4, 1, 3, new DateTime(2024, 10, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, false, "Robotbil med lys!", 2, 2, 3, new DateTime(2024, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alver_Spesialitet",
                table: "Alver",
                column: "Spesialitet");

            migrationBuilder.CreateIndex(
                name: "IX_Barn_NiceScore",
                table: "Barn",
                column: "NiceScore");

            migrationBuilder.CreateIndex(
                name: "IX_Barn_ØnskelisteId",
                table: "Barn",
                column: "ØnskelisteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leker_AlvId",
                table: "Leker",
                column: "AlvId");

            migrationBuilder.CreateIndex(
                name: "IX_Leker_Status",
                table: "Leker",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Ønsker_LekeId",
                table: "Ønsker",
                column: "LekeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ønsker_ØnskelisteId_LekeId",
                table: "Ønsker",
                columns: new[] { "ØnskelisteId", "LekeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Barn");

            migrationBuilder.DropTable(
                name: "Ønsker");

            migrationBuilder.DropTable(
                name: "Leker");

            migrationBuilder.DropTable(
                name: "Ønskelister");

            migrationBuilder.DropTable(
                name: "Alver");
        }
    }
}
