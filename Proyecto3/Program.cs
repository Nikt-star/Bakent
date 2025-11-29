// Program.cs (Proyecto3/Program.cs)

using Microsoft.EntityFrameworkCore;
using Proyecto3.Core.Interfaces;
using Proyecto3.Infrastructure.Data;
using Proyecto3.Infrastructure.Repositories;
using Proyecto3.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Proyecto3.Core.Models;
using Proyecto3.Infrastructure.Seed;
using System.Text.Json.Serialization;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------
// ?? SECCIÓN DE REGISTRO DE SERVICIOS
// -----------------------------------------------------------

// 1. DB CONTEXT
builder.Services.AddDbContext<TalentoInternoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. REGISTRO DE REPOSITORIOS
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
builder.Services.AddScoped<IVacanteRepository, VacanteRepository>();

// 3. REGISTRO DE SERVICIOS
builder.Services.AddScoped<IColaboradorService, ColaboradorService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();

// 4. HTTP CLIENT FACTORY
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>(client =>
{
    // client.BaseAddress = new Uri("https://api-externa-hr.com/");
});

// 5. CORS - PARA CONECTAR CON FRONTEND (Quasar)
// Leer múltiples orígenes desde appsettings.json: Frontend:AllowedOrigins
var allowedOrigins = builder.Configuration.GetSection("Frontend:AllowedOrigins").Get<string[]>();
if (allowedOrigins == null || allowedOrigins.Length == 0)
{
    // Fallback a la clave Frontend:Url (compatibilidad con configuración anterior)
    var frontendUrl = builder.Configuration["Frontend:Url"] ?? "http://localhost:8080";
    allowedOrigins = new[] { frontendUrl };
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 6. SEGURIDAD JWT
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

// 7. JSON SERIALIZATION
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddOpenApi();

// -----------------------------------------------------------
// ?? CONSTRUIR Y CONFIGURAR PIPELINE
// -----------------------------------------------------------

var app = builder.Build();

// CORS MIDDLEWARE - ACTIVAR ANTES DE AUTENTICACIÓN
app.UseCors("FrontendPolicy");

// MIGRACIONES Y SEED
var applyMigrations = app.Environment.IsDevelopment()
                      || System.Environment.GetEnvironmentVariable("APPLY_MIGRATIONS") == "true";

if (applyMigrations)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TalentoInternoDbContext>();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        try
        {
            var canConnect = await context.Database.CanConnectAsync(cts.Token);
            if (!canConnect)
            {
                System.Console.WriteLine("?? No se pudo conectar a la base de datos.");
            }
            else
            {
                var pending = await context.Database.GetPendingMigrationsAsync(cts.Token);
                if (pending != null && pending.Any())
                {
                    System.Console.WriteLine("? Aplicando migraciones pendientes...");
                    await context.Database.MigrateAsync(cts.Token);
                    System.Console.WriteLine("? Migraciones aplicadas.");
                }

                await DataSeeder.SeedAsync(context);
                System.Console.WriteLine("? Seed ejecutado.");
            }
        }
        catch (OperationCanceledException)
        {
            System.Console.WriteLine("?? Timeout al conectar a la BD.");
        }
    }
    catch (Exception ex)
    {
        System.Console.WriteLine($"? Error: {ex.Message}");
    }
}

// SWAGGER EN DESARROLLO
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// STATIC FILES
app.UseDefaultFiles();
app.UseStaticFiles();

// MIDDLEWARE DE SEGURIDAD
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
