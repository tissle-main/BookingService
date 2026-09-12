using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using BookingService.Web.Shared.Extensions;
using BookingService.Web.Shared.Behaviors.Validation;

namespace BookingService.Web.Features.Auth.Handlers.LoginUser;

public static class LoginUserEndpoint
{
    public const string Url = "/api/auth/login";

    public static async Task<IResult> LoginUser(
        [FromBody] LoginUserCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<LoginUserResponse> result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResult();
    }

    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddLoginUserProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status400BadRequest);
            thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
            return thisBuilder.AddValidationBehaviorProductionProblems();
        }
    }
    extension(IEndpointRouteBuilder thisBuilder)
    {
        public void AddLoginUserEndpoint()
        {
            thisBuilder.MapPost(Url, LoginUser)
                .WithName(nameof(LoginUser))
                .Produces<LoginUserResponse>(StatusCodes.Status200OK)
                .AddLoginUserProductionProblems();
        }
    }
    extension(HttpClient thisHttpClient)
    {
        public async ValueTask<HttpResponseMessage> SendLoginUserAsync(LoginUserCommand command, CancellationToken cancellationToken)
        {
            return await thisHttpClient.PostAsJsonAsync(Url, command, cancellationToken);
        }
        public async ValueTask<(HttpResponseMessage Message, LoginUserResponse? Response)> SendLoginUser2Async(
            LoginUserCommand command,
            CancellationToken cancellationToken
        )
        {
            HttpResponseMessage message = await thisHttpClient.SendLoginUserAsync(command, cancellationToken);
            if(message.IsSuccessStatusCode)
            {
                LoginUserResponse? response = await message.Content.ReadFromJsonAsync<LoginUserResponse>(cancellationToken);
                return (message, response);
            }
            return (message, null);
        }
    } 
}