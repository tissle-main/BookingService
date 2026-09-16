using ErrorOr;
using Mediator;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Features.Auth;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.Validation;

namespace BookingService.Web.Features.Rooms.Handlers.CreateRoom;

/// <summary>Defines the HTTP contract for creating rooms.</summary>
public static class CreateRoomEndpoint
{
    public const string Url = "/api/room";

    public static string[] AllowedRoles { get; } = [AuthRoles.Admin];

    public static async Task<IResult> CreateRoom(
        [FromBody] RoomDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Guid> result = await mediator.Send(new CreateRoomCommand(dto), cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddCreateRoomProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status409Conflict);
            thisBuilder.AddAuthorizedBehaviorProductionProblems();
            return thisBuilder.AddValidationBehaviorProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddCreateRoomEndpoint()
        {
            RouteHandlerBuilder routeBuilder = thisBuilder.MapPost(Url, CreateRoom);
            routeBuilder.RequireAuthorization(cfg => cfg.RequireRole(AllowedRoles));
            routeBuilder.WithName(nameof(CreateRoom));
            routeBuilder.Produces<Guid>(StatusCodes.Status200OK);
            routeBuilder.AddCreateRoomProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendCreateRoomAsync(CreateRoomCommand command, CancellationToken cancellationToken)
        {
            return await thisHttpClient.PostAsJsonAsync(Url, command.Room, cancellationToken);
        }
        public async ValueTask<(HttpResponseMessage Message, Guid? Response)> SendCreateRoom2Async(
            CreateRoomCommand command,
            CancellationToken cancellationToken
        )
        {
            HttpResponseMessage message = await thisHttpClient.SendCreateRoomAsync(command, cancellationToken);
            if(message.IsSuccessStatusCode)
            {
                try
                {
                    Guid? response = await message.Content.ReadFromJsonAsync<Guid?>(cancellationToken);
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