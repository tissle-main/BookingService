using BookingService.ServiceDefaults;
using Microsoft.AspNetCore.SignalR.Client;

namespace BookingService.Web.Features.SignalR;

/// <summary>Registers and maps the application's SignalR infrastructure.</summary>
public sealed class SignalRFeatureProvider : FeatureProvider
{
    #region Static
    /// <summary>Gets the route used by the SignalR hub.</summary>
    public const string HubPath = "/signalr";

    /// <summary>Gets the absolute hub URL used by the server-side client connection.</summary>
    public static string Url { get; private set; } = "";
    #endregion

    #region Base
    public override void AddServices(WebApplicationBuilder builder)
    {
        if(builder.Environment.IsProduction())
        {
            string hostname = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME") ?? throw new InvalidOperationException("WEBSITE_HOSTNAME not found");
            string baseUrl = $"https://{hostname}";
            Url = $"{baseUrl}{HubPath}";
        }
        else
        {
            string baseUrl = builder.Configuration["ASPNETCORE_URLS"]?.Split(';')[0] ?? throw new NullReferenceException("Server urls not found");
            Url = $"{baseUrl}{HubPath}";
        }

        if(builder.Environment.IsProduction())
        {
            builder.Services.AddSignalR().AddNamedAzureSignalR(AppHostResources.SignalR);
        }
        else
        {
            builder.Services.AddSignalR();
        }
        builder.Services.AddSingleton(services =>
        {
            return new HubConnectionBuilder().WithUrl(Url).WithAutomaticReconnect().Build();
        });
    }
    public override void UseMiddleware(WebApplication app)
    {
        app.MapHub<SignalRHub>(HubPath);
    }
    #endregion
}