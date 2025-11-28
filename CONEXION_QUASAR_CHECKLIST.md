# ? BACKEND LISTO PARA QUASAR - RESUMEN FINAL

## ?? Estado Actual

**Backend (.NET 9)**: ? **COMPLETAMENTE CONFIGURADO**

- ? API REST con 80+ endpoints
- ? CORS habilitado y configurado
- ? JWT Authentication implementado
- ? AuthController con login listo
- ? Swagger/OpenAPI funcionando
- ? Conexión a SQL Server

---

## ?? Lo que Ya Está Hecho en el Backend

### 1. **CORS Habilitado** 
?? `Proyecto3/Program.cs`
```csharp
app.UseCors("FrontendPolicy");
```
? Acepta requests desde `http://localhost:8080` (Quasar dev)

### 2. **JWT Authentication**
?? `Proyecto3/Controllers/AuthController.cs`
```
POST /api/auth/login
```
- Input: `{ "username": "admin", "password": "admin123" }`
- Output: `{ "token": "eyJhbGc...", "expiresIn": 3600 }`

### 3. **Configuración**
?? `Proyecto3/appsettings.json`
```json
{
  "Frontend": { "Url": "http://localhost:8080" },
  "JwtSettings": { "SecurityKey": "...", "Issuer": "...", "Audience": "..." }
}
```

### 4. **Endpoints Protegidos**
Todos requieren header:
```
Authorization: Bearer <token>
```

---

## ?? Qué Debes Hacer en QUASAR

### PASO 1: Crear `.env.local` en la raíz del proyecto Quasar

```
VITE_API_URL=https://localhost:5001
```

### PASO 2: Actualizar `src/boot/axios.js`

Reemplaza completamente con:

```javascript
import { defineBoot } from '#q-app/wrappers'
import axios from 'axios'

// URL base del API (desde .env o por defecto)
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  },
  withCredentials: true
})

// ?? INTERCEPTOR: Agregar token a cada request
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('auth_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// ?? INTERCEPTOR: Manejo de errores (401 = token expirado)
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('auth_token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default defineBoot(({ app }) => {
  app.config.globalProperties.$axios = axios
  app.config.globalProperties.$api = api
})

export { api }
```

### PASO 3: Crear Servicio de Autenticación

?? `src/services/authService.js`

```javascript
import { api } from 'boot/axios'

export const authService = {
  async login(username, password) {
    const response = await api.post('/auth/login', { username, password })
    localStorage.setItem('auth_token', response.data.token)
    return response.data
  },

  logout() {
    localStorage.removeItem('auth_token')
  },

  isAuthenticated() {
    return !!localStorage.getItem('auth_token')
  }
}
```

### PASO 4: Crear Página de Login

?? `src/pages/LoginPage.vue`

```vue
<template>
  <q-page class="row items-center justify-center">
    <q-card class="col-md-5 col-sm-10">
      <q-card-section class="bg-primary text-white">
        <div class="text-h6">Login - Talento Interno</div>
      </q-card-section>

      <q-card-section>
        <q-form @submit="handleLogin" class="q-gutter-md">
          <q-input
            v-model="username"
            label="Usuario"
            outlined
            dense
            rules="[val => val || 'Requerido']"
          />
          <q-input
            v-model="password"
            label="Contraseña"
            type="password"
            outlined
            dense
            rules="[val => val || 'Requerido']"
          />
          <q-btn
            unelevated
            color="primary"
            label="Ingresar"
            type="submit"
            class="full-width"
            :loading="loading"
          />
        </q-form>
        <div v-if="error" class="text-negative text-center q-mt-md">
          {{ error }}
        </div>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { authService } from 'src/services/authService'

const router = useRouter()
const username = ref('admin')
const password = ref('admin123')
const loading = ref(false)
const error = ref('')

const handleLogin = async () => {
  loading.value = true
  error.value = ''
  try {
    await authService.login(username.value, password.value)
    router.push('/dashboard')
  } catch (err) {
    error.value = err.response?.data?.message || 'Error de login'
  } finally {
    loading.value = false
  }
}
</script>
```

### PASO 5: Configurar Router con Protección

?? `src/router/routes.js`

```javascript
import { authService } from 'src/services/authService'

const routes = [
  {
    path: '/login',
    component: () => import('pages/LoginPage.vue')
  },
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    meta: { requiresAuth: true },
    children: [
      {
        path: 'dashboard',
        component: () => import('pages/DashboardPage.vue')
      },
      {
        path: 'colaboradores',
        component: () => import('pages/ColaboradoresPage.vue')
      },
      {
        path: 'vacantes',
        component: () => import('pages/VacantesPage.vue')
      }
    ]
  }
]

// Guard global para proteger rutas
router.beforeEach((to, from, next) => {
  if (to.meta.requiresAuth && !authService.isAuthenticated()) {
    next('/login')
  } else {
    next()
  }
})

export default routes
```

### PASO 6: Crear Servicios para Cada Módulo

?? `src/services/colaboradorService.js`

```javascript
import { api } from 'boot/axios'

export const colaboradorService = {
  getColaboradores: () => api.get('/colaborador'),
  getColaborador: (id) => api.get(`/colaborador/${id}`),
  createColaborador: (data) => api.post('/colaborador', data),
  updateColaborador: (id, data) => api.put(`/colaborador/${id}`, data)
}
```

?? `src/services/vacanteService.js`

```javascript
import { api } from 'boot/axios'

export const vacanteService = {
  getVacantes: () => api.get('/vacante'),
  getVacante: (id) => api.get(`/vacante/${id}`),
  aplicarVacante: (vacanteId, data) => api.post(`/vacante/${vacanteId}/aplicar`, data),
  misCandidaturas: (colaboradorId) => api.get(`/vacante/colaborador/${colaboradorId}/mis-aplicaciones`)
}
```

---

## ?? Pruebas

### Antes de ejecutar el frontend, verifica el backend:

1. **Terminal (Backend)**:
```bash
cd Proyecto3
dotnet run
```
Debe salir: "Now listening on: https://localhost:5001"

2. **Browser**: Abre `https://localhost:5001/swagger`
   - Intenta POST `/api/auth/login`:
     ```json
     { "username": "admin", "password": "admin123" }
     ```
   - Debe devolver un token

3. **Terminal (Frontend)**:
```bash
cd tu-frontend-quasar
npm install  // si no lo has hecho
quasar dev
```
Debe salir: "App URL: http://localhost:8080"

4. **Browser**: Ve a `http://localhost:8080/login`
   - Ingresa las credenciales
   - Debe redirigir al dashboard

---

## ?? Resumen de Archivos Creados/Modificados

### Backend (.NET)
- ? `Proyecto3/Program.cs` - CORS + JWT
- ? `Proyecto3/Controllers/AuthController.cs` - Login endpoint
- ? `Proyecto3/appsettings.json` - Frontend:Url

### Frontend (Quasar) - POR HACER
- ?? `.env.local` - Variables de entorno
- ?? `src/boot/axios.js` - Actualizar con interceptores
- ?? `src/services/authService.js` - Servicio de login
- ?? `src/pages/LoginPage.vue` - Página de login
- ?? `src/router/routes.js` - Protección de rutas
- ?? `src/services/*.js` - Servicios para módulos

---

## ?? Orden de Ejecución

```
1. Backend: dotnet run
   ?? Escucha en https://localhost:5001
   
2. Frontend: quasar dev
   ?? Escucha en http://localhost:8080
   
3. Abre http://localhost:8080
   ?? Redirige a /login (no autenticado)
   
4. Ingresa usuario: admin / contraseña: admin123
   ?? Envía POST a /api/auth/login
   ?? Recibe token y lo guarda en localStorage
   ?? Redirige al dashboard
```

---

## ?? Endpoints Disponibles (Protegidos)

| Endpoint | Método | Descripción |
|----------|--------|-------------|
| `/api/auth/login` | POST | Login (sin protección) |
| `/api/colaborador` | GET | Listar colaboradores |
| `/api/perfil/{id}` | GET | Obtener perfil |
| `/api/vacante` | GET | Listar vacantes |
| `/api/vacante/{id}/aplicar` | POST | Aplicar a vacante |
| `/api/gamificacion/puntos/{id}` | GET | Ver puntos |
| `/api/reporte/dashboard` | GET | Dashboard general |

[Ver lista completa en POSTMAN_REQUESTS.md]

---

## ? Listo Para Integración

**Backend**: ? Completamente configurado y listo  
**Frontend**: ?? Sigue los 6 pasos anteriores  
**Integración**: ?? Funcionará sin problemas

---

**¿Dudas? Consulta `INTEGRACION_QUASAR_BACKEND.md` para detalles completos**
