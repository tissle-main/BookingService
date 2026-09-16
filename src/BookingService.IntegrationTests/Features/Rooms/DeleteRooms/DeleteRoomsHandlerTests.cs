using Bogus;
using System.Net;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Rooms;
using BookingService.Data.Features.Bookings;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.IntegrationTests.Seeders;
using BookingService.Web.Features.Bookings.Dtos;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Features.Rooms.Handlers.DeleteRooms;
using BookingService.IntegrationTests.Features.Auth.Handlers.LoginUser;

namespace BookingService.IntegrationTests.Features.Rooms.DeleteRooms;

[DependsOn<LoginUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class DeleteRoomsHandlerTests(AppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_ShouldDeleteAllRooms_WhenNoIdsProvided(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        UserDto user = await thisApp.LoginAsAdminAsync(cancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            await new Faker<BookingEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseForUser(db, cancellationToken);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteRoomsAsync([], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool anyRooms = await db.Rooms.AnyAsync(cancellationToken);
            await Assert.That(anyRooms).IsFalse();

            bool anyBookings = await db.Bookings.AnyAsync(cancellationToken);
            await Assert.That(anyBookings).IsFalse();
        });
    }

    [Test]
    public async ValueTask Handler_ShouldDeleteConcreteRooms_WhenIdsProvided(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        UserDto user = await thisApp.LoginAsAdminAsync(cancellationToken);
        (RoomEntity[] rooms, BookingEntity[] bookings) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomEntity[] rooms = await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            BookingEntity[] bookings = await new Faker<BookingEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseForUser(db, cancellationToken);
            return (rooms, bookings);
        });
        RoomEntity[] deleteRooms = Faker.PickRandom(rooms, Faker.Random.Number(1, rooms.Length - 1)).ToArray();
        Guid[] ids = deleteRooms.Select(room => room.Id).ToArray();
        BookingEntity[] expectedBookings = bookings.Where(e => !ids.Contains(e.RoomId)).ToArray();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteRoomsAsync(ids, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomEntity[] actualRooms = await db.Rooms.AsNoTracking().ToArrayAsync(cancellationToken);
            await Assert.That(actualRooms).IsEquivalentTo(rooms.Where(e => !ids.Contains(e.Id)));

            BookingEntity[] actualBookings = await db.Bookings.AsNoTracking().ToArrayAsync(cancellationToken);
            await Assert.That(actualBookings).IsEquivalentTo(expectedBookings);
        });
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenIdsNotFound(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        UserDto user = await thisApp.LoginAsAdminAsync(cancellationToken);
        (RoomEntity[] rooms, BookingEntity[] bookings) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomEntity[] rooms = await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            BookingEntity[] bookings = await new Faker<BookingEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseForUser(db, cancellationToken);
            return (rooms, bookings);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteRoomsAsync([Guid.NewGuid()], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomEntity[] actualRooms = await db.Rooms.AsNoTracking().ToArrayAsync(cancellationToken);
            await Assert.That(actualRooms).IsEquivalentTo(rooms);

            BookingEntity[] actualBookings = await db.Bookings.AsNoTracking().ToArrayAsync(cancellationToken);
            await Assert.That(actualBookings).IsEquivalentTo(bookings);
        });
    }

    [Test]
    public async ValueTask Handler_ShouldForbid_WhenLogginedAsNotAdmin(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteRoomsAsync([], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteRoomsAsync([], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }
}