using Bogus;
using System.Net;
using BookingService.Data.Features.Rooms;
using BookingService.Data.Features.Bookings;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.IntegrationTests.Seeders;
using BookingService.Data.Features.Auth.Users;
using BookingService.Web.Features.Bookings.Dtos;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Features.Bookings.Handlers.GetBookings;
using BookingService.IntegrationTests.Features.Auth.Handlers.LoginUser;

namespace BookingService.IntegrationTests.Features.Bookings.GetBookings;

[DependsOn<LoginUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class GetBookingsHandlerTests(AppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_ShouldReturnAllBookings_WhenNoIdsProvided(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        UserDto user = await thisApp.LoginAsAdminAsync(cancellationToken);
        BookingEntity[] bookings = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            return await new Faker<BookingEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseForUser(db, cancellationToken);
        });

        //Act
        (HttpResponseMessage message, BookingDto[]? bookingDtos) = await thisApp.HttpClient.SendGetBookings2Async([], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(bookingDtos).IsNotNull();
        await Assert.That(bookingDtos).IsEquivalentTo(bookings.ToDtos());
    }

    [Test]
    public async ValueTask Handler_ShouldReturnConcreteBookings_WhenIdsProvided(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        UserDto user = await thisApp.LoginAsAdminAsync(cancellationToken);
        BookingEntity[] bookings = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            return await new Faker<BookingEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseForUser(db, cancellationToken);
        });
        bookings = Faker.PickRandom(bookings, Faker.Random.Number(1, bookings.Length - 1)).ToArray();
        Guid[] ids = bookings.Select(room => room.Id).ToArray();

        //Act
        (HttpResponseMessage message, BookingDto[]? bookingDtos) = await thisApp.HttpClient.SendGetBookings2Async(ids, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(bookingDtos).IsNotNull();
        await Assert.That(bookingDtos).IsEquivalentTo(bookings.ToDtos());
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenIdsNotFound(CancellationToken cancellationToken)
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
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetBookingsAsync([Guid.NewGuid()], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandomAsync(cancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
            await new Faker<BookingEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseForUser(db, cancellationToken);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetBookingsAsync([], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }
}