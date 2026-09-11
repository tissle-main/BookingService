namespace BookingService.Web.Shared.Behaviors.Validation;

public static class ValidationBehaviorEndpoint
{
    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddValidationBehaviorProductionProblems()
        {
            return thisBuilder.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
        }
    }
}