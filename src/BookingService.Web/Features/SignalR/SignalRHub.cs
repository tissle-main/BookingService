using Microsoft.AspNetCore.SignalR;

namespace BookingService.Web.Features.SignalR;

/// <summary>SignalR hub used to broadcast room and booking updates.</summary>
public sealed class SignalRHub : Hub<ISignalRClient>;