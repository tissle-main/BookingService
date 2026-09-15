using Bogus;
using System.Net;
using BookingService.Data.Features.Auth.Users;
using BookingService.IntegrationTests.Seeders;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Features.Auth.Handlers.GetUsers;
using BookingService.IntegrationTests.Features.Auth.Handlers.LoginUser;

namespace BookingService.IntegrationTests.Features.Auth.Handlers.GetUsers;

[DependsOn<LoginUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class GetUsersHandlerTests(AppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_ShouldReturnAllUsers_WhenNoIdsProvided(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        UserDto user = await thisApp.LoginAsAdminAsync(cancellationToken);
        IEnumerable<(UserEntity User, string Password)> users = await thisApp.AddUsers2Async(cancellationToken);

        //Act
        (HttpResponseMessage message, UserDto[]? userDtos) = await thisApp.HttpClient.SendGetUsers2Async([], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(userDtos).IsNotNull();
        await Assert.That(userDtos).IsEquivalentTo(users.Select(pair => pair.User).ToDtos().Prepend(user));
    }

    [Test]
    public async ValueTask Handler_ShouldReturnConcreteUsers_WhenIdsProvided(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.LoginAsAdminAsync(cancellationToken);
        IEnumerable<(UserEntity User, string Password)> users = await thisApp.AddUsers2Async(cancellationToken);
        (UserEntity User, string Password)[] usersArray = users.ToArray();
        usersArray = Faker.PickRandom(usersArray, Faker.Random.Number(1, usersArray.Length - 1)).ToArray();
        Guid[] ids = usersArray.Select(pair => pair.User.Id).ToArray();

        //Act
        (HttpResponseMessage message, UserDto[]? userDtos) = await thisApp.HttpClient.SendGetUsers2Async(ids, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(userDtos).IsNotNull();
        await Assert.That(userDtos).IsEquivalentTo(usersArray.Select(pair => pair.User).ToDtos());
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenIdsNotFound(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.LoginAsAdminAsync(cancellationToken);
        IEnumerable<(UserEntity User, string Password)> users = await thisApp.AddUsers2Async(cancellationToken);
        (UserEntity User, string Password)[] usersArray = users.ToArray();
        usersArray = Faker.PickRandom(usersArray, Faker.Random.Number(1, usersArray.Length - 1)).ToArray();
        Guid[] ids = usersArray.Select(pair => Guid.NewGuid()).ToArray();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetUsersAsync(ids, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async ValueTask Handler_ShouldForbid_WhenLoginAsNotAdmin(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);

        //Act
        (HttpResponseMessage message, UserDto[]? users) = await thisApp.HttpClient.SendGetUsers2Async([], cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Forbidden).Or.IsEqualTo(HttpStatusCode.OK);
        await Assert.That(users).IsNull().Because(
            $"if '{nameof(message.StatusCode)}' is '{HttpStatusCode.OK}', " +
            $"then user was redirected to the login page, " +
            $"and we do not expect '{nameof(users)}' to be parsed."
        );
    }
}