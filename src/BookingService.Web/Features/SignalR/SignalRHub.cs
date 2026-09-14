using Microsoft.AspNetCore.SignalR;

namespace BookingService.Web.Features.SignalR;

public sealed class SignalRHub : Hub<ISignalRClient>;