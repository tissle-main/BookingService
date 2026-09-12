using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Shared.Extensions;

namespace BookingService.Web.Features.Auth.Handlers.LogoutUser;

public static class LogoutUserEndpoint
{
    public const string Url = "/api/auth/logout";
    public static async Task<IResult> LogoutUser(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> result = await mediator.Send(new LogoutUserCommand(), cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddLogoutUserProductionProblems()
        {
            return thisBuilder;
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddLogoutUserEndpoint()
        {
            thisBuilder.MapPost(Url, LogoutUser)
                .WithName(nameof(LogoutUser))
                .Produces(StatusCodes.Status204NoContent)
                .AddLogoutUserProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendLogoutUserAsync(CancellationToken cancellationToken)
        {
            return await thisHttpClient.PostAsync(Url, null, cancellationToken);
        }
    }
}