using Bogus;
using System.Net;
using BookingService.Data.Features.Rooms;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.Web.Features.Auth.Handlers.LoginUser;
using BookingService.Web.Features.Rooms.Handlers.GetRooms;

namespace BookingService.IntegrationTests.Features.Rooms.GetRooms;

[DependsOn<LoginUserHandler>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class GetRoomsHandlerTests(AppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_ShouldReturnAllRooms_WhenNoIdsProvided(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.LoginAsAdminAsync(cancellationToken);
        RoomEntity[] rooms = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
        });

        //Act
        (HttpResponseMessage message, RoomDto[]? roomDtos) = await thisApp.HttpClient.SendGetRooms2Async([], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(roomDtos).IsNotNull();
        await Assert.That(roomDtos).IsEquivalentTo(rooms.ToDtos());
    }

    [Test]
    public async ValueTask Handler_ShouldReturnConcreteRooms_WhenIdsProvided(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.LoginAsAdminAsync(cancellationToken);
        RoomEntity[] rooms = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
        });
        rooms = Faker.PickRandom(rooms, Faker.Random.Number(1, rooms.Length - 1)).ToArray();
        Guid[] ids = rooms.Select(room => room.Id).ToArray();

        //Act
        (HttpResponseMessage message, RoomDto[]? roomDtos) = await thisApp.HttpClient.SendGetRooms2Async(ids, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(roomDtos).IsNotNull();
        await Assert.That(roomDtos).IsEquivalentTo(rooms.ToDtos());
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenIdsNotFound(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.LoginAsAdminAsync(cancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetRoomsAsync([Guid.NewGuid()], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetRoomsAsync([], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }
}