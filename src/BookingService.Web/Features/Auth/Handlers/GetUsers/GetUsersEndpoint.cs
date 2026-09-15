using ErrorOr;
using Mediator;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Auth.Handlers.GetUsers;

public static class GetUsersEndpoint
{
    public const string Url = "/api/users";

    public static string[] AllowedRoles
    {
        get => [AuthRoles.Admin];
    }

    public static string CreateSendableUrl(Guid[] ids)
    {
        IEnumerable<KeyValuePair<string, string?>> queryParams = ids.Select(id =>
        {
            return new KeyValuePair<string, string?>(nameof(ids), id.ToString());
        });
        return QueryHelpers.AddQueryString(Url, queryParams);
    }
    public static async Task<IResult> GetUsers(
        [FromQuery] Guid[]? ids,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<IEnumerable<UserDto>> result = await mediator.Send(new GetUsersQuery(ids ?? []), cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddGetUsersProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
            return thisBuilder.AddAuthorizedBehaviorProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddGetUsersEndpoint()
        {
            RouteHandlerBuilder routeBuilder = thisBuilder.MapGet(Url, GetUsers);
            routeBuilder.RequireAuthorization(cfg => cfg.RequireRole(AllowedRoles));
            routeBuilder.WithName(nameof(GetUsers));
            routeBuilder.Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK);
            routeBuilder.AddGetUsersProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendGetUsersAsync(Guid[] ids, CancellationToken cancellationToken)
        {
            return await thisHttpClient.GetAsync(CreateSendableUrl(ids), cancellationToken);
        }
        public async ValueTask<(HttpResponseMessage Message, UserDto[]? Response)> SendGetUsers2Async(
            Guid[] ids,
            CancellationToken cancellationToken
        )
        {
            HttpResponseMessage message = await thisHttpClient.SendGetUsersAsync(ids, cancellationToken);
            if(message.IsSuccessStatusCode)
            {
                try
                {
                    UserDto[]? response = await message.Content.ReadFromJsonAsync<UserDto[]>(cancellationToken);
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