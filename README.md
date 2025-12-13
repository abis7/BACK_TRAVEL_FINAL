# TravelFriend API

API REST desarrollada en ASP.NET Core 8.0 para la gestión de viajes compartidos, gastos y liquidaciones entre amigos.

## Descripción del Proyecto

TravelFriend es una aplicación backend que permite a grupos de amigos organizar viajes, registrar gastos compartidos y calcular automáticamente las liquidaciones finales. El sistema implementa autenticación JWT, validación de datos con FluentValidation, y manejo de relaciones mediante Entity Framework Core.

### Funcionalidades Principales

- **Gestión de Usuarios**: Registro, autenticación y administración de perfiles de usuario
- **Sistema de Amistades**: Los usuarios pueden agregar amigos para organizar viajes conjuntos
- **Creación de Viajes**: Organización de viajes con múltiples participantes
- **Registro de Gastos**: Seguimiento detallado de gastos individuales y compartidos
- **Liquidaciones Automáticas**: Cálculo inteligente de deudas entre participantes
- **Notificaciones**: Sistema de notificaciones para mantener informados a los usuarios
- **Balance en Tiempo Real**: Consulta del estado de cuentas durante el viaje

### Endpoints Implementados

#### Módulo de Usuarios
- `POST /api/Usuario/crear` - Registro de nuevos usuarios
- `POST /api/Usuario/login` - Autenticación y generación de token JWT (solo prueba)
- `GET /api/Usuario/perfil` - Obtener perfil del usuario autenticado
- `GET /api/Usuario` - Listar todos los usuarios
- `GET /api/Usuario/{id}` - Obtener usuario por ID
- `DELETE /api/Usuario/{id}` - Eliminar cuenta de usuario

#### Módulo de Amigos
- `POST /api/Amigo/agregar` - Agregar nuevo amigo por email
- `GET /api/Amigo/amigos` - Obtener lista de amigos del usuario
- `GET /api/Amigo/AmigoDeUsuario/{usuarioId}` - Obtener amigos de un usuario específico

#### Módulo de Viajes
- `POST /api/Viaje/crear` - Crear nuevo viaje con participantes
- `GET /api/Viaje/{viajeId}` - Obtener detalles de un viaje
- `GET /api/Viaje/activos` - Listar viajes activos del usuario
- `POST /api/Viaje/{viajeId}/finalizar` - Finalizar viaje y generar liquidaciones

#### Módulo de Gastos
- `GET /api/Gasto/viaje/{viajeId}` - Listar gastos de un viaje
- `POST /api/Gasto/crear` - Registrar nuevo gasto
- `GET /api/Gasto/resumen/{viajeId}` - Obtener resumen de gastos del viaje

#### Módulo de Liquidaciones
- `GET /api/Liquidacion/{viajeId}` - Obtener liquidaciones finales
- `GET /api/Liquidacion/{viajeId}/balance` - Consultar balance en tiempo real
- `POST /api/Liquidacion/{id}/pagar` - Marcar deuda como pagada

#### Módulo de Notificaciones
- `GET /api/Notificaciones` - Obtener notificaciones del usuario
- `POST /api/Notificaciones/{id}/leer` - Marcar notificación como leída

## Tecnologías Utilizadas

- **.NET 8.0** - Framework principal
- **Entity Framework Core 8.0.6** - ORM para acceso a datos
- **SQL Server** - Base de datos relacional
- **JWT (JSON Web Tokens)** - Autenticación y autorización
- **BCrypt.Net** - Hashing seguro de contraseñas
- **FluentValidation** - Validación de modelos
- **Swagger/OpenAPI** - Documentación de la API

## Requisitos del Sistema

- **.NET SDK 8.0** o superior
- **SQL Server 2019** o superior (también compatible con SQL Server Express/LocalDB)
- **Postman** (para pruebas de API)

## Configuración Inicial

### 1. Clonar el Repositorio

```bash
git clone https://github.com/abis7/BACK_TRAVEL_FINAL.git
cd TravelFriend
```

### 2. Configurar la Base de Datos

Editar el archivo `appsettings.json` con la cadena de conexión de tu SQL Server:

```json
{
  "ConnectionStrings": {
    "ConexionSQL": "Server=localhost,1433;Database=MiApiDB;User Id=sa;Password=TuPassword;TrustServerCertificate=True;"
  }
}
```

**Nota**: Ajusta los valores según tu configuración:
- `Server`: Dirección y puerto de tu SQL Server
- `Database`: Nombre de la base de datos
- `User Id` y `Password`: Credenciales de acceso


### 3. Instalar Dependencias

```bash
dotnet restore
```

### 4. Aplicar Migraciones

```bash
dotnet ef database update
```

Si las migraciones no existen o necesitas crear una nueva:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Ejecutar el Proyecto

```bash
dotnet run
```


## Autenticación

La API utiliza JWT Bearer Tokens para la autenticación. Para acceder a endpoints protegidos:

### 1. Registrar un Usuario

```http
POST /api/Usuario/crear
Content-Type: application/json

{
  "usuarioNombre": "john_doe",
  "nombre": "John",
  "apellido": "Doe",
  "email": "john@example.com",
  "password": "Password123!"
}
```

### 2. Iniciar Sesión

```http
POST /api/Usuario/login
Content-Type: application/json

{
  "usuario": "john_doe",
  "password": "Password123!"
}
```

**Respuesta**:
```json
{
  "mensaje": "Login exitoso",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 3. Usar el Token

Incluir el token en el header `Authorization` de las peticiones:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```


## Colección de Postman

La colección de Postman:
- **Enlace**: https://web.postman.co/workspace/My-Workspace~29909492-0282-48cf-8968-34860086357a/collection/37264634-46d7135e-e85c-4eed-963b-72fda93abe9a?action=share&source=copy-link&creator=37264634

## VIDEO

- **Enlace**: https://youtu.be/_f3s5xFffdM





**Solución**:
```bash
dotnet ef database drop --force
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

