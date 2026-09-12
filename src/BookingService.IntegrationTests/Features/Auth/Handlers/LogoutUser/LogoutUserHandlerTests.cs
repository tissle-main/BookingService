using System.Net;
using BookingService.Data.Features.Auth.Users;
using BookingService.IntegrationTests.Seeders;
using BookingService.Web.Features.Auth.Handlers.LogoutUser;
using BookingService.Web.Features.Auth.Handlers.CheckLoggedInUser;
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
        (UserEntity user, _) = await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);

        //Act
        HttpResponseMessage message = await thisApp.HttpClient.SendLogoutUserAsync(cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        message.Dispose();

        message = await thisApp.HttpClient.SendCheckLoggedInUserAsync(user.Email!, cancellationToken);
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
        message.Dispose();
    }
}