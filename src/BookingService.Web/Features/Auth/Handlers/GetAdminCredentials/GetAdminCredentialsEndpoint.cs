using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Features.Auth.Options;

namespace BookingService.Web.Features.Auth.Handlers.GetAdminCredentials;

public static class GetAdminCredentialsEndpoint
{
    public const string Url = "/api/admin-credentials";

    public static async Task<IResult> GetAdminCredentials(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<AdminCredentials> result = await mediator.Send(new GetAdminCredentialsQuery(), cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddGetAdminCredentialsProductionProblems()
        {
            return thisBuilder;
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddGetAdminCredentialsEndpoint()
        {
            RouteHandlerBuilder routeBuilder = thisBuilder.MapGet(Url, GetAdminCredentials);
            routeBuilder.WithName(nameof(GetAdminCredentials));
            routeBuilder.Produces<AdminCredentials>(StatusCodes.Status200OK);
            routeBuilder.AddGetAdminCredentialsProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendGetAdminCredentialsAsync(CancellationToken cancellationToken)
        {
            return await thisHttpClient.GetAsync(Url, cancellationToken);
        }
        public async ValueTask<(HttpResponseMessage Message, AdminCredentials? Response)> SendGetAdminCredentials2Async(CancellationToken cancellationToken)
        {
            HttpResponseMessage message = await thisHttpClient.SendGetAdminCredentialsAsync(cancellationToken);
            if(message.IsSuccessStatusCode)
            {
                AdminCredentials? response = await message.Content.ReadFromJsonAsync<AdminCredentials>(cancellationToken);
                return (message, response);
            }
            return (message, null);
        }
    }
}