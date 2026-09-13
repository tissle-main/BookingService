using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Features.Auth;
using BookingService.Web.Shared.Extensions;

namespace BookingService.Web.Shared.DbSeeding;

public static class DbSeedingEndpoint
{
    public const string Url = "/api/dbseeding";

    public static async Task<IResult> DbSeeding([FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        ErrorOr<Unit> errorOrUnit = await mediator.Send(new DbSeedingCommand(), cancellationToken);
        return errorOrUnit.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddDbSeedingProductionProblems()
        {        
            return thisBuilder.AddAuthDbSeederProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddDbSeedingEndpoint()
        {
            RouteHandlerBuilder routeBuilder = thisBuilder.MapPost(Url, DbSeeding);
            routeBuilder.WithName(nameof(DbSeeding));
            routeBuilder.Produces(StatusCodes.Status204NoContent);
            routeBuilder.AddDbSeedingProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendDbSeedingAsync(CancellationToken cancellationToken)
        {
            return await thisHttpClient.PostAsync(Url, null, cancellationToken);
        }
    }
}