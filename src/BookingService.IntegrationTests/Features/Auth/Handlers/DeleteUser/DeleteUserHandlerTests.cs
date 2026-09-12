using System.Net;
using BookingService.Data.Features.Auth.Users;
using BookingService.IntegrationTests.Seeders;
using BookingService.Web.Features.Auth.Handlers.DeleteUser;
using BookingService.IntegrationTests.Features.Auth.Handlers.LoginUser;
using Microsoft.EntityFrameworkCore;
using BookingService.Web.Features.Auth.Handlers.CheckLoggedInUser;

namespace BookingService.IntegrationTests.Features.Auth.Handlers.DeleteUser;

[DependsOn<LoginUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class DeleteUserHandlerTests(AppFixture thisApp)
{
    [Test]
    public async ValueTask Handler_ShouldDeleteUser(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteUserAsync(cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool userExists = await db.Users.AnyAsync(e => e.Id == user.Id, cancellationToken);
            await Assert.That(userExists).IsFalse();
        });
    }

    [Test]
    [DependsOn(nameof(Handler_ShouldDeleteUser))]
    public async ValueTask Handler_ShouldLogoutUser(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetDatabaseAsync(cancellationToken);
        await thisApp.SeedDatabaseAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);

        //Act
        HttpResponseMessage message = await thisApp.HttpClient.SendDeleteUserAsync(cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        message.Dispose();

        message = await thisApp.HttpClient.SendCheckLoggedInUserAsync(user.Email!, cancellationToken);
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
        message.Dispose();
    }
}