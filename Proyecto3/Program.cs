// Program.cs (Proyecto3/Program.cs)

using Microsoft.EntityFrameworkCore; // Necesario para UseSqlServer
using Proyecto3.Core.Interfaces;
using Proyecto3.Infrastructure.Data;
using Proyecto3.Infrastructure.Repositories;
using Proyecto3.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Necesario para JWT
using Microsoft.IdentityModel.Tokens; // Necesario para JWT
using System.Text;
using Proyecto3.Core.Models; // Asumiendo que JwtSettings está aquí
using Proyecto3.Infrastructure.Seed; // Seeder
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------
// 📌 SECCIÓN DE REGISTRO DE SERVICIOS (ANTES de builder.Build())
// -----------------------------------------------------------

// 1. DB CONTEXT
builder.Services.AddDbContext<TalentoInternoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. REGISTRO DE REPOSITORIOS
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
builder.Services.AddScoped<IVacanteRepository, VacanteRepository>();

// 3. REGISTRO DE SERVICIOS (Lógica de Negocio y Matching)
builder.Services.AddScoped<IColaboradorService, ColaboradorService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();

// 4. API EXTERNA (Eleazar) - Integración con HttpClientFactory
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>(client =>
{
    // Puedes configurar la URL base si fuera una API real
    // client.BaseAddress = new Uri("https://api-externa-hr.com/"); 
});

// 5. SEGURIDAD JWT (Eleazar) - Autenticación
// Asegúrate de tener la sección "JwtSettings" en appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecurityKey ?? string.Empty))
    };
});

// Servicios de Framework: habilitar manejo de ciclos en JSON para evitar JsonException
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddOpenApi(); // Para Swagger/OpenAPI

// -----------------------------------------------------------
// 📌 CONSTRUIR Y CONFIGURAR PIPELINE (DESPUÉS de builder.Build())
// -----------------------------------------------------------

var app = builder.Build();

// Seeder: aplicar migraciones y poblar datos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TalentoInternoDbContext>();
    // Aplicar migraciones en inicio (útil en desarrollo)
    context.Database.Migrate();
    // Poblar datos si está vacío
    await DataSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Servir archivos estáticos desde wwwroot (formularios HTML)
app.UseDefaultFiles();
app.UseStaticFiles();

// 6. MIDDLEWARE DE SEGURIDAD
// DEBE IR ANTES de app.UseAuthorization() y app.MapControllers()
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();