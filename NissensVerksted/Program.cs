using Microsoft.EntityFrameworkCore;
using NissensVerksted.Data;
using System.Text.Json.Serialization; // VIKTIG: Legg til denne

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Fikser circular reference problemet
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Valgfritt: Gjør JSON mer leselig
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Legg til Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    {"ConnectionStrings:DefaultConnection", "Data Source=NissensVerksted.db"}
});

builder.Services.AddDbContext<VerkstedDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VerkstedDbContext>();
    context.Database.EnsureCreated();
}

app.Run();