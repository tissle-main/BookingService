using BookingService.Web.Shared.Extensions;
using BookingService.Web.Features.Auth.Dtos.RefreshTokens;
using BookingService.Web.Features.Auth.Handlers.RefreshAccessToken;

namespace BookingService.Web.Features.Auth.Extensions;

public static class RefreshTokenExtensions
{
    public const string RefreshTokenKey = "refresh-token";

    extension(HttpContext thisHttpContext)
    {
        public void AddRefreshToken(RefreshTokenDto refreshToken)
        {
            thisHttpContext.Response.Cookies.Append(RefreshTokenKey, refreshToken.Value, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshToken.ExpiresAt,
                Path = RefreshAccessTokenEndpoint.Url
            });
        }
        public string? GetRefreshToken()
        {
            return thisHttpContext.Request.Cookies[RefreshTokenKey];
        }
    }
    extension(HttpResponseMessage thisHttpResponse)
    {
        public string? GetRefreshToken()
        {
            string? refreshToken = thisHttpResponse.GetCookie(RefreshTokenKey);
            if(refreshToken is not null)
            {
                return Uri.UnescapeDataString(refreshToken);
            }
            return refreshToken;
        }
    }
}