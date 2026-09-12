# BookingService

## General rules

- Inspect the existing implementation before making changes.
- Follow established project patterns instead of introducing alternative abstractions.
- Keep changes minimal, focused, and directly related to the requested task.
- Do not change public APIs, routes, database contracts, or architecture unless the task requires it.
- Do not introduce a new validation, mediator, logging, feature-discovery, or dependency-injection mechanism when an existing one already serves the purpose.
- Preserve existing behavior unless the task explicitly asks to change it.
- Do not modify generated files manually.
- Do not add documentation files or update unrelated documentation unless explicitly required.
- Keep nullable reference types enabled and fix warnings rather than suppressing them.
- Use `CancellationToken` for asynchronous operations when the surrounding API supports it.
- Before declaring a change complete, build the solution and run the relevant tests.

## Repository overview

BookingService is a .NET 10 solution built around ASP.NET Core Blazor Web App, .NET Aspire, Entity Framework Core, and ASP.NET Core Identity. SQL Server is used for persistence and TUnit is used for tests.

The solution file is `src/BookingService.slnx`. Project paths are relative to `src/`:

- `BookingService.Web` — ASP.NET Core host, Blazor components, HTTP endpoints, application features, mediator handlers, validation, authorization, and database seeding.
- `BookingService.Data` — `AppDbContext`, Identity entities, EF Core configurations, and migrations.
- `BookingService.AppHost` — Aspire distributed application host and database/application resource orchestration.
- `BookingService.ServiceDefaults` — shared Aspire defaults: OpenTelemetry, health checks, service discovery, and resilient HTTP client defaults.
- `BookingService.UnitTests` — TUnit tests for validators and other isolated behavior.
- `BookingService.IntegrationTests` — TUnit.Aspire integration tests that start the AppHost and use the SQL Server resource.

All projects target `net10.0`, use nullable reference types, and enable implicit usings.

## Development commands

Run commands from the repository root.

```powershell
# Restore and build
dotnet restore src/BookingService.slnx
dotnet build src/BookingService.slnx

# Run all tests
dotnet test src/BookingService.slnx

# Run the application through Aspire
dotnet run --project src/BookingService.AppHost/BookingService.AppHost.csproj

# Run the web project directly
dotnet run --project src/BookingService.Web/BookingService.Web.csproj

# Run the web project with the Test launch profile
dotnet run --project src/BookingService.Web/BookingService.Web.csproj --launch-profile Test
```

The Aspire AppHost is the normal local-development entry point. It runs SQL Server in a container for the local environment, creates the application database, waits for the database, and exposes the web application through the Aspire dashboard.

Aspire and integration-test workflows require a working .NET 10 SDK and container-capable local tooling.

## Application architecture

### Startup and composition

`src/BookingService.Web/Program.cs` is intentionally small:

1. `AddServiceDefaults()` configures Aspire defaults.
2. `AddCore()` registers the database, Blazor, mediator pipeline, validators, and feature providers.
3. `UseCore()` migrates/seeds the database, configures Blazor middleware, and activates feature providers.

`src/BookingService.Web/DIContainer.cs` owns this composition.

Preserve middleware and service ordering when changing startup behavior:

- Database migration and seeding occur during `UseCore()`.
- `UseBlazor()` configures exception handling, HTTPS redirection, antiforgery, static assets, and Razor components.
- `UseFeatures()` invokes discovered `FeatureProvider` implementations.
- Authentication and authorization are registered by `AuthFeatureProvider` and enabled before authentication endpoints are mapped.

Feature providers are discovered through reflection from the web assembly. A feature normally derives from `BookingService.Web.Features.FeatureProvider` and overrides `AddServices(WebApplicationBuilder)` and/or `UseMiddleware(WebApplication)`.

### Features and mediator

Application behavior is organized by feature under `src/BookingService.Web/Features`.

Follow the existing feature structure:

```text
Features/<Feature>/
  <Feature>FeatureProvider.cs
  Handlers/<Action>/
    <Action>Command.cs
    <Action>Handler.cs
    <Action>Endpoint.cs
    <Action>CommandValidator.cs   # when validation is required
```

Use Mediator commands and handlers for application actions.

`DIContainer.AddCQRS()` registers the mediator pipeline in this order:

1. `ValidationBehavior<,>`
2. `AuthorizedBehavior<,>`
3. `DbTransactionBehavior<,>`

Commands requiring transaction handling implement `IDbTransactionBehaviorMessage`.

Commands requiring an authenticated user or role implement `IAuthorizedBehaviorMessage`.

Return `ErrorOr<T>` for expected application errors rather than throwing exceptions. Convert endpoint results using the existing `ToHttpResult()` extension.

Minimal API endpoint definitions and client helpers commonly live together in `*Endpoint.cs`. Keep route constants and endpoint registration close to the handler contract. Follow existing explicit return types and `RouteHandlerBuilder` usage when chaining endpoint configuration.

### Blazor

The web project is a Blazor Web App using global interactive server rendering.

- `Components/App.razor` renders `HeadOutlet` and `Routes` with `InteractiveServer`.
- `Components/Routes.razor` wraps routing in `CascadingAuthenticationState` and uses `AuthorizeRouteView`.
- `Components/Layout/MainLayout.razor` is the default layout.
- `Components/_Imports.razor` contains shared component imports.
- Tailwind classes are used in components.
- The checked-in stylesheet is `src/BookingService.Web/wwwroot/tailwind.css`.

Use Blazor components for UI behavior.

Use the existing authentication endpoints for browser login/logout so Identity cookies are issued to the browser.

Use `IMediator` for other application actions invoked by components.

Browser-only operations, such as cookie-setting login/logout, must be performed through browser requests rather than attempting to perform them through server-side mediator calls.

### Persistence and Identity

`BookingService.Data.AppDbContext` derives from:

```text
IdentityDbContext<UserEntity, RoleEntity, Guid>
```

It applies configurations from the data assembly and generates IDs for keyed entities before saving.

Application startup automatically runs EF Core migrations and database seeding.

`AuthDbSeeder` creates roles and the configured administrator user. Admin credentials are read from the `AdminCredentials` configuration section and must be supplied through user secrets or environment configuration rather than committed files.

Migrations are stored under:

```text
src/BookingService.Data/Migrations
```

When changing the EF Core model:

1. Inspect the existing model and migration history.
2. Make the model/configuration changes.
3. Generate the migration using EF Core tooling.
4. Do not manually edit generated migration/designer files unless explicitly required.
5. Verify the migration against a disposable or local database.

### Authentication and authorization

ASP.NET Core Identity uses cookie authentication.

Current paths:

- Login: `/Auth/Login`
- Access denied: `/Auth/Login`
- Authentication API routes: `/api/auth/*`

Existing authentication endpoints:

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/logout`
- `GET /api/auth/check?email=...`
- `DELETE /api/auth/delete`

Keep component route authorization and mediator authorization aligned.

Endpoint authorization is explicit through `RequireAuthorization`.

Mediator authorization is enforced by `AuthorizedBehavior` using the current Identity user and required roles.

## Testing

### Unit tests

Unit tests use TUnit and are located under:

```text
src/BookingService.UnitTests
```

They currently focus on FluentValidation validators and other isolated behavior.

Follow existing test conventions:

- Use `[Test]`.
- Prefer asynchronous `ValueTask` test methods where appropriate.
- Use arrange/act/assert comments when they match existing tests.
- Use Bogus fakers for valid and invalid command instances where appropriate.

### Integration tests

Integration tests use TUnit.Aspire and `AppFixture : AspireFixture<BookingService_AppHost>`.

The fixture:

- Sets `DOTNET_LAUNCH_PROFILE=Test`.
- Starts the Aspire AppHost.
- Creates an HTTP client for the web resource.
- Applies EF Core migrations.
- Uses Respawn to reset the SQL Server database between tests.

Prefer the existing fixture and endpoint extension helpers rather than creating independent host or database setup.

Integration tests require Aspire prerequisites and a container-capable environment.

After code changes, run at least:

```powershell
dotnet build src/BookingService.slnx
dotnet test src/BookingService.slnx
```

If integration tests cannot run because containers or another external resource are unavailable:

1. Still build the solution.
2. Run unit tests.
3. Report the environmental limitation clearly.
4. Do not claim that integration tests passed.

## Configuration and secrets

- `src/BookingService.Web/appsettings.json` contains non-secret logging/host defaults.
- Development configuration is in `appsettings.Development.json`.
- Use the project user-secrets store for local administrator credentials and connection-related secrets.
- Never commit passwords, credential-bearing connection strings, tokens, certificates, or generated local Aspire state.
- Local development uses the SQL Server resource provided by the Aspire AppHost.
- Do not assume or hard-code production infrastructure when implementing application functionality; follow the existing deployment configuration.

## C# and file conventions

Match the surrounding code rather than introducing a different style.

- Use file-scoped namespaces.
- Prefer explicit types for local variables and parameters, following the existing codebase's `var`-free style.
- Use primary constructors and collection expressions when they match surrounding code.
- Use expression-bodied members only when they fit the style of the existing file.
- Keep feature-specific code inside its feature directory.
- Put cross-cutting mediator behavior under `Web/Shared/Behaviors`.
- Use `sealed` for concrete command, handler, DTO, and service types when appropriate.
- Preserve existing naming conventions such as `thisBuilder`, `thisApp`, and `thisUserManager` in extension blocks and primary constructors.
- Preserve existing `#region` organization in files that already use it.
- Do not introduce a second implementation of existing shared infrastructure without first checking how the current infrastructure should be extended.

## Dependencies

Before adding a NuGet package:

1. Check whether .NET, ASP.NET Core, or an existing project dependency already provides the required functionality.
2. Check how the same problem is solved elsewhere in the repository.
3. Add a dependency only when it is justified by the task.
4. Do not upgrade unrelated package versions as part of another change.
5. Keep package versions consistent with the existing solution.

## Git safety

- Do not commit changes unless explicitly requested.
- Do not reset, revert, checkout, or discard user changes.
- Do not overwrite unrelated modifications already present in the working tree.
- Keep changes focused on the requested task.
- Before making destructive changes, inspect the current repository state and verify that the affected files belong to the requested task.

## Change workflow

### Before editing

1. Locate the owning project and feature directory.
2. Read the relevant command, handler, endpoint, validator, component, and tests together.
3. Check existing implementations of similar functionality before introducing a new pattern.
4. Determine whether the change affects:
   - EF Core models or migrations
   - authentication/authorization
   - Aspire resources
   - mediator pipeline behavior
   - public routes or API contracts
   - shared infrastructure
5. Inspect existing tests and determine which ones should be updated or added.

### When adding a web feature

Follow the existing feature-provider/handler/endpoint structure.

- Register feature-specific services through the owning feature provider.
- Use Mediator commands and handlers for application actions.
- Add validation markers and validators when required.
- Add authorization markers when required.
- Add unit and/or integration tests appropriate to the behavior.
- Reuse existing infrastructure rather than creating parallel mechanisms.

### When changing Blazor UI

- Preserve interactive server rendering.
- Follow existing component and Tailwind conventions.
- Keep browser-only operations in browser requests where required.
- Use `IMediator` for server-side application actions.
- Do not introduce client-side state-management infrastructure unless the task requires it.

### After editing

1. Review the diff for unrelated changes.
2. Build the solution.
3. Run relevant unit tests.
4. Run integration tests when the environment supports them.
5. Verify database migrations when the model changed.
6. Report any tests that could not be executed and why.

Avoid broad formatting-only changes and avoid modifying generated files unless the change specifically requires a generated artifact update.

Preserve existing public route contracts unless the task explicitly requires a breaking change.