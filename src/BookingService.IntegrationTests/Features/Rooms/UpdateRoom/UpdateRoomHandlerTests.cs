using Bogus;
using System.Net;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Rooms;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.IntegrationTests.Seeders;
using BookingService.Web.Features.Auth.Handlers.LoginUser;
using BookingService.Web.Features.Rooms.Handlers.CreateRoom;
using BookingService.Web.Features.Rooms.Handlers.UpdateRoom;

namespace BookingService.IntegrationTests.Features.Rooms.UpdateRoom;

[DependsOn<LoginUserHandler>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class UpdateRoomHandlerTests(AppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_ShouldUpdateRoom(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.LoginAsAdminAsync(cancellationToken);
        RoomEntity room = await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomEntity[] rooms = await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            return Faker.PickRandom(rooms);
        });
        UpdateRoomCommand command = new Faker<UpdateRoomCommand>().ValidInstance();
        command.Room.Id = room.Id;

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateRoomAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomEntity? room = await db.Rooms.AsNoTracking().SingleOrDefaultAsync(e => e.Id == command.Room.Id, cancellationToken);
            await Assert.That(room).IsNotNull();
            await Assert.That(room.ToDto()).IsEquivalentTo(command.Room).IgnoringMember(nameof(RoomDto.CreatedAt));
        });
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenNameExists(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.LoginAsAdminAsync(cancellationToken);
        RoomEntity[] rooms = await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomEntity[] rooms = await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            return Faker.PickRandom(rooms, 2).ToArray();
        });
        UpdateRoomCommand command = new Faker<UpdateRoomCommand>().ValidInstance().WithRoom(rooms[0].ToDto());
        command.Room.Name = rooms[1].Name;

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateRoomAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    [Test]
    public async ValueTask Handler_ShouldNotFail_WhenUpdateSameInstance(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.LoginAsAdminAsync(cancellationToken);
        RoomEntity room = await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomEntity[] rooms = await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            return Faker.PickRandom(rooms);
        });
        UpdateRoomCommand command = new Faker<UpdateRoomCommand>().ValidInstance().WithRoom(room.ToDto());

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateRoomAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
    }

    [Test]
    public async ValueTask Handler_ShouldForbid_WhenLogginedAsNotAdmin(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        RoomEntity room = await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomEntity[] rooms = await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            return Faker.PickRandom(rooms);
        });
        UpdateRoomCommand command = new Faker<UpdateRoomCommand>().ValidInstance();
        command.Room.Id = room.Id;

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateRoomAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        RoomEntity room = await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomEntity[] rooms = await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            return Faker.PickRandom(rooms);
        });
        UpdateRoomCommand command = new Faker<UpdateRoomCommand>().ValidInstance();
        command.Room.Id = room.Id;

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateRoomAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenNameIsEmpty(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.LoginAsAdminAsync(cancellationToken);
        RoomEntity room = new Faker<RoomEntity>().ValidInstance().WithEmptyName();
        UpdateRoomCommand command = new Faker<UpdateRoomCommand>().ValidInstance().WithRoom(room.ToDto());

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateRoomAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenNameIsTooLarge(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.LoginAsAdminAsync(cancellationToken);
        RoomEntity room = new Faker<RoomEntity>().ValidInstance().WithTooLargeName();
        UpdateRoomCommand command = new Faker<UpdateRoomCommand>().ValidInstance().WithRoom(room.ToDto());

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateRoomAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }
}