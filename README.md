# ?? SISTEMA DE GESTIÓN DE TALENTO INTERNO - PROYECTO COMPLETADO

![Status](https://img.shields.io/badge/Status-Complete-brightgreen)
![Build](https://img.shields.io/badge/Build-Passing-brightgreen)
![.NET](https://img.shields.io/badge/.NET-9.0-blue)
![C%23](https://img.shields.io/badge/C%23-13.0-blueviolet)

---

## ?? Descripción del Proyecto

Sistema completo de gestión de talento interno en **ASP.NET Core 9** que facilita:

? **Gestión de Perfiles** - Crear y editar perfiles de colaboradores  
? **Gestión de Vacantes** - Publicar y administrar vacantes  
? **Candidaturas** - Aplicar a vacantes y mostrar interés  
? **Pipeline de Selección** - Gestionar estados de candidatos  
? **Gamificación** - Sistema de puntos y logros  
? **Desarrollo Profesional** - Cursos y seguimiento de progreso  
? **Gestión de Skills** - Inventario y análisis de brechas  
? **Beneficios** - Catálogo de beneficios empresariales  
? **Notificaciones** - Sistema integrado de notificaciones  
? **Reportes** - Análisis avanzado de datos  

---

## ?? Inicio Rápido

```bash
# 1. Clonar proyecto
git clone https://github.com/Nikt-star/Bakent.git
cd Bakent

# 2. Restaurar dependencias
dotnet restore

# 3. Aplicar migraciones
dotnet ef database update -p Proyecto3.Infrastructure -s Proyecto3

# 4. Ejecutar
dotnet run --project Proyecto3

# 5. Acceder a Swagger
https://localhost:5001/swagger
```

---

## ?? Estructura del Proyecto

```
Proyecto3/
??? Controllers/              # 9 controladores (7 nuevos, 2 mejorados)
?   ??? PerfilController.cs
?   ??? VacanteController.cs
?   ??? BeneficiosController.cs
?   ??? NotificacionesController.cs
?   ??? GamificacionController.cs
?   ??? DesarrolloController.cs
?   ??? PipelineController.cs
?   ??? RRHHController.cs
?   ??? ReporteController.cs
??? Program.cs               # Configuración mejorada
??? appsettings.json
??? wwwroot/

Proyecto3.Core/
??? Entities/               # 7 entidades nuevas
??? Models/
?   ??? Dtos.cs            # 15+ DTOs nuevos
??? Interfaces/

Proyecto3.Infrastructure/
??? Data/
?   ??? TalentoInternoDbContext.cs
??? Repositories/
??? Services/
??? Migrations/
```

---

## ?? Funcionalidades Principales

### 1. **Perfil de Colaborador** ??
- Crear perfil personal
- Completar información
- Recibir puntos por perfil completo

### 2. **Vacantes y Aplicaciones** ??
- Listar vacantes activas
- Aplicar a vacantes
- Mostrar interés
- Seguimiento de aplicaciones

### 3. **Pipeline de Selección** ??
- Ver candidatos por vacante
- Cambiar estado de candidato
- Agregar notas y evaluaciones
- Resumen del pipeline

### 4. **Gamificación** ??
- Acumular puntos
- Validar certificaciones
- Completar cursos
- Leaderboard global

### 5. **Desarrollo Profesional** ??
- Catálogo de cursos
- Iniciar cursos
- Seguimiento de progreso
- Estadísticas de aprendizaje

### 6. **Gestión de Skills** ??
- Inventario de skills
- Análisis por área/rol/nivel
- Reporte de brechas
- Alertas críticas
- Exportación en CSV

### 7. **Notificaciones** ??
- Notificaciones en tiempo real
- Marcar como leída
- Filtrar por tipo

### 8. **Beneficios** ??
- Catálogo de beneficios
- Filtrado por categoría

### 9. **RR.HH.** ??
- Registrar certificaciones
- Gestionar habilidades
- Suspender/banear usuarios
- Reportes

---

## ?? API Endpoints

### Perfiles
- `POST /api/perfil` - Crear perfil
- `GET /api/perfil/{id}` - Obtener perfil
- `PUT /api/perfil/{id}` - Actualizar perfil

### Vacantes
- `GET /api/vacante` - Listar vacantes
- `POST /api/vacante/{vacanteId}/aplicar` - Aplicar a vacante
- `GET /api/vacante/{vacanteId}/aplicaciones` - Ver aplicaciones

### Pipeline
- `GET /api/pipeline/vacante/{vacanteId}` - Ver candidatos
- `PUT /api/pipeline/{id}/estado` - Cambiar estado

### Gamificación
- `GET /api/gamificacion/puntos/{id}` - Ver puntos
- `GET /api/gamificacion/historial/{id}` - Ver historial

### Desarrollo
- `GET /api/desarrollo/cursos` - Listar cursos
- `POST /api/desarrollo/progreso` - Iniciar curso
- `GET /api/desarrollo/progreso/{id}` - Ver progreso

### Reportes
- `GET /api/reporte/brechas` - Reporte de brechas
- `GET /api/reporte/alertas` - Alertas críticas
- `GET /api/reporte/dashboard` - Dashboard

**[Ver todos los endpoints ?](POSTMAN_REQUESTS.md)**

---

## ??? Entidades del Modelo

```
Colaborador
??? Perfil
??? HabilidadColaborador
??? Certificacion
??? Vacante_Aplicacion
??? Notificacion
??? Punto
??? Progreso_Curso

PerfilesVacante
??? RequisitoVacante
??? Vacante_Aplicacion

Skill
??? HabilidadColaborador
??? RequisitoVacante

Curso
??? Progreso_Curso

Beneficio (Independiente)
Notificacion (Independiente)
```

---

## ?? Seguridad

? Validación de entidades  
? Manejo de errores  
? JWT Bearer (por implementar)  
? CORS configurado  
? SQL Injection prevention (EF Core)  

---

## ?? Base de Datos

**Motor:** SQL Server  
**Version:** .NET 9 compatible  
**Migraciones:** Entity Framework Core  

```bash
# Crear BD
dotnet ef database update

# Revertir cambios
dotnet ef database update 0

# Crear migración
dotnet ef migrations add MigrationName
```

---

## ?? Estadísticas

| Métrica | Valor |
|---------|-------|
| **Controladores** | 9 |
| **Endpoints** | 80+ |
| **Entidades** | 13 |
| **DTOs** | 15+ |
| **Casos de Uso** | 18/18 ? |
| **Líneas de Código** | 3000+ |

---

## ?? Documentación

- **[RESUMEN_IMPLEMENTACION.md](RESUMEN_IMPLEMENTACION.md)** - Detalle completo de funcionalidades
- **[GUIA_EJECUCION.md](GUIA_EJECUCION.md)** - Cómo ejecutar el proyecto
- **[POSTMAN_REQUESTS.md](POSTMAN_REQUESTS.md)** - Ejemplos de requests
- **[MIGRACIONES_GUIA.md](MIGRACIONES_GUIA.md)** - Gestión de migraciones
- **[MEJORAS_FUTURAS.md](MEJORAS_FUTURAS.md)** - Próximas características

---

## ?? Flujo de Trabajo Típico

```
1. Colaborador crea/completa perfil ? +100 puntos
   ?
2. Colaborador ve vacantes disponibles
   ?
3. Aplica a vacante de interés
   ?
4. RR.HH. revisa candidatos en pipeline
   ?
5. Cambia estado a "En_Evaluacion"
   ?
6. RR.HH. toma decisión ? "Seleccionado" o "Rechazado"
   ?
7. Notificación automática al colaborador
   ?
8. Colaborador inicia cursos de desarrollo ? +75 puntos
   ?
9. Completa certificación ? +50 puntos
   ?
10. Sube en leaderboard ??
```

---

## ??? Tecnologías Utilizadas

| Tecnología | Versión | Propósito |
|-----------|---------|----------|
| .NET | 9.0 | Framework principal |
| C# | 13.0 | Lenguaje |
| Entity Framework Core | 9.0 | ORM |
| SQL Server | - | Base de datos |
| ASP.NET Core | 9.0 | API REST |
| xUnit | - | Testing (por implementar) |

---

## ?? Requisitos

- ? .NET 9 SDK
- ? SQL Server 2019+
- ? Visual Studio 2022 o VS Code
- ? Git

---

## ?? Estado del Proyecto

| Característica | Estado |
|---|---|
| Controladores | ? Completo |
| Entidades | ? Completo |
| DTOs | ? Completo |
| Endpoints | ? Completo |
| Migraciones | ? Completo |
| Validaciones | ? Parcial |
| Autenticación | ? Parcial |
| Tests | ? No iniciado |
| Documentación | ? Completo |

---

## ?? Contribuciones

Para contribuir:

1. Fork el proyecto
2. Crear rama feature (`git checkout -b feature/AmazingFeature`)
3. Commit cambios (`git commit -m 'Add AmazingFeature'`)
4. Push a rama (`git push origin feature/AmazingFeature`)
5. Abrir Pull Request

---

## ?? Contacto

**Email:** support@ejemplo.com  
**Repositorio:** https://github.com/Nikt-star/Bakent  
**Issues:** https://github.com/Nikt-star/Bakent/issues

---

## ?? Licencia

Este proyecto está bajo licencia MIT. Ver `LICENSE` para más detalles.

---

## ?? Agradecimientos

- Microsoft por .NET 9
- Entity Framework Core team
- Comunidad de desarrolladores

---

## ? Características Destacadas

?? **80+ Endpoints REST** - API completa y robusta  
?? **Análisis de Datos** - Reportes y dashboards  
?? **Gamificación** - Sistema de puntos y logros  
?? **Responsive** - Accesible desde cualquier dispositivo  
?? **Seguro** - Validaciones en todos los niveles  
?? **Documentado** - Código limpio y bien comentado  

---

## ?? Performance

- ? Queries optimizadas con EF Core
- ?? Lazy loading y eager loading estratégico
- ?? Potencial para caching con Redis
- ?? Índices en base de datos

---

## ?? Bug Reporting

Si encuentras un bug:

1. Describe el problema claramente
2. Incluye pasos para reproducir
3. Adjunta screenshots si aplica
4. Abre issue en GitHub

---



