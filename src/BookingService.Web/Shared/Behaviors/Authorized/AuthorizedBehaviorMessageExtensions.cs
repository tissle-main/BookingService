using System.Runtime.CompilerServices;
using BookingService.Data.Features.Auth.Roles;
using BookingService.Data.Features.Auth.Users;

namespace BookingService.Web.Shared.Behaviors.Authorized;

/// <summary>
/// Provides extension properties for <see cref="IAuthorizedBehaviorMessage"/>
/// </summary>
public static class AuthorizedBehaviorMessageExtensions
{
    private static ConditionalWeakTable<IAuthorizedBehaviorMessage, AuthorizedBehaviorMessageExtraProperties> ExtraPropertiesTable { get; } = [];

    extension(IAuthorizedBehaviorMessage thisMessage)
    {
        /// <summary>Gets the authenticated user.</summary>
        public UserEntity User
        {
            get => thisMessage.GetExtraProperties().User;
            set
            {
                thisMessage.GetExtraProperties().User = value;
            }
        }

        /// <summary>Gets or sets the roles assigned to the authenticated user.</summary>
        public IList<string> Roles
        {
            get => thisMessage.GetExtraProperties().Roles;
            set
            {
                thisMessage.GetExtraProperties().Roles = value;
            }
        }

        private AuthorizedBehaviorMessageExtraProperties GetExtraProperties()
        {
            return ExtraPropertiesTable.GetOrAdd(thisMessage, static _ => new AuthorizedBehaviorMessageExtraProperties());
        }
    }
}