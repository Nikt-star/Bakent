## ?? GUÍA DE SOLUCIONES PARA MIGRACIONES EN ENTITY FRAMEWORK CORE

### Problema Original
```csharp
context.Database.Migrate(); // ?? Acción no controlada por el usuario
```

Esta línea genera una advertencia porque las migraciones se aplican automáticamente sin intervención del usuario.

---

### ? SOLUCIÓN 1: Aplicar Migraciones Solo en Desarrollo (RECOMENDADA - Implementada)

**Ventajas:**
- ? Seguro para producción
- ? Solo se aplica en desarrollo
- ? Verifica migraciones pendientes
- ? Incluye manejo de errores

```csharp
if (app.Environment.IsDevelopment())
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TalentoInternoDbContext>();
            
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
```

---

### ?? SOLUCIÓN 2: Usar Comando dotnet para Migraciones (Alternativa Manual)

**En lugar de aplicar migraciones automáticamente, hazlo manualmente:**

```bash
# Crear una nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update

# Ver migraciones pendientes
dotnet ef migrations list

# Revertir última migración
dotnet ef database update NombreMigracionAnterior
```

**Ventajas:**
- ? Control total del usuario
- ? Auditable
- ? Seguro para producción
- ? Permite revisar cambios antes de aplicar

---

### ?? SOLUCIÓN 3: Separar Lógica en Extensión (Patrón Limpio)

```csharp
// Crear archivo: Extensions/DatabaseExtensions.cs
public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<TalentoInternoDbContext>();
            
            try
            {
                var pendingMigrations = await context.Database
                    .GetPendingMigrationsAsync();
                    
                if (pendingMigrations.Any())
                {
                    Console.WriteLine("?? Aplicando migraciones...");
                    await context.Database.MigrateAsync();
                    Console.WriteLine("? Migraciones completadas.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error: {ex.Message}");
                throw;
            }
        }
    }
}

// En Program.cs:
if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationsAsync();
}
```

---

### ?? SOLUCIÓN 4: Usar Service Scope Factory (Producción Segura)

```csharp
// En Program.cs
var serviceProvider = app.Services;
using (var scope = serviceProvider.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TalentoInternoDbContext>();

    // Verifica si la BD existe
    if (await context.Database.CanConnectAsync())
    {
        var migrations = await context.Database.GetPendingMigrationsAsync();
        if (migrations.Any())
        {
            if (app.Environment.IsDevelopment())
            {
                await context.Database.MigrateAsync();
            }
            else
            {
                // En producción: registra advertencia sin aplicar
                Console.WriteLine($"?? Migraciones pendientes detectadas: {string.Join(", ", migrations)}");
            }
        }
    }
}
```

---

### ?? COMPARATIVA DE SOLUCIONES

| Solución | Desarrollo | Producción | Riesgo | Control |
|----------|-----------|-----------|--------|---------|
| Auto en Startup | ? | ? | Alto | Bajo |
| Comando CLI (Recomendado) | ? | ? | Bajo | Alto |
| Extensión Limpia | ? | ? | Bajo | Medio |
| Service Scope Factory | ? | ? | Bajo | Alto |

---

### ?? RECOMENDACIÓN FINAL

**Para tu proyecto:**

```csharp
// EN DESARROLLO: Usar la Solución 1 (ya implementada)
if (app.Environment.IsDevelopment())
{
    // Las migraciones se aplican automáticamente
}

// EN PRODUCCIÓN: Usar CLI
// dotnet ef database update --configuration Release
```

---

### ?? PASOS PARA CREAR MIGRACIONES NUEVAS

1. **Realizar cambios en entidades**
   ```csharp
   public class MiEntidad
   {
       public int Id { get; set; }
       public string Nombre { get; set; } // Campo nuevo
   }
   ```

2. **Crear migración**
   ```bash
   dotnet ef migrations add AgregarNombreAMiEntidad
   ```

3. **Revisar el archivo de migración** generado en `Migrations/`

4. **Aplicar la migración**
   ```bash
   # En desarrollo (automático en Program.cs)
   # O manual:
   dotnet ef database update
   ```

5. **Commit a Git**
   ```bash
   git add Migrations/
   git commit -m "chore: agregar migración AgregarNombreAMiEntidad"
   ```

---

### ?? MEJORES PRÁCTICAS

? **Usar CLI para migraciones en producción**
? **Revisar archivos de migración antes de aplicar**
? **Mantener migraciones en control de versiones**
? **Usar backups antes de aplicar a producción**
? **Documentar cambios en BD**
? **Usar transacciones en migraciones complejas**
