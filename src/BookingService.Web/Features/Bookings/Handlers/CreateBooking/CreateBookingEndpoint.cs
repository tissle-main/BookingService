using ErrorOr;
using Mediator;
using System.Text.Json;
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
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendCreateBookingAsync(CreateBookingCommand command, CancellationToken cancellationToken)
        {
            return await thisHttpClient.PostAsJsonAsync(Url, command.Booking, cancellationToken);
        }
        public async ValueTask<(HttpResponseMessage Message, Guid? Response)> SendCreateBooking2Async(
            CreateBookingCommand command,
            CancellationToken cancellationToken
        )
        {
            HttpResponseMessage message = await thisHttpClient.SendCreateBookingAsync(command, cancellationToken);
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