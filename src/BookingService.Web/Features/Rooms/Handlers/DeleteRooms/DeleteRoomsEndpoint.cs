using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Features.Auth;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Rooms.Handlers.DeleteRooms;

public static class DeleteRoomsEndpoint
{
    public const string Url = "/api/rooms";

    public static string[] AllowedRoles { get; } = [AuthRoles.Admin];

    public static async Task<IResult> DeleteRooms(
        [FromQuery] Guid[]? ids,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> result = await mediator.Send(new DeleteRoomsCommand(ids ?? []), cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddDeleteRoomsProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
            return thisBuilder.AddAuthorizedBehaviorProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddDeleteRoomsEndpoint()
        {
            RouteHandlerBuilder routeBuilder = thisBuilder.MapDelete(Url, DeleteRooms);
            routeBuilder.RequireAuthorization(cfg => cfg.RequireRole(AllowedRoles));
            routeBuilder.WithName(nameof(DeleteRooms));
            routeBuilder.Produces(StatusCodes.Status204NoContent);
            routeBuilder.AddDeleteRoomsProductionProblems();
        }
    }
}