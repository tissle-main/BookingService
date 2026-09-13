using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Features.Auth;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Features.Bookings.Dtos;

namespace BookingService.Web.Features.Bookings.Handlers.GetBookings;

public static class GetBookingsEndpoint
{
    public const string Url = "/api/bookings";

    public static string[] AllowedRoles
    {
        get => AuthRoles.Any;
    }

    public static async Task<IResult> GetBookings(
        [FromQuery] Guid[]? ids,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<IEnumerable<BookingDto>> result = await mediator.Send(new GetBookingsQuery(ids ?? []), cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddGetBookingsProductionProblems()
        {
            return thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddGetBookingsEndpoint()
        {
            RouteHandlerBuilder routeBuilder = thisBuilder.MapGet(Url, GetBookings);
            routeBuilder.WithName(nameof(GetBookings));
            routeBuilder.Produces<IEnumerable<BookingDto>>(StatusCodes.Status200OK);
            routeBuilder.AddGetBookingsProductionProblems();
        }
    }
}