using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Auth.Handlers.GetUsers;

public static class GetUsersEndpoint
{
    public const string Url = "/api/users";

    public static string[] AllowedRoles
    {
        get => AuthRoles.Any;
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
}