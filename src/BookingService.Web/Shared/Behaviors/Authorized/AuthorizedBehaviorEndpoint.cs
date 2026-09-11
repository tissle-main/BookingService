namespace BookingService.Web.Shared.Behaviors.Authorized;

public static class AuthorizedBehaviorEndpoint
{
    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddAuthorizedBehaviorProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status401Unauthorized);
            return thisBuilder.ProducesProblem(StatusCodes.Status403Forbidden);
        }
    }
}