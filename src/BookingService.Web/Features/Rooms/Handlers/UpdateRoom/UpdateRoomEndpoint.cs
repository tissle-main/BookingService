using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Features.Auth;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.Validation;

namespace BookingService.Web.Features.Rooms.Handlers.UpdateRoom;

public static class UpdateRoomEndpoint
{
    public const string Url = "/api/room";

    public static string[] AllowedRoles { get; } = [AuthRoles.Admin];

    public static async Task<IResult> UpdateRoom(
        [FromBody] RoomDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> result = await mediator.Send(new UpdateRoomCommand(dto), cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddUpdateRoomProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
            thisBuilder.ProducesProblem(StatusCodes.Status409Conflict);
            thisBuilder.AddAuthorizedBehaviorProductionProblems();
            return thisBuilder.AddValidationBehaviorProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddUpdateRoomEndpoint()
        {
            RouteHandlerBuilder routeBuilder = thisBuilder.MapPut(Url, UpdateRoom);
            routeBuilder.RequireAuthorization(cfg => cfg.RequireRole(AllowedRoles));
            routeBuilder.WithName(nameof(UpdateRoom));
            routeBuilder.Produces(StatusCodes.Status204NoContent);
            routeBuilder.AddUpdateRoomProductionProblems();
        }
    }
}