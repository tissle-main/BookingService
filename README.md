# BookingService

BookingService is a room-booking application built with .NET 10, ASP.NET Core Blazor Web App, Entity Framework Core (SQL Server), ASP.NET Core Identity, and .NET Aspire.
Authenticated users can browse rooms, review schedules, book rooms, and manage room-related data through the Blazor UI.

## Solution structure

The solution is organized into focused projects:

- **`BookingService.Web`** — ASP.NET Core host, interactive Blazor components, feature endpoints, mediator handlers, validation, authorization, SignalR notifications, and database seeding.
- **`BookingService.Data`** — `AppDbContext`, Identity entities, room and booking entities, EF Core configurations, and migrations.
- **`BookingService.AppHost`** — .NET Aspire app host that orchestrates the web application and SQL Server resources for local development and integration tests.
- **`BookingService.ServiceDefaults`** — shared Aspire defaults, including OpenTelemetry, health checks, service discovery, and resilient HTTP client configuration.
- **`BookingService.UnitTests`** — isolated TUnit tests, primarily for validators.
- **`BookingService.IntegrationTests`** — Aspire-based integration tests that run against the application and SQL Server resource.

## API endpoints

Almost all API endpoints are not public, while Blazor UI uses `IMediator` instead, 
because it runs in the server itself due to globally enabled `InteractiveServer` render mode.
Endpoints only available for tests, except a login/logout ones 
(`InteractiveServer` render mode does not support changes of http headers that are required to set auth cookies).

The feature endpoints include:

- `POST /api/auth/register` - add new user
- `POST /api/auth/login` - login user (public)
- `POST /api/auth/logout` - logout user (public)
- `GET /api/user` [Authorize] - get current authenticated user
- `GET /api/users` [Authorize(Roles = "Admin")] - get all or concrete users
- `GET /api/rooms` [Authorize] - get all or concrete rooms
- `POST /api/room` [Authorize(Roles = "Admin")] - add new room
- `PUT /api/room` [Authorize(Roles = "Admin")] - update room
- `DELETE /api/rooms` [Authorize(Roles = "Admin")] - delete all or concrete rooms
- `GET /api/bookings` [Authorize] - get all or concrete bookings
- `POST /api/booking` [Authorize] - book a room

There are some extra helper endpoints provided for integration tests.

## Request pipeline

For an HTTP API request, the effective flow is:

```text
HTTP request
	-> exception handling and HTTPS redirection
	-> antiforgery and static asset middleware where applicable
	-> authentication
	-> authorization
	-> endpoint
	-> IMediator.Send(...)
	-> ValidationBehavior
	-> AuthorizedBehavior (for authorized messages)
	-> DbTransactionBehavior (for transactional messages)
	-> command/query handler
	-> EF Core / SQL Server
	-> ErrorOr<T> converted to an HTTP result
	-> HTTP response
```

The validation, authorization, and transaction behaviors are applied in that order when a message requires them.
Endpoint authorization is configured with `RequireAuthorization`, while mediator authorization verifies the current Identity user and allowed roles.
Commands marked for transaction handling execute inside an EF Core execution-strategy transaction.

For a Blazor interaction, the component runs inside an interactive server circuit and sends the command or query directly through the scoped mediator.
The handler follows the same mediator behavior pipeline and persistence path, but no separate browser HTTP request is required.
After a booking is committed, the application publishes a SignalR notification to connected clients so affected schedules can be reloaded.

## Running locally

.NET Aspire allows solution to be run locally. It requires docker or podman running.
It also requires having next `UserSecrets` for `BookingService.Web`:
```json
{
  "AdminCredentials": {
    "Email": "<some email>",
    "Password": "<some password>"
  }
}
```
SQL Server database gonna be run as container. SignalR works inside ASP.NET Core of `BookingService.Web` project.
Run AppHost project, wait untill all services state becomes "Running" and than click on the link of `booking-web` resource.

## Running on Azure

.NET Aspire allows solution to be deployed on Azure. It requires docker or podman running. It also requires 'Microsoft.Azd' and 'Aspire.Cli' to be installed.
Aspire creates all necessary resources and deploys everything.
SQL Server gonna be as a `SQL Server` + `SQL Database` resource and SignalR as separated `SignalR` resource.
`BookingService.Web` gonna be `AppService` website.

1. Open powershell with `src` folder selected of the solution
2. Authorize with command `az login`
3. Run `aspire deploy`
4. Open Azure
5. Open `AppService` => Settings => Environment Variables
6. Add "AdminCredentials__Email" and "AdminCredentials__Password" variables (two underscores in the name)
7. Open link of `AppService`