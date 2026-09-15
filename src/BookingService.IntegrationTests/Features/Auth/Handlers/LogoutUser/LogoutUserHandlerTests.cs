using System.Net;
using BookingService.IntegrationTests.Seeders;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Features.Auth.Handlers.GetUser;
using BookingService.Web.Features.Auth.Handlers.LogoutUser;
using BookingService.IntegrationTests.Features.Auth.Handlers.LoginUser;

namespace BookingService.IntegrationTests.Features.Auth.Handlers.LogoutUser;

[DependsOn<LoginUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class LogoutUserHandlerTests(AppFixture thisApp)
{
    [Test]
    public async ValueTask Handler_ShouldLogoutUser(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);

        //Act
        HttpResponseMessage message = await thisApp.HttpClient.SendLogoutUserAsync(cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        (message, UserDto? userDto) = await thisApp.HttpClient.SendGetUser2Async(cancellationToken);
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized).Or.IsEqualTo(HttpStatusCode.OK);
        await Assert.That(userDto).IsNull().Because(
            $"if '{nameof(message.StatusCode)}' is '{HttpStatusCode.OK}', " +
            $"then user was redirected to the login page, " +
            $"and we do not expect '{nameof(userDto)}' to be parsed."
        );
    }
}