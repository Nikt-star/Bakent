# ?? EJEMPLO COMPLETO DE FLUJO - PASO A PASO

## Escenario: Juan busca una nueva oportunidad laboral

### ?? Paso 1: Juan crea su perfil

```http
POST /api/perfil
Content-Type: application/json

{
  "colaboradorId": 1,
  "resumen": "Ingeniero de Software Senior con 10 años en desarrollo backend",
  "fotoUrl": "https://example.com/juan.jpg",
  "linkedin": "https://linkedin.com/in/juan-dev",
  "telefono": "+34 912 345 678",
  "email": "juan@empresa.com"
}
```

**Respuesta:**
```json
{
  "perfilID": 1,
  "colaboradorID": 1,
  "resumen": "Ingeniero de Software...",
  "perfilCompleto": true,
  "fechaCreacion": "2024-01-15T10:30:00Z"
}
```

? **Juan gana 100 puntos por perfil completo**

---

### ?? Paso 2: El sistema notifica a Juan

```json
{
  "notificacionID": 1,
  "colaboradorID": 1,
  "titulo": "¡Perfil Completado!",
  "mensaje": "Tu perfil está completo. Has ganado 100 puntos.",
  "tipo": "Puntos",
  "leida": false,
  "fechaCreacion": "2024-01-15T10:30:05Z"
}
```

---

### ?? Paso 3: Juan visualiza sus puntos

```http
GET /api/gamificacion/puntos/1
```

**Respuesta:**
```json
{
  "totalPuntos": 100
}
```

---

### ?? Paso 4: RR.HH. publica una vacante

```http
POST /api/vacante
Content-Type: application/json

{
  "nombrePerfil": "Tech Lead - Backend",
  "nivelDeseado": "Senior",
  "fechaInicio": "2024-02-01",
  "fechaUrgencia": "2024-02-15",
  "activa": true,
  "requisitosVacante": [
    {
      "skillID": 1,
      "nivelMinimoRequerido": "Avanzado"
    },
    {
      "skillID": 2,
      "nivelMinimoRequerido": "Avanzado"
    },
    {
      "skillID": 3,
      "nivelMinimoRequerido": "Intermedio"
    }
  ]
}
```

**Respuesta:**
```json
{
  "vacanteID": 5,
  "nombrePerfil": "Tech Lead - Backend",
  "nivelDeseado": "Senior",
  "activa": true
}
```

---

### ?? Paso 5: El sistema busca candidatos automáticos

```http
GET /api/matching/candidates/5
```

**Respuesta:**
```json
[
  {
    "colaboradorID": 1,
    "nombreCompleto": "Juan García",
    "porcentajeMatch": 85.5,
    "skillsFaltantes": ["Docker (Tiene: Intermedio, Requiere: Avanzado)"]
  },
  {
    "colaboradorID": 3,
    "nombreCompleto": "María López",
    "porcentajeMatch": 92.0,
    "skillsFaltantes": []
  }
]
```

---

### ?? Paso 6: Juan ve la vacante disponible

```http
GET /api/vacante
```

**Respuesta:**
```json
[
  {
    "vacanteID": 5,
    "nombrePerfil": "Tech Lead - Backend",
    "nivelDeseado": "Senior",
    "fechaUrgencia": "2024-02-15",
    "activa": true
  }
]
```

---

### ?? Paso 7: Juan aplica a la vacante

```http
POST /api/vacante/5/aplicar
Content-Type: application/json

{
  "vacanteID": 5,
  "colaboradorID": 1,
  "interes": "Alto"
}
```

**Respuesta:**
```json
{
  "aplicacionID": 1,
  "vacanteID": 5,
  "colaboradorID": 1,
  "fechaAplicacion": "2024-01-15T14:00:00Z",
  "estado": "Aplicado",
  "interes": "Alto"
}
```

---

### ?? Paso 8: RR.HH. ve los candidatos en el pipeline

```http
GET /api/pipeline/vacante/5
```

**Respuesta:**
```json
[
  {
    "aplicacionID": 1,
    "colaboradorID": 1,
    "colaborador": {
      "nombreCompleto": "Juan García",
      "rolActual": "Senior Developer",
      "cuentaProyecto": "ACME-001"
    },
    "estado": "Aplicado",
    "interes": "Alto",
    "fechaAplicacion": "2024-01-15T14:00:00Z"
  }
]
```

---

### ?? Paso 9: RR.HH. cambia estado a "En_Evaluacion"

```http
PUT /api/pipeline/1/estado
Content-Type: application/json

{
  "estado": "En_Evaluacion",
  "interes": "Alto",
  "notas": "Perfil muy interesante, requiere entrevista técnica el 20/01"
}
```

**Respuesta:** `204 No Content`

---

### ?? Paso 10: Juan recibe notificación

Sistema automático genera:
```json
{
  "notificacionID": 2,
  "colaboradorID": 1,
  "titulo": "Actualización de Candidatura",
  "mensaje": "Tu aplicación está siendo evaluada",
  "tipo": "Vacante",
  "leida": false,
  "fechaCreacion": "2024-01-15T14:05:00Z"
}
```

---

### ?? Paso 11: Juan decide ampliar sus skills con un curso

```http
GET /api/desarrollo/cursos/categoria/Técnico
```

**Respuesta:**
```json
[
  {
    "cursoID": 1,
    "nombre": "Docker & Kubernetes Advanced",
    "descripcion": "Domina Docker y Kubernetes en producción",
    "categoria": "Técnico",
    "duracionHoras": 40
  }
]
```

---

### ?? Paso 12: Juan inicia el curso

```http
POST /api/desarrollo/progreso
Content-Type: application/json

{
  "colaboradorId": 1,
  "cursoId": 1
}
```

**Respuesta:**
```json
{
  "progresoID": 1,
  "colaboradorID": 1,
  "cursoID": 1,
  "porcentajeCompletacion": 0,
  "completado": false,
  "fechaInicio": "2024-01-16T09:00:00Z"
}
```

---

### ?? Paso 13: Juan actualiza su progreso

```http
PUT /api/desarrollo/progreso/1
Content-Type: application/json

{
  "porcentajeCompletacion": 100,
  "completado": true
}
```

**Respuesta:** `204 No Content`

---

### ?? Paso 14: Sistema marca curso como completado y otorga puntos

```http
POST /api/gamificacion/marcar-curso-completado
?colaboradorId=1&cursoId=1
```

**Respuesta:**
```json
{
  "message": "Curso marcado como completado y puntos otorgados",
  "puntosOtorgados": 75
}
```

? **Juan gana 75 puntos adicionales (total: 175)**

---

### ?? Paso 15: Juan registra una certificación

```http
POST /api/rrhh/certificaciones?colaboradorId=1
Content-Type: application/json

{
  "nombreCertificacion": "Certified Kubernetes Administrator",
  "fechaObtencion": "2024-01-17",
  "institucion": "CNCF"
}
```

**Respuesta:**
```json
{
  "certificacionID": 1,
  "colaboradorID": 1,
  "nombreCertificacion": "Certified Kubernetes Administrator",
  "fechaObtencion": "2024-01-17",
  "institucion": "CNCF"
}
```

---

### ?? Paso 16: RR.HH. valida la certificación

```http
POST /api/gamificacion/validar-certificacion
?certificacionId=1&colaboradorId=1
```

**Respuesta:**
```json
{
  "message": "Certificación validada y puntos otorgados",
  "puntosOtorgados": 50
}
```

? **Juan gana 50 puntos adicionales (total: 225)**

---

### ?? Paso 17: Juan actualiza sus habilidades

RR.HH. actualiza:
```http
PUT /api/rrhh/habilidades/actualizar?colaboradorId=1&skillId=3
Content-Type: application/json

{
  "nivelDominio": "Avanzado"
}
```

**Respuesta:** `204 No Content`

---

### ?? Paso 18: RR.HH. verifica el match actualizado

```http
GET /api/matching/candidates/5
```

**Respuesta (Actualizado):**
```json
[
  {
    "colaboradorID": 1,
    "nombreCompleto": "Juan García",
    "porcentajeMatch": 100.0,
    "skillsFaltantes": []
  }
]
```

¡Match perfecto! ?

---

### ?? Paso 19: RR.HH. cambia estado a "Seleccionado"

```http
PUT /api/pipeline/1/estado
Content-Type: application/json

{
  "estado": "Seleccionado",
  "interes": "Alto",
  "notas": "Candidato perfecto, iniciar trámites de oferta"
}
```

**Respuesta:** `204 No Content`

---

### ?? Paso 20: Juan recibe notificación de selección

```json
{
  "notificacionID": 3,
  "colaboradorID": 1,
  "titulo": "Actualización de Candidatura",
  "mensaje": "¡Felicitaciones! Has sido seleccionado para esta vacante",
  "tipo": "Vacante",
  "leida": false,
  "fechaCreacion": "2024-01-17T15:00:00Z"
}
```

---

### ?? Paso 21: Juan visualiza su historial de puntos

```http
GET /api/gamificacion/historial/1
```

**Respuesta:**
```json
{
  "totalPuntos": 225,
  "historial": [
    {
      "fecha": "2024-01-15T10:30:00Z",
      "cantidad": 100,
      "razon": "Perfil Completado"
    },
    {
      "fecha": "2024-01-17T10:00:00Z",
      "cantidad": 75,
      "razon": "Curso Completado"
    },
    {
      "fecha": "2024-01-17T11:00:00Z",
      "cantidad": 50,
      "razon": "Certificación validada: Certified Kubernetes Administrator"
    }
  ]
}
```

---

### ?? Paso 22: Juan ve el leaderboard

```http
GET /api/gamificacion/leaderboard?top=5
```

**Respuesta:**
```json
[
  {
    "colaboradorID": 1,
    "nombreCompleto": "Juan García",
    "totalPuntos": 225
  },
  {
    "colaboradorID": 3,
    "nombreCompleto": "María López",
    "totalPuntos": 180
  }
]
```

Juan está en el top! ??

---

### ?? Paso 23: RR.HH. genera reportes

**Dashboard:**
```http
GET /api/reporte/dashboard
```

**Respuesta:**
```json
{
  "totalColaboradores": 50,
  "totalSkills": 25,
  "totalVacantes": 8,
  "vacantesActivas": 3,
  "totalAplicaciones": 15,
  "perfilesCompletos": 42,
  "fechaReporte": "2024-01-17T16:00:00Z"
}
```

**Inventario de Skills:**
```http
GET /api/reporte/inventario-skills/por-nivel
```

**Respuesta:**
```json
[
  {
    "nivel": "Experto",
    "totalColaboradores": 5,
    "skills": [
      {"skill": "Docker", "cantidad": 4},
      {"skill": "Kubernetes", "cantidad": 3}
    ]
  },
  {
    "nivel": "Avanzado",
    "totalColaboradores": 15,
    "skills": [...]
  }
]
```

---

## ?? Resumen del Flujo

| Paso | Acción | Resultado |
|------|--------|-----------|
| 1-2 | Juan crea perfil | +100 puntos |
| 3 | Ve sus puntos | Total: 100 |
| 4-6 | RR.HH publica vacante, Juan ve oferta | - |
| 7 | Juan aplica | Estado: Aplicado |
| 8-9 | RR.HH. evalúa a Juan | Estado: En_Evaluacion |
| 10 | Juan recibe notificación | Notificación enviada |
| 11-14 | Juan hace curso | +75 puntos (Total: 175) |
| 15-16 | Juan se certifica | +50 puntos (Total: 225) |
| 17-18 | RR.HH actualiza skills | Match: 100% |
| 19-20 | RR.HH. selecciona a Juan | Notificación: Seleccionado |
| 21 | Juan ve su progreso | 225 puntos en top 2 |
| 22-23 | RR.HH. ve reportes | Dashboard completo |

---

## ?? Diagrama de Estados

```
???????????????
? Aplicado    ?
???????????????
       ?
       ?
???????????????????????
? En_Evaluacion       ?
? (Entrevista Técnica)?
???????????????????????
       ?      ?
       ?      ?
       ?  ????????????????
       ?  ? Rechazado    ?
       ?  ????????????????
       ?
       ?
???????????????????
? Seleccionado    ? ??
???????????????????
```

---

## ?? Ganancia de Puntos en el Proceso

```
Inicio:        0 pts
    ?
Perfil:       +100 = 100 pts ?
    ?
Curso:        +75  = 175 pts ?
    ?
Certificación: +50 = 225 pts ?
    ?
Total:         225 pts ??
```

---

## ?? Insights del Flujo

1. ? **Sistema gamificado** - Puntos motivan actividad
2. ? **Automático** - Sistema notifica cambios
3. ? **Integral** - Perfil ? Aplicación ? Desarrollo
4. ? **Datos** - Reportes ayudan decisiones
5. ? **Eficiente** - Matching reduce tiempo de búsqueda

---

**¡Este es el flujo completo del sistema en acción!** ??
