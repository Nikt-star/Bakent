# ?? EJEMPLOS DE REQUESTS - POSTMAN COLLECTION

## Configuración Base
```
Base URL: http://localhost:5000/api
```

---

## 1?? PERFIL ENDPOINTS

### Crear Perfil
```http
POST /api/perfil
Content-Type: application/json

{
  "colaboradorId": 1,
  "resumen": "Ingeniero de Software Senior con 8 años de experiencia",
  "fotoUrl": "https://example.com/foto.jpg",
  "linkedin": "https://linkedin.com/in/miusuario",
  "telefono": "+34 912 345 678",
  "email": "usuario@empresa.com"
}
```

### Obtener Perfil
```http
GET /api/perfil/1
```

### Actualizar Perfil
```http
PUT /api/perfil/1
Content-Type: application/json

{
  "resumen": "Ingeniero Senior - Especialista en .NET",
  "telefono": "+34 912 345 679"
}
```

### Verificar Perfil Completo
```http
GET /api/perfil/1/completo
```

---

## 2?? VACANTE ENDPOINTS

### Crear Vacante
```http
POST /api/vacante
Content-Type: application/json

{
  "nombrePerfil": "Developer Backend Senior",
  "nivelDeseado": "Senior",
  "fechaInicio": "2024-01-15",
  "fechaUrgencia": "2024-02-15",
  "activa": true,
  "requisitosVacante": [
    {
      "skillID": 1,
      "nivelMinimoRequerido": "Avanzado"
    },
    {
      "skillID": 2,
      "nivelMinimoRequerido": "Intermedio"
    }
  ]
}
```

### Listar Vacantes Activas
```http
GET /api/vacante
```

### Aplicar a Vacante
```http
POST /api/vacante/1/aplicar
Content-Type: application/json

{
  "vacanteID": 1,
  "colaboradorID": 5,
  "interes": "Alto"
}
```

### Mostrar Interés
```http
POST /api/vacante/1/mostrar-interes
Content-Type: application/json

{
  "colaboradorID": 5,
  "interes": "Muy Alto"
}
```

### Mis Aplicaciones
```http
GET /api/vacante/colaborador/5/mis-aplicaciones
```

---

## 3?? BENEFICIOS ENDPOINTS

### Listar Beneficios
```http
GET /api/beneficios
```

### Listar por Categoría
```http
GET /api/beneficios/categoria/Salud
```

### Crear Beneficio (Admin)
```http
POST /api/beneficios
Content-Type: application/json

{
  "nombre": "Seguro Médico Privado",
  "descripcion": "Cobertura médica completa para empleados",
  "categoria": "Salud",
  "activo": true
}
```

---

## 4?? NOTIFICACIONES ENDPOINTS

### Obtener Notificaciones
```http
GET /api/notificaciones/colaborador/1
```

### Obtener No Leídas
```http
GET /api/notificaciones/colaborador/1/no-leidas
```

### Marcar Como Leída
```http
PUT /api/notificaciones/5/marcar-leida
```

### Contar No Leídas
```http
GET /api/notificaciones/colaborador/1/contar
```

---

## 5?? GAMIFICACIÓN ENDPOINTS

### Ver Total de Puntos
```http
GET /api/gamificacion/puntos/1
```

### Ver Historial de Puntos
```http
GET /api/gamificacion/historial/1
```

### Validar Certificación
```http
POST /api/gamificacion/validar-certificacion
?certificacionId=1&colaboradorId=1
```

### Marcar Perfil Completo
```http
POST /api/gamificacion/marcar-perfil-completo
Content-Type: application/json

{
  "colaboradorId": 1
}
```

### Leaderboard
```http
GET /api/gamificacion/leaderboard?top=10
```

---

## 6?? DESARROLLO ENDPOINTS

### Listar Cursos
```http
GET /api/desarrollo/cursos
```

### Listar Cursos por Categoría
```http
GET /api/desarrollo/cursos/categoria/Técnico
```

### Crear Curso (Admin)
```http
POST /api/desarrollo/cursos
Content-Type: application/json

{
  "nombre": "ASP.NET Core Avanzado",
  "descripcion": "Curso avanzado de ASP.NET Core con patrones empresariales",
  "categoria": "Técnico",
  "duracionHoras": 40
}
```

### Ver Progreso de Cursos
```http
GET /api/desarrollo/progreso/1
```

### Iniciar Curso
```http
POST /api/desarrollo/progreso
Content-Type: application/json

{
  "colaboradorId": 1,
  "cursoId": 2
}
```

### Actualizar Progreso
```http
PUT /api/desarrollo/progreso/1
Content-Type: application/json

{
  "porcentajeCompletacion": 50,
  "completado": false
}
```

### Marcar Curso Completado
```http
POST /api/gamificacion/marcar-curso-completado
?colaboradorId=1&cursoId=2
```

### Estadísticas
```http
GET /api/desarrollo/estadisticas/1
```

---

## 7?? PIPELINE ENDPOINTS

### Ver Candidatos en Pipeline
```http
GET /api/pipeline/vacante/1
```

### Candidatos por Estado
```http
GET /api/pipeline/vacante/1/por-estado
```

### Cambiar Estado de Candidato
```http
PUT /api/pipeline/1/estado
Content-Type: application/json

{
  "estado": "En_Evaluacion",
  "interes": "Alto",
  "notas": "Requiere entrevista técnica el 20/01"
}
```

### Resumen del Pipeline
```http
GET /api/pipeline/vacante/1/resumen
```

---

## 8?? RR.HH. ENDPOINTS

### Registrar Certificación
```http
POST /api/rrhh/certificaciones?colaboradorId=1
Content-Type: application/json

{
  "nombreCertificacion": "AWS Certified Solutions Architect",
  "fechaObtencion": "2023-06-15",
  "institucion": "Amazon"
}
```

### Listar Certificaciones
```http
GET /api/rrhh/certificaciones
```

### Certificaciones por Colaborador
```http
GET /api/rrhh/certificaciones/colaborador/1
```

### Actualizar Habilidad por Evaluación
```http
PUT /api/rrhh/habilidades/actualizar?colaboradorId=1&skillId=3
Content-Type: application/json

{
  "nivelDominio": "Avanzado"
}
```

### Suspender Usuario
```http
PUT /api/rrhh/usuarios/1/suspender
```

### Banear Usuario
```http
PUT /api/rrhh/usuarios/1/banear
```

### Reporte de Certificaciones
```http
GET /api/rrhh/reporte-certificaciones
```

### Reporte de Habilidades
```http
GET /api/rrhh/reporte-habilidades
```

---

## 9?? REPORTE ENDPOINTS

### Inventario de Skills
```http
GET /api/reporte/inventario-skills
```

### Inventario por Área
```http
GET /api/reporte/inventario-skills/por-area
```

### Inventario por Nivel
```http
GET /api/reporte/inventario-skills/por-nivel
```

### Reporte de Brechas
```http
GET /api/reporte/brechas
```

### Brechas por Rol
```http
GET /api/reporte/brechas/por-rol
```

### Alertas Críticas
```http
GET /api/reporte/alertas
```

### Exportar Brechas CSV
```http
GET /api/reporte/exportar-brechas-csv
```

### Dashboard
```http
GET /api/reporte/dashboard
```

---

## ?? RESPUESTAS ESPERADAS

### Success - 200 OK
```json
{
  "id": 1,
  "nombre": "...",
  "estado": "..."
}
```

### Created - 201 Created
```json
{
  "id": 1,
  "nombre": "...",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

### No Content - 204
(Sin respuesta JSON)

### Bad Request - 400
```json
{
  "message": "El campo requerido está vacío"
}
```

### Not Found - 404
```json
{
  "message": "Recurso no encontrado"
}
```

### Conflict - 409
```json
{
  "message": "El recurso ya existe"
}
```

---

## ?? NOTAS IMPORTANTES

1. **Autenticación:** Los endpoints requieren JWT token (por implementar)
2. **Headers:** Incluir `Content-Type: application/json` en POST/PUT
3. **Validaciones:** Todos los IDs deben existir en la BD
4. **Formatos de Fecha:** Usar formato ISO 8601 (YYYY-MM-DDTHH:mm:ssZ)
5. **Paginación:** Por implementar en endpoints con muchos resultados

---

## ?? CÓMO IMPORTAR EN POSTMAN

1. Copiar esta colección
2. Ir a Postman ? Import
3. Seleccionar "Raw text"
4. Pegar el contenido
5. Click en "Import"

¡Listo para usar! ??
