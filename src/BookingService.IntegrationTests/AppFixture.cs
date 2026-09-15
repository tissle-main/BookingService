using Bogus;
using Respawn;
using Projects;
using TUnit.Aspire;
using BookingService.Web;
using BookingService.Data;
using BookingService.AppHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BookingService.Web.Shared.DbSeeding;
using BookingService.Web.Features.Auth.Options;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Features.Auth.Handlers.LoginUser;
using BookingService.Web.Features.Auth.Handlers.GetAdminCredentials;

namespace BookingService.IntegrationTests;

public sealed class AppFixture : AspireFixture<BookingService_AppHost>
{
    #region Instance
    private string ConnectionString { get; set; } = null!; //Init after InitializedAsync
    private DbContextOptions<AppDbContext> DbOptions { get; set; } = null!; //Init after InitializedAsync
    private Respawner Respawner { get; set; } = null!; //Init after InitializedAsync
    public HttpClient HttpClient { get; private set; } = null!; //Init after InitializedAsync

    public async ValueTask ExecuteDbContextAsync(Func<AppDbContext, ValueTask> func)
    {
        await using AppDbContext context = new(DbOptions);
        await func(context);
    }
    public async ValueTask<T> ExecuteDbContextAsync<T>(Func<AppDbContext, ValueTask<T>> func)
    {
        await using AppDbContext context = new(DbOptions);
        return await func(context);
    }
    public async ValueTask ResetDatabaseAsync(CancellationToken cancellationToken)
    {
        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await Respawner.ResetAsync(connection);
    }
    public async ValueTask SeedDatabaseAsync(CancellationToken cancellationToken)
    {
        using HttpResponseMessage message = await HttpClient.SendDbSeedingAsync(cancellationToken);
        message.EnsureSuccessStatusCode();
    }
    public async ValueTask<UserDto> LoginAsAdminAsync(CancellationToken cancellationToken)
    {
        (HttpResponseMessage message, AdminCredentials? credentials) = await HttpClient.SendGetAdminCredentials2Async(cancellationToken);
        message.EnsureSuccessStatusCode();
        await Assert.That(credentials).IsNotNull();
        message.Dispose();

        LoginUserCommand loginCommand = new Faker<LoginUserCommand>().ValidInstance().WithEmail(credentials.Email).WithPassword(credentials.Password);
        (message, LoginUserResponse? response) = await HttpClient.SendLoginUser2Async(loginCommand, cancellationToken);
        message.EnsureSuccessStatusCode();
        await Assert.That(response).IsNotNull();
        return response.User;
    }
    #endregion

    #region Base
    protected override TimeSpan ResourceTimeout
    {
        get => TimeSpan.FromMinutes(10);
    }
    protected override AspireFixtureOptions Options
    {
        get => field ??= new AspireFixtureOptions()
        {
            ForwardResourceLogs = true
        };
    }

    public override async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("DOTNET_LAUNCH_PROFILE", ProfileNames.Test);
        await base.InitializeAsync();

        HttpClient = base.CreateHttpClient(AppHostResources.Web);
        ConnectionString = await base.GetConnectionStringAsync(AppHostResources.AppDatabase) ?? throw new NullReferenceException("ConnectionString is null");
        DbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(ConnectionString).Options;
        await ExecuteDbContextAsync(async db =>
        {
            await db.Database.MigrateAsync(base.RunCancellationToken);
        });

        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync(base.RunCancellationToken);
        Respawner = await Respawner.CreateAsync(connection, new RespawnerOptions()
        {
            DbAdapter = DbAdapter.SqlServer,
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }
    public override async ValueTask DisposeAsync()
    {
        await ResetDatabaseAsync(base.RunCancellationToken);
        await base.DisposeAsync();
    }
    #endregion
}