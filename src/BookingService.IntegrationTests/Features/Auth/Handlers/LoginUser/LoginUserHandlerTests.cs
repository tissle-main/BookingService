using Bogus;
using System.Net;
using BookingService.Data.Features.Auth.Users;
using BookingService.IntegrationTests.Seeders;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Features.Auth.Handlers.GetUser;
using BookingService.Web.Features.Auth.Handlers.LoginUser;
using BookingService.Web.Features.Auth.Handlers.RegisterUser;
using BookingService.IntegrationTests.Features.Auth.Handlers.RegisterUser;

namespace BookingService.IntegrationTests.Features.Auth.Handlers.LoginUser;

[DependsOn<RegisterUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class LoginUserHandlerTests(AppFixture thisApp)
{
    [Test]
    public async ValueTask Handler_ShouldReturnUserWithRequestedEmail(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        (UserEntity user, string password) = await thisApp.AddUsers2AndPickRandomAsync(cancellationToken);
        LoginUserCommand command = new(user.Email!, password);

        //Act
        (HttpResponseMessage message, LoginUserResponse? response) = await thisApp.HttpClient.SendLoginUser2Async(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response).IsNotNull();
        await Assert.That(response.User.Email).IsEqualTo(command.Email);
        message.Dispose();
    }

    [Test]
    public async ValueTask Handler_ShouldLoginUser(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        (UserEntity user, string password) = await thisApp.AddUsers2AndPickRandomAsync(cancellationToken);
        LoginUserCommand command = new(user.Email!, password);

        //Act
        (HttpResponseMessage message, LoginUserResponse? response) = await thisApp.HttpClient.SendLoginUser2Async(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response).IsNotNull();
        message.Dispose();

        (message, UserDto? userDto) = await thisApp.HttpClient.SendGetUser2Async(cancellationToken);
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(userDto).IsNotNull();
        await Assert.That(userDto).IsEquivalentTo(user.ToDto());
        message.Dispose();
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUserNotFound(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        (_, string password) = await thisApp.AddUsers2AndPickRandomAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPassword(password);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandomAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithEmail(user.Email!);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenEmailIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithInvalidEmail().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordTooShord(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithTooShortPassword().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainUppercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutUppercaseLetters().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainLowercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutLowercaseLetters().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainDigits(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutDigits().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }
}