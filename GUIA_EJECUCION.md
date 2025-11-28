# ?? GUÍA DE EJECUCIÓN - SISTEMA DE GESTIÓN DE TALENTO INTERNO

## ?? Requisitos Previos

? **.NET 9 SDK** instalado  
? **SQL Server** (local o remoto)  
? **Visual Studio 2022** o **VS Code**  
? **Git**

---

## 1?? CLONAR Y PREPARAR EL PROYECTO

```bash
# Clonar repositorio
git clone https://github.com/Nikt-star/Bakent.git
cd Bakent

# Navegar a la carpeta del proyecto
cd Proyecto3
```

---

## 2?? CONFIGURAR LA BASE DE DATOS

### A. Actualizar Connection String

Editar `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TalentoInternoDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "Issuer": "http://localhost:5000",
    "Audience": "http://localhost:5000",
    "SecurityKey": "tu-clave-secreta-muy-segura-aqui-minimo-32-caracteres"
  }
}
```

### B. Crear Migraciones (si no existen)

```bash
# En la raíz del proyecto
dotnet ef migrations add InitialCreate -p Proyecto3.Infrastructure -s Proyecto3

# Aplicar migraciones
dotnet ef database update -p Proyecto3.Infrastructure -s Proyecto3
```

### C. Verificar Base de Datos

```bash
# Con SQL Server Management Studio
# - Server: (localdb)\mssqllocaldb
# - Database: TalentoInternoDb
```

---

## 3?? RESTAURAR DEPENDENCIAS Y COMPILAR

```bash
# Restaurar paquetes NuGet
dotnet restore

# Compilar la solución
dotnet build

# Compilar modo Release (producción)
dotnet build --configuration Release
```

---

## 4?? EJECUTAR EL PROYECTO

### Opción A: Desde Visual Studio
1. Abrir `Proyecto3.sln`
2. Click derecho en "Proyecto3" ? "Set as Startup Project"
3. Presionar `F5` o Click en "Start Debugging"

### Opción B: Desde Terminal
```bash
cd Proyecto3
dotnet run
```

### Opción C: Con IIS Express
```bash
dotnet run --launch-profile "IIS Express"
```

---

## 5?? VERIFICAR QUE ESTÁ FUNCIONANDO

### A. Acceder a Swagger/OpenAPI
```
https://localhost:5001/openapi/v1.json
```

### B. Verificar Database Migrate
```
? Aplicando migraciones pendientes...
? Migraciones aplicadas correctamente.
? Datos de prueba cargados.
```

### C. Probar un Endpoint
```bash
curl http://localhost:5000/api/Colaborador
```

---

## 6?? ESTRUCTURA DE CARPETAS

```
Proyecto3/
??? Controllers/
?   ??? PerfilController.cs                 ? Nuevo
?   ??? VacanteController.cs                ? Mejorado
?   ??? BeneficiosController.cs             ? Nuevo
?   ??? NotificacionesController.cs         ? Nuevo
?   ??? GamificacionController.cs           ? Nuevo
?   ??? DesarrolloController.cs             ? Mejorado
?   ??? PipelineController.cs               ? Nuevo
?   ??? RRHHController.cs                   ? Nuevo
?   ??? ReporteController.cs                ? Nuevo
?   ??? ColaboradorController.cs            Existente
?   ??? MatchingController.cs               Existente
?   ??? SkillsController.cs                 Existente
??? Program.cs                              ? Actualizado
??? appsettings.json
??? wwwroot/
    ??? (Archivos estáticos HTML)

Proyecto3.Core/
??? Entities/
?   ??? Perfil.cs                          ? Nuevo
?   ??? Vacante_Aplicacion.cs              ? Nuevo
?   ??? Beneficio.cs                       ? Nuevo
?   ??? Notificacion.cs                    ? Nuevo
?   ??? Punto.cs                           ? Nuevo
?   ??? Curso.cs                           ? Nuevo
?   ??? Progreso_Curso.cs                  ? Nuevo
?   ??? (Otros existentes)
??? Models/
?   ??? Dtos.cs                            ? Actualizado
??? Interfaces/

Proyecto3.Infrastructure/
??? Data/
?   ??? TalentoInternoDbContext.cs         ? Actualizado
??? Migrations/
?   ??? (Migraciones EF Core)
??? Repositories/
??? Services/
??? Seed/
    ??? DataSeeder.cs
```

---

## 7?? COMANDOS ÚTILES

### Base de Datos

```bash
# Crear migración
dotnet ef migrations add "MigracionName" -p Proyecto3.Infrastructure -s Proyecto3

# Ver historial de migraciones
dotnet ef migrations list -p Proyecto3.Infrastructure -s Proyecto3

# Revertir a migración anterior
dotnet ef database update "MigracionAnterior" -p Proyecto3.Infrastructure -s Proyecto3

# Revertir todos los cambios
dotnet ef database update 0 -p Proyecto3.Infrastructure -s Proyecto3

# Eliminar última migración
dotnet ef migrations remove -p Proyecto3.Infrastructure -s Proyecto3
```

### Compilación y Ejecución

```bash
# Compilar sin ejecutar
dotnet build

# Compilar y ejecutar
dotnet run

# Ejecutar en producción
dotnet publish -c Release
dotnet Proyecto3.dll

# Limpiar compilación
dotnet clean
```

### Testing

```bash
# Ejecutar pruebas
dotnet test

# Con coverage
dotnet test /p:CollectCoverage=true
```

---

## 8?? CONFIGURACIÓN DE PUERTOS

El proyecto se ejecuta en los siguientes puertos por defecto:

```
HTTP:   http://localhost:5000
HTTPS:  https://localhost:5001
```

Para cambiar, editar `Program.cs`:

```csharp
app.Run("http://localhost:3000");
```

---

## 9?? TROUBLESHOOTING

### ? Error: "Database connection failed"
**Solución:**
```bash
# Verificar connection string en appsettings.json
# Iniciar SQL Server
# Reintentar dotnet ef database update
```

### ? Error: "Port 5000 already in use"
**Solución:**
```bash
# Linux/Mac
lsof -i :5000
kill -9 <PID>

# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### ? Error: "Migraciones pendientes"
**Solución:**
```bash
dotnet ef database update -p Proyecto3.Infrastructure -s Proyecto3
```

### ? Error: "NuGet packages not restored"
**Solución:**
```bash
dotnet nuget locals all --clear
dotnet restore
```

---

## ?? TESTING DE ENDPOINTS

### Usar cURL

```bash
# GET
curl -X GET http://localhost:5000/api/Colaborador

# POST
curl -X POST http://localhost:5000/api/perfil \
  -H "Content-Type: application/json" \
  -d '{"colaboradorId":1,"resumen":"Test"}'

# PUT
curl -X PUT http://localhost:5000/api/perfil/1 \
  -H "Content-Type: application/json" \
  -d '{"resumen":"Updated"}'

# DELETE
curl -X DELETE http://localhost:5000/api/beneficios/1
```

### Usar Postman

1. Importar `POSTMAN_REQUESTS.md`
2. Configurar Base URL: `http://localhost:5000`
3. Ejecutar requests

### Usar Insomnia

Similar a Postman, importar colección y ejecutar requests.

---

## 1??1?? DEPLOYMENT EN PRODUCCIÓN

### Opción A: IIS (Windows)

```bash
# 1. Publicar
dotnet publish -c Release -o ./publish

# 2. Copiar carpeta publish a IIS
# 3. Crear Application Pool en IIS
# 4. Crear sitio web apuntando a la carpeta publish
```

### Opción B: Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /src
COPY . .
RUN dotnet build "Proyecto3/Proyecto3.csproj" -c Release

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=builder /src/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Proyecto3.dll"]
```

```bash
# Construir imagen
docker build -t talento-interno:latest .

# Ejecutar contenedor
docker run -p 5000:5000 talento-interno:latest
```

### Opción C: Linux (systemd)

```bash
# 1. Publicar
dotnet publish -c Release -o /opt/talento-interno

# 2. Crear servicio
sudo nano /etc/systemd/system/talento-interno.service
```

```ini
[Unit]
Description=Talento Interno API
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /opt/talento-interno/Proyecto3.dll
Restart=always
User=www-data
Environment="ASPNETCORE_ENVIRONMENT=Production"

[Install]
WantedBy=multi-user.target
```

```bash
# 3. Habilitar servicio
sudo systemctl daemon-reload
sudo systemctl enable talento-interno
sudo systemctl start talento-interno
```

---

## 1??2?? MONITOREO Y LOGGING

### Configurar Logging

Editar `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Proyecto3": "Debug"
    }
  }
}
```

### Ver Logs en Tiempo Real

```bash
# Terminal
dotnet run

# O en archivo
dotnet run > app.log 2>&1
tail -f app.log
```

---

## ?? RESUMEN RÁPIDO

```bash
# 1. Clonar
git clone https://github.com/Nikt-star/Bakent.git && cd Bakent/Proyecto3

# 2. Restaurar
dotnet restore

# 3. Migraciones
dotnet ef database update -p ../Proyecto3.Infrastructure -s .

# 4. Ejecutar
dotnet run

# 5. Acceder
# Browser: https://localhost:5001
# Swagger: https://localhost:5001/swagger (si está habilitado)
```

---

## ?? SOPORTE

Para problemas o preguntas:
- ?? Email: support@ejemplo.com
- ?? Documentación: `/RESUMEN_IMPLEMENTACION.md`
- ?? Bug Reports: https://github.com/Nikt-star/Bakent/issues

---

**¡Listo para ejecutar! ??**
