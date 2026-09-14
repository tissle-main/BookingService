using BookingService.ServiceDefaults;
using Microsoft.AspNetCore.SignalR.Client;

namespace BookingService.Web.Features.SignalR;

public sealed class SignalRFeatureProvider : FeatureProvider
{
    #region Static
    public const string HubPath = "/signalr";

    public static string Url { get; private set; } = "";
    #endregion

    #region Base
    public override void AddServices(WebApplicationBuilder builder)
    {
        string baseUrl = builder.Configuration["ASPNETCORE_URLS"]?.Split(';')[0] ?? throw new NullReferenceException("Server urls not found");
        Url = $"{baseUrl}{HubPath}";

        if(builder.Environment.IsProduction())
        {
            builder.Services.AddSignalR().AddAzureSignalR(AppHostResources.SignalR);
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