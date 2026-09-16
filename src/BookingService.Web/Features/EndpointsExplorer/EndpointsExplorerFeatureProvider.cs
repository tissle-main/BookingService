using Scalar.AspNetCore;

namespace BookingService.Web.Features.EndpointsExplorer;

/// <summary>Registers OpenAPI endpoint exploration support.</summary>
public sealed class EndpointsExplorerFeatureProvider : FeatureProvider
{
    #region Base
    public override void AddServices(WebApplicationBuilder builder)
    {
        if(builder.Environment.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<CookieSecuritySchemeTransformer>();
                options.AddOperationTransformer<CookieSecurityOperationTransformer>();
            });
        }
    }
    public override void UseMiddleware(WebApplication app)
    {
        if(app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "BookingService.Web");
            });
        }
    }
    #endregion
}