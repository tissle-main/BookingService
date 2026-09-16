using Bogus;
using System.Net;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Rooms;
using BookingService.Data.Features.Bookings;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.IntegrationTests.Seeders;
using BookingService.Data.Features.Auth.Users;
using BookingService.Web.Features.Bookings.Dtos;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Features.Rooms.Handlers.CreateRoom;
using BookingService.Web.Features.Bookings.Handlers.CreateBooking;
using BookingService.IntegrationTests.Features.Auth.Handlers.LoginUser;

namespace BookingService.IntegrationTests.Features.Bookings.CreateBooking;

[DependsOn<LoginUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class CreateBookingHandlerTests(AppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_ShouldCreateBooking(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        UserDto user = await thisApp.LoginAsAdminAsync(cancellationToken);
        RoomEntity[] rooms = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
        });
        Guid roomId = Faker.PickRandom(rooms).Id;
        BookingDto booking = new Faker<BookingEntity>().ValidInstance().WithUserId(user.Id).WithRoomId(roomId).Generate().ToDto();
        CreateBookingCommand command = new Faker<CreateBookingCommand>().ValidInstance().WithBooking(booking);

        //Act
        (HttpResponseMessage message, Guid? response) = await thisApp.HttpClient.SendCreateBooking2Async(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response).IsNotNull();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            command.Booking.Id = response!.Value;
            BookingEntity? booking = await db.Bookings.AsNoTracking().SingleOrDefaultAsync(cancellationToken);
            await Assert.That(booking).IsNotNull();
            await Assert.That(booking.ToDto()).IsEquivalentTo(command.Booking);
        });
    }

    [Test]
    [DependsOn(nameof(Handler_ShouldCreateBooking))]
    public async ValueTask Handler_ShouldFail_WhenBookingPeriodOverlaps(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        UserDto user = await thisApp.LoginAsAdminAsync(cancellationToken);
        RoomEntity[] rooms = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
        });
        Guid roomId = Faker.PickRandom(rooms).Id;
        BookingDto booking = new Faker<BookingEntity>().ValidInstance().WithUserId(user.Id).WithRoomId(roomId).Generate().ToDto();
        CreateBookingCommand command = new Faker<CreateBookingCommand>().ValidInstance().WithBooking(booking);
        (HttpResponseMessage message, Guid? response) = await thisApp.HttpClient.SendCreateBooking2Async(command, cancellationToken);
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response).IsNotNull();

        //Act
        (message, response) = await thisApp.HttpClient.SendCreateBooking2Async(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    [Test]
    [DependsOn(nameof(Handler_ShouldCreateBooking))]
    [DependsOn(nameof(Handler_ShouldFail_WhenBookingPeriodOverlaps))]
    public async ValueTask Handler_ShouldCreateBooking_Concurrent(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        UserDto user = await thisApp.LoginAsAdminAsync(cancellationToken);
        RoomEntity[] rooms = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
        });
        Guid roomId = Faker.PickRandom(rooms).Id;
        BookingDto booking = new Faker<BookingEntity>().ValidInstance().WithUserId(user.Id).WithRoomId(roomId).Generate().ToDto();
        CreateBookingCommand command = new Faker<CreateBookingCommand>().ValidInstance().WithBooking(booking);

        //Act
        Task<(HttpResponseMessage Message, Guid? Response)>[] tasks = Enumerable.Range(0, 20).Select(_ =>
        {
            return thisApp.HttpClient.SendCreateBooking2Async(command, cancellationToken).AsTask();
        }).ToArray();
        await Task.WhenAll(tasks);

        //Assert
        Task<(HttpResponseMessage Message, Guid? Response)>? successTask = tasks.SingleOrDefault(task => task.Result.Message.StatusCode is HttpStatusCode.OK);
        await Assert.That(successTask is not null).IsTrue();
        await Assert.That(tasks.Except([successTask]).All(task => task!.Result.Message.StatusCode is HttpStatusCode.Conflict)).IsTrue();
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandomAsync(cancellationToken);
        RoomEntity[] rooms = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
        });
        Guid roomId = Faker.PickRandom(rooms).Id;
        BookingDto booking = new Faker<BookingEntity>().ValidInstance().WithUserId(user.Id).WithRoomId(roomId).Generate().ToDto();
        CreateBookingCommand command = new Faker<CreateBookingCommand>().ValidInstance().WithBooking(booking);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateBookingAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenBookingDurationIsZero(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        RoomEntity[] rooms = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<RoomEntity>().ValidInstance().SeedDatabaseAsync(db, cancellationToken);
        });
        Guid roomId = Faker.PickRandom(rooms).Id;
        BookingDto booking = new Faker<BookingEntity>().ValidInstance().WithZeroBookingDuration().WithUserId(user.Id).WithRoomId(roomId).Generate().ToDto();
        CreateBookingCommand command = new Faker<CreateBookingCommand>().ValidInstance().WithBooking(booking);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateBookingAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }
}