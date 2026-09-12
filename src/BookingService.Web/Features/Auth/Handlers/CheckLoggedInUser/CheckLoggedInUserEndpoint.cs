using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Auth.Handlers.CheckLoggedInUser;

public static class CheckLoggedInUserEndpoint
{
    public const string Url = "/api/auth/check";

    public static async Task<IResult> CheckLoggedInUser(
        [FromQuery] string email,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<bool> result = await mediator.Send(new CheckLoggedInUserCommand(email), cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddCheckLoggedInUserProductionProblems()
        {
            return thisBuilder.AddAuthorizedBehaviorProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddCheckLoggedInUserEndpoint()
        {
            thisBuilder.MapGet(Url, CheckLoggedInUser)
                .RequireAuthorization(cfg => cfg.RequireRole(AuthRoles.User, AuthRoles.Admin))
                .WithName(nameof(CheckLoggedInUser))
                .Produces<bool>(StatusCodes.Status200OK)
                .AddCheckLoggedInUserProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendCheckLoggedInUserAsync(string email, CancellationToken cancellationToken)
        {
            string url = $"{Url}?{nameof(email)}={Uri.EscapeDataString(email)}";
            return await thisHttpClient.GetAsync(url, cancellationToken);
        }
        public async ValueTask<(HttpResponseMessage Message, bool? Response)> SendCheckLoggedInUser2Async(
            string email,
            CancellationToken cancellationToken
        )
        {
            HttpResponseMessage message = await thisHttpClient.SendCheckLoggedInUserAsync(email, cancellationToken);
            if(message.IsSuccessStatusCode)
            {
                bool? response = await message.Content.ReadFromJsonAsync<bool?>(cancellationToken);
                return (message, response);
            }
            return (message, null);
        }
    }
}