using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Features.Auth;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Features.Bookings.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.Validation;

namespace BookingService.Web.Features.Bookings.Handlers.CreateBooking;

public static class CreateBookingEndpoint
{
    public const string Url = "/api/booking";

    public static string[] AllowedRoles { get; } = AuthRoles.Any;

    public static async Task<IResult> CreateBooking(
        [FromBody] BookingDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Guid> result = await mediator.Send(new CreateBookingCommand(dto), cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddCreateBookingProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status409Conflict);
            thisBuilder.AddAuthorizedBehaviorProductionProblems();
            return thisBuilder.AddValidationBehaviorProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddCreateBookingEndpoint()
        {
            RouteHandlerBuilder routeBuilder = thisBuilder.MapPost(Url, CreateBooking);
            routeBuilder.RequireAuthorization(cfg => cfg.RequireRole(AllowedRoles));
            routeBuilder.WithName(nameof(CreateBooking));
            routeBuilder.Produces<Guid>(StatusCodes.Status200OK);
            routeBuilder.AddCreateBookingProductionProblems();
        }
    }
}