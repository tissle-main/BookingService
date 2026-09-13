using BookingService.Web.Features.Rooms.Handlers.GetRooms;
using BookingService.Web.Features.Rooms.Handlers.CreateRoom;
using BookingService.Web.Features.Rooms.Handlers.UpdateRoom;
using BookingService.Web.Features.Rooms.Handlers.DeleteRooms;

namespace BookingService.Web.Features.Rooms;

public sealed class RoomFeatureProvider : FeatureProvider
{
    #region Base
    public override void UseMiddleware(WebApplication app)
    {
        if(app.Environment.IsEnvironment(ProfileNames.Test))
        {
            app.AddGetRoomsEndpoint();
            app.AddCreateRoomEndpoint();
            app.AddUpdateRoomEndpoint();
            app.AddDeleteRoomsEndpoint();
        }
    }
    #endregion
}