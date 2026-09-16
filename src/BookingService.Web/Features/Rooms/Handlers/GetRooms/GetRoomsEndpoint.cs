using ErrorOr;
using Mediator;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Features.Auth;
using Microsoft.AspNetCore.WebUtilities;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Rooms.Handlers.GetRooms;

public static class GetRoomsEndpoint
{
    public const string Url = "/api/rooms";

    public static string[] AllowedRoles { get; } = AuthRoles.Any;

    public static string CreateSendableUrl(Guid[] ids)
    {
        IEnumerable<KeyValuePair<string, string?>> queryParams = ids.Select(id =>
        {
            return new KeyValuePair<string, string?>(nameof(ids), id.ToString());
        });
        return QueryHelpers.AddQueryString(Url, queryParams);
    }
    public static async Task<IResult> GetRooms(
        [FromQuery] Guid[]? ids,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<IEnumerable<RoomDto>> result = await mediator.Send(new GetRoomsQuery(ids ?? []), cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddGetRoomsProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
            return thisBuilder.AddAuthorizedBehaviorProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddGetRoomsEndpoint()
        {
            RouteHandlerBuilder routeBuilder = thisBuilder.MapGet(Url, GetRooms);
            routeBuilder.RequireAuthorization(cfg => cfg.RequireRole(AllowedRoles));
            routeBuilder.WithName(nameof(GetRooms));
            routeBuilder.Produces<IEnumerable<RoomDto>>(StatusCodes.Status200OK);
            routeBuilder.AddGetRoomsProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendGetRoomsAsync(Guid[] ids, CancellationToken cancellationToken)
        {
            return await thisHttpClient.GetAsync(CreateSendableUrl(ids), cancellationToken);
        }
        public async ValueTask<(HttpResponseMessage Message, RoomDto[]? Response)> SendGetRooms2Async(
            Guid[] ids,
            CancellationToken cancellationToken
        )
        {
            HttpResponseMessage message = await thisHttpClient.SendGetRoomsAsync(ids, cancellationToken);
            if(message.IsSuccessStatusCode)
            {
                try
                {
                    RoomDto[]? response = await message.Content.ReadFromJsonAsync<RoomDto[]>(cancellationToken);
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