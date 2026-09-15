using ErrorOr;
using Mediator;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Auth.Handlers.GetUser;

public static class GetUserEndpoint
{
    public const string Url = "/api/user";

    public static string[] AllowedRoles
    {
        get => AuthRoles.Any;
    }

    public static async Task<IResult> GetUser(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<UserDto> result = await mediator.Send(new GetUserQuery(), cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddGetUserProductionProblems()
        {
            return thisBuilder.AddAuthorizedBehaviorProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddGetUserEndpoint()
        {
            RouteHandlerBuilder routeBuilder = thisBuilder.MapGet(Url, GetUser);
            routeBuilder.RequireAuthorization(cfg => cfg.RequireRole(AllowedRoles));
            routeBuilder.WithName(nameof(GetUser));
            routeBuilder.Produces<UserDto>(StatusCodes.Status200OK);
            routeBuilder.AddGetUserProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendGetUserAsync(CancellationToken cancellationToken)
        {
            return await thisHttpClient.GetAsync(Url, cancellationToken);
        }
        public async ValueTask<(HttpResponseMessage Message, UserDto? Response)> SendGetUser2Async(CancellationToken cancellationToken)
        {
            HttpResponseMessage message = await thisHttpClient.SendGetUserAsync(cancellationToken);
            if(message.IsSuccessStatusCode)
            {
                try
                {
                    UserDto? response = await message.Content.ReadFromJsonAsync<UserDto>(cancellationToken);
                    return (message, response);
                }
                catch(JsonException)
                {
                    return (message, null);
                }
            }
            return (message, null);
        }
    }
}