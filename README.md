# VaultHistory.Microservice.User

Microservicio desarrollado con .NET para el modulo de usuarios dentro del sistema VaultHistory.

Este repositorio documenta principalmente aspectos tecnicos del proyecto: arquitectura, ejecucion local, Docker, PostgreSQL, Entity Framework Core y flujo de migraciones.

## Stack Tecnico

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Docker / Docker Compose
- xUnit
- Testcontainers
- Swagger / OpenAPI

## Arquitectura

El proyecto esta organizado por capas, separando dominio, casos de uso, infraestructura y API.

```txt
src/
  VaultHistory.User.Api/
  VaultHistory.User.Application/
  VaultHistory.User.Domain/
  VaultHistory.User.Infrastructure/

tests/
  VaultHistory.User.Api.UnitTests/
  VaultHistory.User.Api.IntegrationTests/
  VaultHistory.User.Application.UnitTests/
  VaultHistory.User.Domain.UnitTests/
  VaultHistory.User.Infrastructure.UnitTests/
  VaultHistory.User.Infrastructure.IntegrationTests/
```

### Domain

Contiene el modelo de dominio y las reglas principales del sistema.

Incluye:

- Entities
- Value Objects
- Domain Events
- Interfaces de repositorios
- Abstracciones compartidas del dominio

Esta capa no depende de infraestructura, frameworks externos ni detalles de persistencia.

### Application

Contiene los casos de uso y la logica de aplicacion.

Incluye:

- Commands
- Queries
- Use Cases
- Validators
- Provider abstractions
- Behaviors
- DTOs de aplicacion

Esta capa coordina el flujo de trabajo entre el dominio y las dependencias externas, sin conocer detalles concretos de infraestructura.

### Infrastructure

Contiene implementaciones concretas para persistencia y servicios externos.

Incluye:

- `ApplicationDbContext`
- Configuracion de Entity Framework Core
- Repositorios
- Outbox messages
- Configuracion de PostgreSQL
- Migraciones de base de datos

### Api

Expone los endpoints HTTP del microservicio.

Incluye:

- Controllers
- Middlewares
- Swagger
- Configuracion de autenticacion
- Configuracion del host ASP.NET Core

## Domain-Driven Design

El proyecto aplica conceptos de Domain-Driven Design para mantener el dominio aislado y expresivo.

Conceptos utilizados:

- **Entities**: objetos con identidad propia.
- **Value Objects**: objetos que representan conceptos del dominio.
- **Domain Events**: eventos generados por cambios relevantes dentro del dominio.
- **Repositories**: contratos definidos desde el dominio para acceder a la persistencia.
- **Unit of Work**: abstraccion para confirmar cambios de manera transaccional.
- **Outbox Pattern**: persistencia de eventos de dominio como mensajes pendientes de procesamiento.

### Notificacion de inicio de sesion

Cada inicio de sesion exitoso registra `UserSignedInEvent` y lo persiste en la tabla de outbox con estado `PENDING` en la misma confirmacion de cambios. El mensaje usa el tipo `UserSignedInEvent` y un payload JSON compatible con Jobs:

```json
{
  "userId": "<id-del-usuario>"
}
```

Si no se puede persistir el evento, el inicio de sesion devuelve un error y no emite el JWT. El payload no contiene la contrasena ni el token.

## Configuracion Local

La API carga configuracion desde la carpeta:

```txt
src/VaultHistory.User.Api/Configurations/
```

El orden de carga es:

```txt
appsettings.json
appsettings.{Environment}.json
Environment variables
```

Para ejecucion local, normalmente se usa el ambiente `Local`.

Ejemplo de connection string local:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=app_db;Username=admin;Password=admin123"
  }
}
```

Cuando la API corre dentro de Docker, la connection string debe usar el nombre del servicio de PostgreSQL:

```txt
Host=postgres;Port=5432;Database=app_db;Username=admin;Password=admin123
```

## Ejecutar Con dotnet watch

Desde la raiz del repositorio:

```bash
dotnet watch run --project src/VaultHistory.User.Api
```

Para ejecutar explicitamente usando ambiente `Local`:

```bash
ASPNETCORE_ENVIRONMENT=Local dotnet watch run --project src/VaultHistory.User.Api
```

En PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Local"
dotnet watch run --project src/VaultHistory.User.Api
```

## Ejecutar Con Docker

La configuracion Docker esta ubicada dentro de la carpeta:

```txt
docker/
```

Estructura:

```txt
docker/
  Dockerfile
  docker-compose.yml
  .dockerignore
  database/
    init/
      001-initial-schema.sql
```

Para levantar la API junto con PostgreSQL:

```bash
docker compose -f docker/docker-compose.yml up --build
```

Tambien se puede ejecutar desde la carpeta `docker`:

```bash
cd docker
docker compose up --build
```

La API queda disponible en:

```txt
http://localhost:5000
```

Swagger:

```txt
http://localhost:5000/swagger/index.html
```

Para detener los contenedores:

```bash
docker compose -f docker/docker-compose.yml down
```

Para detener los contenedores y eliminar el volumen de PostgreSQL:

```bash
docker compose -f docker/docker-compose.yml down -v
```

Los scripts ubicados en `docker/database/init` solo se ejecutan cuando PostgreSQL inicializa la base de datos por primera vez. Si el volumen ya existe, los scripts no se vuelven a ejecutar automaticamente.

## Entity Framework Core

El proyecto usa Entity Framework Core con PostgreSQL.

Las migraciones se almacenan en:

```txt
src/VaultHistory.User.Infrastructure/Database/Migrations
```

## Crear Una Nueva Migracion

Cuando se realicen cambios en entidades o configuraciones de EF Core que afecten el esquema de base de datos, se debe crear una nueva migracion:

```bash
dotnet ef migrations add NombreDeLaMigracion \
  --project src/VaultHistory.User.Infrastructure \
  --startup-project src/VaultHistory.User.Api \
  --output-dir Database/Migrations
```

Ejemplo:

```bash
dotnet ef migrations add InitialCreate \
  --project src/VaultHistory.User.Infrastructure \
  --startup-project src/VaultHistory.User.Api \
  --output-dir Database/Migrations
```

## Aplicar Migraciones A La Base Local

Para actualizar una base de datos local existente:

```bash
dotnet ef database update \
  --project src/VaultHistory.User.Infrastructure \
  --startup-project src/VaultHistory.User.Api
```

## Generar SQL Inicial Para Docker

El archivo SQL usado por Docker debe generarse desde las migraciones de EF Core:

```bash
dotnet ef migrations script \
  --project src/VaultHistory.User.Infrastructure \
  --startup-project src/VaultHistory.User.Api \
  --idempotent \
  -o docker/database/init/001-initial-schema.sql
```

Este script permite que PostgreSQL cree el esquema inicial cuando el contenedor se levanta por primera vez.

## Flujo Recomendado Para Cambios De Base De Datos

Cada vez que se modifique el modelo persistente:

```txt
1. Actualizar entidades o configuraciones de EF Core.
2. Crear una nueva migracion.
3. Aplicar la migracion a la base local.
4. Regenerar el SQL inicial para Docker.
5. Probar el proyecto localmente.
```

Comandos:

```bash
dotnet ef migrations add NombreDeLaMigracion \
  --project src/VaultHistory.User.Infrastructure \
  --startup-project src/VaultHistory.User.Api \
  --output-dir Database/Migrations

dotnet ef database update \
  --project src/VaultHistory.User.Infrastructure \
  --startup-project src/VaultHistory.User.Api

dotnet ef migrations script \
  --project src/VaultHistory.User.Infrastructure \
  --startup-project src/VaultHistory.User.Api \
  --idempotent \
  -o docker/database/init/001-initial-schema.sql
```

Si se desea probar Docker desde cero despues de regenerar el SQL:

```bash
docker compose -f docker/docker-compose.yml down -v
docker compose -f docker/docker-compose.yml up --build
```

## Tests

Ejecutar todos los tests:

```bash
dotnet test
```

Ejecutar tests de un proyecto especifico:

```bash
dotnet test tests/VaultHistory.User.Domain.UnitTests
```

Los tests de integracion usan PostgreSQL mediante Testcontainers, por lo que requieren Docker en ejecucion.

## Herramientas Necesarias

- .NET SDK 10
- Docker Desktop
- PostgreSQL, opcional si se usa Docker
- EF Core CLI

Instalar EF Core CLI globalmente:

```bash
dotnet tool install --global dotnet-ef --version 10.0.7
```

Actualizar EF Core CLI:

```bash
dotnet tool update --global dotnet-ef --version 10.0.7
```

Verificar la instalacion:

```bash
dotnet ef --version
```
