# ?? GUÍA DE INTEGRACIÓN FRONTEND (Quasar) + BACKEND (.NET)

## ?? Resumen Rápido

Tu backend (.NET 9) expone una API REST en:
- **URL**: `https://localhost:5001` (desarrollo)
- **CORS**: Habilitado para `http://localhost:8080` (Quasar dev)
- **Auth**: JWT Bearer token
- **Swagger**: `https://localhost:5001/swagger`

---

## ?? Pasos de Integración

### 1?? Configurar appsettings.json (Backend)

En `Proyecto3/appsettings.json`:

```json
{
  "Frontend": {
    "Url": "http://localhost:8080"
  },
  "JwtSettings": {
    "SecurityKey": "EstaEsMiClaveSecretaMuyLargaYSeguraParaProyecto3TCSESAN",
    "Issuer": "Proyecto3-API",
    "Audience": "FrontEnd-Cliente"
  }
}
```

? Ya está hecho.

---

### 2?? Actualizar boot/axios.js (Quasar Frontend)

En tu proyecto Quasar, en `src/boot/axios.js`:

```javascript
import { defineBoot } from '#q-app/wrappers'
import axios from 'axios'

// Leer URL de API desde .env
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Interceptor: Agregar token a cada request
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

// Interceptor: Manejo de errores 401
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expirado o inválido
      localStorage.removeItem('auth_token')
      window.location.href = '/login' // Redirigir a login
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

---

### 3?? Crear archivo .env.local (Quasar)

En la raíz del proyecto Quasar, crea `.env.local`:

**Desarrollo (local):**
```
VITE_API_URL=https://localhost:5001
```

**Producción (servidor):**
```
VITE_API_URL=https://api.tudominio.com
```

---

### 4?? Crear Servicio de Autenticación (Quasar)

Crea `src/services/authService.js`:

```javascript
import { api } from 'boot/axios'

export const authService = {
  // Login
  async login(username, password) {
    try {
      const response = await api.post('/auth/login', {
        username,
        password
      })
      
      // Guardar token en localStorage
      localStorage.setItem('auth_token', response.data.token)
      
      return response.data
    } catch (error) {
      console.error('Error de login:', error)
      throw error
    }
  },

  // Logout
  logout() {
    localStorage.removeItem('auth_token')
  },

  // Verificar si está autenticado
  isAuthenticated() {
    return !!localStorage.getItem('auth_token')
  },

  // Obtener token
  getToken() {
    return localStorage.getItem('auth_token')
  }
}
```

---

### 5?? Crear Servicio para Colaboradores (Quasar)

Crea `src/services/colaboradorService.js`:

```javascript
import { api } from 'boot/axios'

export const colaboradorService = {
  // Obtener todos
  async getColaboradores() {
    const response = await api.get('/colaborador')
    return response.data
  },

  // Obtener por ID
  async getColaborador(id) {
    const response = await api.get(`/colaborador/${id}`)
    return response.data
  },

  // Crear
  async createColaborador(data) {
    const response = await api.post('/colaborador', data)
    return response.data
  },

  // Actualizar
  async updateColaborador(id, data) {
    const response = await api.put(`/colaborador/${id}`, data)
    return response.data
  }
}
```

---

### 6?? Ejemplo de Componente Vue (Quasar)

Crea `src/pages/LoginPage.vue`:

```vue
<template>
  <q-page class="row items-center justify-center">
    <div class="col-md-5 col-sm-10 col-xs-12">
      <q-card>
        <q-card-section class="bg-primary text-white">
          <div class="text-h6">Login</div>
        </q-card-section>

        <q-card-section>
          <q-form @submit="handleLogin">
            <q-input
              v-model="username"
              label="Usuario"
              type="text"
              outlined
              dense
              class="q-mb-md"
              rules="[val => val && val.length > 0 || 'Requerido']"
            />

            <q-input
              v-model="password"
              label="Contraseña"
              type="password"
              outlined
              dense
              class="q-mb-md"
              rules="[val => val && val.length > 0 || 'Requerido']"
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

          <div v-if="error" class="text-negative q-mt-md">
            ? {{ error }}
          </div>
        </q-card-section>
      </q-card>
    </div>
  </q-page>
</template>

<script>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { authService } from 'src/services/authService'

export default {
  name: 'LoginPage',
  setup() {
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

    return {
      username,
      password,
      loading,
      error,
      handleLogin
    }
  }
}
</script>
```

---

### 7?? Ejemplo de Página de Colaboradores (Quasar)

Crea `src/pages/ColaboradoresPage.vue`:

```vue
<template>
  <q-page class="q-pa-md">
    <div class="row items-center q-mb-md">
      <h5>Colaboradores</h5>
      <q-space />
      <q-btn
        color="primary"
        label="Nuevo"
        @click="openDialog = true"
      />
    </div>

    <q-table
      :rows="colaboradores"
      :columns="columns"
      row-key="colaboradorID"
      flat
      bordered
      :loading="loading"
    />
  </q-page>
</template>

<script>
import { ref, onMounted } from 'vue'
import { colaboradorService } from 'src/services/colaboradorService'
import { useQuasar } from 'quasar'

export default {
  name: 'ColaboradoresPage',
  setup() {
    const $q = useQuasar()
    const colaboradores = ref([])
    const loading = ref(false)
    const openDialog = ref(false)

    const columns = [
      { name: 'id', label: 'ID', field: 'colaboradorID' },
      { name: 'nombre', label: 'Nombre', field: 'nombreCompleto' },
      { name: 'rol', label: 'Rol', field: 'rolActual' },
      { name: 'cuenta', label: 'Cuenta', field: 'cuentaProyecto' }
    ]

    const cargarColaboradores = async () => {
      loading.value = true
      try {
        colaboradores.value = await colaboradorService.getColaboradores()
      } catch (error) {
        $q.notify({
          type: 'negative',
          message: 'Error al cargar colaboradores'
        })
      } finally {
        loading.value = false
      }
    }

    onMounted(() => {
      cargarColaboradores()
    })

    return {
      colaboradores,
      columns,
      loading,
      openDialog,
      cargarColaboradores
    }
  }
}
</script>
```

---

### 8?? Configurar Router (Quasar)

En `src/router/routes.js`:

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
    children: [
      { path: '', component: () => import('pages/DashboardPage.vue') },
      {
        path: 'colaboradores',
        component: () => import('pages/ColaboradoresPage.vue')
      },
      {
        path: 'vacantes',
        component: () => import('pages/VacantesPage.vue')
      },
      {
        path: 'perfil',
        component: () => import('pages/PerfilPage.vue')
      }
    ],
    beforeEnter: (to, from, next) => {
      // Proteger rutas: solo si está autenticado
      if (authService.isAuthenticated()) {
        next()
      } else {
        next('/login')
      }
    }
  }
]

export default routes
```

---

## ?? Ejecutar en Desarrollo

### Terminal 1: Backend (.NET)

```bash
cd "C:\Ruta\A\Proyecto3"
dotnet run
```

Verifica que compile sin errores y escuche en `https://localhost:5001`

### Terminal 2: Frontend (Quasar)

```bash
cd "C:\Ruta\A\Frontend"
quasar dev
```

Debe levantarse en `http://localhost:8080`

---

## ?? Probar Conexión

1. **Backend**: Abre `https://localhost:5001/swagger`
   - Verifica que todos los endpoints estén listados
   - Prueba POST `/api/auth/login` con:
     ```json
     {
       "username": "admin",
       "password": "admin123"
     }
     ```
   - Debería devolver un `token`

2. **Frontend**: Abre `http://localhost:8080/login`
   - Ingresa: usuario `admin` / contraseña `admin123`
   - Debe redirigir a dashboard si es exitoso
   - Abre DevTools (F12) ? Network/Storage
   - Verifica que `auth_token` esté en localStorage

---

## ?? CORS: Desarrollo vs Producción

### Desarrollo

```csharp
// appsettings.json
"Frontend": {
  "Url": "http://localhost:8080"
}
```

### Producción

```csharp
// appsettings.Production.json
"Frontend": {
  "Url": "https://tudominio.com"
}
```

---

## ?? Endpoints Disponibles

| Método | Ruta | Descripción | Auth |
|--------|------|-------------|------|
| POST | `/api/auth/login` | Login | ? No |
| GET | `/api/colaborador` | Listar colaboradores | ? Sí |
| GET | `/api/perfil/{id}` | Obtener perfil | ? Sí |
| POST | `/api/vacante/{id}/aplicar` | Aplicar a vacante | ? Sí |
| GET | `/api/gamificacion/puntos/{id}` | Ver puntos | ? Sí |
| GET | `/api/reporte/dashboard` | Dashboard | ? Sí |

[Ver todos en POSTMAN_REQUESTS.md]

---

## ?? Troubleshooting

### Error: CORS policy: No 'Access-Control-Allow-Origin'

**Causa**: `appsettings.json` no coincide con origen del frontend

**Solución**:
```json
"Frontend": {
  "Url": "http://localhost:8080"  // ? Debe coincidir exactamente
}
```

### Error: 401 Unauthorized

**Causa**: Token no se está enviando o ha expirado

**Solución**: Verificar en DevTools ? Network ? Headers ? `Authorization: Bearer token`

### Error: SSL certificate problem

**Causa**: Certificado autofirmado del backend HTTPS

**Solución** (desarrollo):
```javascript
// En axios.js
const api = axios.create({
  baseURL: API_BASE_URL,
  httpsAgent: new https.Agent({ rejectUnauthorized: false }) // ?? Solo desarrollo
})
```

---

## ?? Próximos Pasos

1. ? Crear AuthController con Login
2. ? Habilitar CORS en Program.cs
3. ? Configurar axios con interceptores
4. ?? Crear servicios para cada módulo (Vacantes, Perfil, Gamificación, etc.)
5. ?? Crear páginas Vue/Quasar
6. ?? Testear flujos e2e

---

**Listo para conectar Quasar con tu backend .NET** ??
