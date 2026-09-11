using Mediator;

namespace BookingService.Web.Shared.Behaviors.Authorized;

public interface IAuthorizedBehaviorMessage : IMessage
{
    public abstract string Role { get; }
}