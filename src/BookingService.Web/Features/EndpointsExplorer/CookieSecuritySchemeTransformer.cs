using Microsoft.OpenApi;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BookingService.Web.Features.EndpointsExplorer;

public sealed class CookieSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    #region Interfaces
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        IEnumerable<AuthenticationScheme> schemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if(!schemes.Any(x => x.Name == CookieAuthenticationDefaults.AuthenticationScheme))
        {
            return;
        }
        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[CookieAuthenticationDefaults.AuthenticationScheme] = new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Cookie,
            Name = ".AspNetCore.Identity.Application"
        };
    }
    #endregion
}
