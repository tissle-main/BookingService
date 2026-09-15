using ErrorOr;
using Mediator;
using BookingService.Web.Features.Auth.Options;

namespace BookingService.Web.Features.Auth.Handlers.GetAdminCredentials;

public sealed record class GetAdminCredentialsQuery() : IQuery<ErrorOr<AdminCredentials>>;