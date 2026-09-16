using Bogus;
using System.Net;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Rooms;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.IntegrationTests.Seeders;
using BookingService.Web.Features.Auth.Handlers.LoginUser;
using BookingService.Web.Features.Rooms.Handlers.CreateRoom;

namespace BookingService.IntegrationTests.Features.Rooms.CreateRoom;

[DependsOn<LoginUserHandler>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class CreateRoomHandlerTests(AppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_ShouldCreateRoom(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.LoginAsAdminAsync(cancellationToken);
        CreateRoomCommand command = new Faker<CreateRoomCommand>().ValidInstance();

        //Act
        (HttpResponseMessage message, Guid? response) = await thisApp.HttpClient.SendCreateRoom2Async(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response).IsNotNull();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            command.Room.Id = response!.Value;
            RoomEntity? room = await db.Rooms.AsNoTracking().SingleOrDefaultAsync(cancellationToken);
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
        RoomEntity room = await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomEntity[] rooms = await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            return Faker.PickRandom(rooms);
        });
        CreateRoomCommand command = new Faker<CreateRoomCommand>().ValidInstance().WithRoom(room.ToDto());

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateRoomAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    [Test]
    public async ValueTask Handler_ShouldForbid_WhenLogginedAsNotAdmin(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        CreateRoomCommand command = new Faker<CreateRoomCommand>().ValidInstance();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateRoomAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        CreateRoomCommand command = new Faker<CreateRoomCommand>().ValidInstance();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateRoomAsync(command, cancellationToken);

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
        CreateRoomCommand command = new Faker<CreateRoomCommand>().ValidInstance().WithRoom(room.ToDto());

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateRoomAsync(command, cancellationToken);

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
        CreateRoomCommand command = new Faker<CreateRoomCommand>().ValidInstance().WithRoom(room.ToDto());

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateRoomAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }
}