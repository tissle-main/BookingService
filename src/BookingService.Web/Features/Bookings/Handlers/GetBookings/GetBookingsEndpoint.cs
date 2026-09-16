using ErrorOr;
using Mediator;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Features.Auth;
using Microsoft.AspNetCore.WebUtilities;
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

    public static string CreateSendableUrl(Guid[] ids)
    {
        IEnumerable<KeyValuePair<string, string?>> queryParams = ids.Select(id =>
        {
            return new KeyValuePair<string, string?>(nameof(ids), id.ToString());
        });
        return QueryHelpers.AddQueryString(Url, queryParams);
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
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendGetBookingsAsync(Guid[] ids, CancellationToken cancellationToken)
        {
            return await thisHttpClient.GetAsync(CreateSendableUrl(ids), cancellationToken);
        }
        public async ValueTask<(HttpResponseMessage Message, BookingDto[]? Response)> SendGetBookings2Async(
            Guid[] ids,
            CancellationToken cancellationToken
        )
        {
            HttpResponseMessage message = await thisHttpClient.SendGetBookingsAsync(ids, cancellationToken);
            if(message.IsSuccessStatusCode)
            {
                try
                {
                    BookingDto[]? response = await message.Content.ReadFromJsonAsync<BookingDto[]>(cancellationToken);
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