using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Shared.Behaviors.Validation;

namespace BookingService.Web.Features.Auth.Handlers.RegisterUser;

public static class RegisterUserEndpoint
{
    public const string Url = "/api/auth/register";

    public static async Task<IResult> RegisterUser(
        [FromBody] RegisterUserCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> errorOrUnit = await mediator.Send(command, cancellationToken);
        return errorOrUnit.ToHttpResult();
    }
    
    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddRegisterUserProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status400BadRequest);
            thisBuilder.ProducesProblem(StatusCodes.Status409Conflict);
            return thisBuilder.AddValidationBehaviorProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddRegisterUserEndpoint()
        {
            RouteHandlerBuilder routeBuilder = thisBuilder.MapPost(Url, RegisterUser);
            routeBuilder.WithName(nameof(RegisterUser));
            routeBuilder.Produces(StatusCodes.Status204NoContent);
            routeBuilder.AddRegisterUserProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendRegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            return await thisHttpClient.PostAsJsonAsync(Url, command, cancellationToken);
        }
    }
}