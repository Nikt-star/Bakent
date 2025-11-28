# ? PREGUNTAS FRECUENTES (FAQ)

## General

### ¿Cómo inicio el proyecto?
```bash
dotnet run --project Proyecto3
```
Accede a https://localhost:5001

### ¿Cómo cambio el puerto?
En `Properties/launchSettings.json`:
```json
{
  "Proyecto3": {
    "commandName": "Project",
    "dotnetRunMessages": true,
    "launchBrowser": true,
    "applicationUrl": "https://localhost:7000;http://localhost:5000",
    ...
  }
}
```

### ¿Dónde cambio la connection string?
En `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tu_servidor;Database=TalentoInternoDb;..."
  }
}
```

---

## Base de Datos

### ¿Cómo creo una nueva migración?
```bash
dotnet ef migrations add NombreDeLaMigracion -p Proyecto3.Infrastructure -s Proyecto3
```

### ¿Cómo revierço la última migración?
```bash
dotnet ef database update NombreDeLaMigracionAnterior -p Proyecto3.Infrastructure -s Proyecto3
```

### ¿Cómo veo todas las migraciones?
```bash
dotnet ef migrations list -p Proyecto3.Infrastructure -s Proyecto3
```

### ¿Qué hago si la migración no se aplica?
```bash
# Opción 1: Aplicar con verbose
dotnet ef database update --verbose -p Proyecto3.Infrastructure -s Proyecto3

# Opción 2: Borrar BD y comenzar
dotnet ef database drop -p Proyecto3.Infrastructure -s Proyecto3
dotnet ef database update -p Proyecto3.Infrastructure -s Proyecto3
```

---

## Migraciones

### ¿Es seguro usar `Migrate()` en producción?
? **NO**. En producción:
```bash
# Usar CLI
dotnet ef database update --configuration Release

# O aplicar antes de desplegar
```

### ¿Cómo deshabilito las migraciones automáticas?
En `Program.cs`:
```csharp
// Comentar o quitar esta sección
/*
if (app.Environment.IsDevelopment())
{
    await context.Database.MigrateAsync();
}
*/
```

### ¿Puedo aplicar migraciones a múltiples BDs?
Sí, crea múltiples `DbContext`:
```csharp
services.AddDbContext<TalentoInternoDbContext>(opts => 
    opts.UseSqlServer(conn1));
    
services.AddDbContext<OtherDbContext>(opts => 
    opts.UseSqlServer(conn2));
```

---

## Controladores

### ¿Cómo agregar un nuevo endpoint?
```csharp
[HttpPost("ruta")]
[ProducesResponseType(201)]
public async Task<IActionResult> MiEndpoint([FromBody] MiDto dto)
{
    // Lógica aquí
    return Ok(resultado);
}
```

### ¿Cuáles son los status codes correctos?
```
200 - OK (GET exitoso)
201 - Created (POST exitoso)
204 - No Content (PUT/DELETE exitoso)
400 - Bad Request (Validación fallida)
401 - Unauthorized (No autenticado)
403 - Forbidden (No autorizado)
404 - Not Found (Recurso no existe)
409 - Conflict (Duplicado)
500 - Server Error
```

### ¿Cómo validar parámetros?
```csharp
if (string.IsNullOrWhiteSpace(dto.Nombre))
    return BadRequest(new { message = "Nombre es requerido" });

var existe = await _context.Colaboradores.FindAsync(id);
if (existe == null)
    return NotFound(new { message = "Colaborador no encontrado" });
```

---

## DTOs

### ¿Por qué usar DTOs?
? Seguridad - No expongas entidades internas  
? Validación - Control de datos de entrada  
? Flexibilidad - Diferentes vistas de datos  
? Performance - Solo campos necesarios

### ¿Cómo crear un DTO?
```csharp
public class MiDto
{
    public string Nombre { get; set; }
    public string Email { get; set; }
    // Otros campos
}
```

### ¿Cómo mapear entre DTO y Entidad?
```csharp
// Manual
var entidad = new Colaborador
{
    NombreCompleto = dto.Nombre,
    Email = dto.Email
};

// Con AutoMapper (recomendado)
var entidad = _mapper.Map<Colaborador>(dto);
```

---

## Autenticación

### ¿Cómo agrego autenticación JWT?
```csharp
// Ya está configurado en Program.cs
// Solo necesita login endpoint
[HttpPost("login")]
public IActionResult Login([FromBody] LoginRequest request)
{
    // Validar credenciales
    var token = GenerateToken(user);
    return Ok(new { token });
}
```

### ¿Cómo protejo un endpoint?
```csharp
[Authorize]
[HttpGet]
public async Task<IActionResult> GetDatos()
{
    // Solo accesible con token válido
}
```

### ¿Cómo agrego roles?
```csharp
[Authorize(Roles = "Admin,RRH")]
[HttpPost]
public async Task<IActionResult> AdminAction()
{
    // Solo Admin o RRH
}
```

---

## Errores Comunes

### Error: "DbContext no puede crear instancias"
**Solución:** Asegurate de que DbContext está registrado en Program.cs
```csharp
builder.Services.AddDbContext<TalentoInternoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Error: "Port already in use"
**Solución:**
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID [PID] /F

# Linux/Mac
lsof -i :5000
kill -9 [PID]
```

### Error: "Package version conflicts"
**Solución:**
```bash
dotnet nuget locals all --clear
dotnet restore
```

### Error: "Migraciones pendientes"
**Solución:**
```bash
dotnet ef database update
```

### Error: "NullReferenceException"
**Solución:** Verifica Include() para lazy loading
```csharp
var vacante = await _context.Set<PerfilesVacante>()
    .Include(v => v.RequisitosVacante)  // ? Agregar Include
    .FirstOrDefaultAsync(v => v.VacanteID == id);
```

---

## Testing

### ¿Cómo ejecuto tests?
```bash
dotnet test
```

### ¿Cómo creo un test?
```csharp
[Fact]
public async Task MiTest()
{
    // Arrange
    var input = new MiDto { };
    
    // Act
    var result = await _controller.MiMetodo(input);
    
    // Assert
    Assert.NotNull(result);
}
```

### ¿Cómo mockeo DbContext?
```csharp
var mockContext = new Mock<TalentoInternoDbContext>();
mockContext.Setup(m => m.Colaboradores)
    .Returns(GetTestData().AsQueryable().BuildMockDbSet().Object);
```

---

## Performance

### ¿Cómo mejoro performance?
1. **Índices en BD**
   ```csharp
   modelBuilder.Entity<Vacante_Aplicacion>()
       .HasIndex(a => a.VacanteID);
   ```

2. **Proyecciones**
   ```csharp
   .Select(a => new { a.ID, a.Nombre })
   ```

3. **AsNoTracking()**
   ```csharp
   .AsNoTracking()  // Para lectura
   ```

4. **Caching**
   ```csharp
   _cache.Get("key") ?? FetchData();
   ```

---

## Deployment

### ¿Cómo despliego a Azure?
```bash
# 1. Crear App Service
az appservice create --resource-group myGroup --plan myPlan --name myApp

# 2. Publicar
dotnet publish -c Release
# Subir carpeta publish a Azure
```

### ¿Cómo despliego a Docker?
```bash
docker build -t app:latest .
docker run -p 5000:5000 app:latest
```

### ¿Cómo configuro HTTPS?
```csharp
app.UseHttpsRedirection();
app.UseHsts();
```

---

## Logs y Debugging

### ¿Cómo veo logs?
```csharp
_logger.LogInformation("Mensaje: {id}", id);
_logger.LogError("Error: {ex}", exception);
```

### ¿Cómo debugueo?
```bash
# VS Code
# Presiona F5 para iniciar debugging

# Visual Studio
# Presiona F5 o Debug ? Start Debugging
```

### ¿Cómo agrego breakpoints?
Visual Studio/VS Code:
- Haz click en el número de línea
- O presiona F9

---

## Git

### ¿Cómo ignoro archivos?
En `.gitignore`:
```
bin/
obj/
.vs/
*.user
appsettings.Production.json
```

### ¿Cómo hago commit?
```bash
git add .
git commit -m "feat: agregar nuevo endpoint"
git push origin master
```

### ¿Cómo creo rama?
```bash
git checkout -b feature/nueva-funcionalidad
# Hacer cambios
git push origin feature/nueva-funcionalidad
# Pull request
```

---

## Seguridad

### ¿Cómo aseguro contraseñas?
```csharp
using System.Security.Cryptography;

var hash = BCrypt.Net.BCrypt.HashPassword(password);
bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
```

### ¿Cómo protejo secrets?
```bash
# User Secrets (desarrollo)
dotnet user-secrets set "MyKey" "MyValue"

# Environment Variables (producción)
set ASPNETCORE_ConnectionStrings__DefaultConnection=...
```

### ¿Cómo implemento CORS?
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod());
});

app.UseCors("AllowAll");
```

---

## Utilidades

### ¿Cómo genero un GUID?
```csharp
Guid id = Guid.NewGuid();
string guidString = Guid.NewGuid().ToString();
```

### ¿Cómo formateo fechas?
```csharp
DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
DateTime.UtcNow  // Usar UTC siempre
```

### ¿Cómo hago paginación?
```csharp
var page = 1;
var pageSize = 10;
var data = await query
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

---

## Recursos Útiles

?? **Documentación:**
- [Microsoft Docs](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)

?? **Tutoriales:**
- [Microsoft Learn](https://learn.microsoft.com/)
- [Pluralsight](https://www.pluralsight.com/)
- [Udemy](https://www.udemy.com/)

?? **Herramientas:**
- [Postman](https://www.postman.com/)
- [SQL Server Management Studio](https://docs.microsoft.com/ssms/)
- [Visual Studio Code](https://code.visualstudio.com/)

---

## ¿Aún tienes preguntas?

?? Contacta: support@ejemplo.com  
?? Abre issue en GitHub  
?? Participa en discusiones

---

**Última actualización:** Enero 2024
