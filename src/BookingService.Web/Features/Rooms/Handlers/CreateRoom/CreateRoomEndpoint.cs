using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Features.Auth;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.Validation;

namespace BookingService.Web.Features.Rooms.Handlers.CreateRoom;

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
}