using ErrorOr;
using Mediator;
using Microsoft.Extensions.Options;
using BookingService.Web.Features.Auth.Options;

namespace BookingService.Web.Features.Auth.Handlers.GetAdminCredentials;

public sealed class GetAdminCredentialsHandler(IOptions<AdminCredentials> adminCredentials) : IQueryHandler<GetAdminCredentialsQuery, ErrorOr<AdminCredentials>>
{
    #region Interfaces
    public ValueTask<ErrorOr<AdminCredentials>> Handle(GetAdminCredentialsQuery query, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(adminCredentials.Value.ToErrorOr());
    }
    #endregion
}