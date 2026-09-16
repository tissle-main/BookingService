using Mediator;

namespace BookingService.Web.Shared.Behaviors.Authorized;

/// <summary>Marks a Mediator message that requires an authenticated user in an allowed role.</summary>
public interface IAuthorizedBehaviorMessage : IMessage
{
    /// <summary>Gets the roles permitted to handle the message.</summary>
    public abstract string[] AllowedRoles { get; }
}