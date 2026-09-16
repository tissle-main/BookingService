using Bogus;
using System.Net;
using Microsoft.EntityFrameworkCore;
using BookingService.Web.Features.Auth;
using BookingService.IntegrationTests.Seeders;
using BookingService.Data.Features.Auth.Users;
using BookingService.Data.Features.Auth.Roles;
using BookingService.Web.Features.Auth.Handlers.RegisterUser;

namespace BookingService.IntegrationTests.Features.Auth.Handlers.RegisterUser;

[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class RegisterUserHandlerTests(AppFixture thisApp)
{
    [Test]
    public async ValueTask Handler_ShouldCreateUser(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync(cancellationToken);
        });
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            UserEntity? user = await db.Users.AsNoTracking().SingleOrDefaultAsync(cancellationToken);
            await Assert.That(user).IsNotNull();
            await Assert.That(user.Email).IsEqualTo(command.Email);
        });
    }

    [Test]
    public async ValueTask Handler_ShouldSetUserRole(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync(cancellationToken);
        });
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            UserEntity? user = await db.Users.AsNoTracking().SingleOrDefaultAsync(cancellationToken);
            await Assert.That(user).IsNotNull();

            RoleEntity? role = await db.Roles.AsNoTracking().SingleOrDefaultAsync(e => e.Name == AuthRoles.User, cancellationToken);
            await Assert.That(user).IsNotNull();

            bool roleAssigned = await db.UserRoles.AnyAsync(e => e.UserId == user!.Id && e.RoleId == role!.Id, cancellationToken);
            await Assert.That(roleAssigned).IsTrue();
        });
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUserExists(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        RegisterUserCommand command = await thisApp.AddUsersAndPickRandomAsync(cancellationToken);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenEmailIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithInvalidEmail().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordTooShord(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithTooShortPassword().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainUppercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutUppercaseLetters().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainLowercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutLowercaseLetters().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainDigits(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutDigits().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }
}