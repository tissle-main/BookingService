using ErrorOr;
using Mediator;
using FluentValidation;
using BookingService.Data;
using System.Collections.Frozen;
using BookingService.Web.Features;
using Microsoft.EntityFrameworkCore;
using BookingService.Web.Components;
using BookingService.ServiceDefaults;
using BookingService.Web.Shared.DbSeeding;
using BookingService.Web.Shared.Behaviors.Validation;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web;

public static class DIContainer
{
    private static FrozenSet<FeatureProvider> Features { get; set; } = [];

    extension(WebApplicationBuilder thisBuilder)
    {
        public void AddCore()
        {
            thisBuilder.AddSqlServerDbContext<AppDbContext>(AppHostResources.AppDatabase);
            thisBuilder.Services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = static void (ProblemDetailsContext ctx) =>
                {
                    ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
                };
            });
            thisBuilder.Configuration.AddUserSecrets<Program>();
            thisBuilder.AddBlazor();
            thisBuilder.AddCQRS();
            thisBuilder.AddFeatures();
        }
        public void AddBlazor()
        {
            thisBuilder.Services.AddRazorComponents().AddInteractiveServerComponents();
        }
        public void AddCQRS()
        {
            thisBuilder.Services.AddMediator(options =>
            {
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.PipelineBehaviors = [
                    typeof(ValidationBehavior<,>),
                    typeof(AuthorizedBehavior<,>),
                    typeof(DbTransactionBehavior<,>)
                ];
            });
            thisBuilder.Services.AddValidatorsFromAssemblyContaining(typeof(DIContainer), ServiceLifetime.Singleton);
            ValidatorOptions.Global.LanguageManager.Enabled = false;
        }
        public void AddFeatures()
        {
            Features = typeof(DIContainer).Assembly.GetTypes().Where(type =>
            {
                return !type.IsAbstract && type.IsAssignableTo(typeof(FeatureProvider));
            }).Select(type =>
            {
                return (FeatureProvider)Activator.CreateInstance(type)!;
            }).ToFrozenSet();
            foreach(FeatureProvider provider in Features)
            {
                provider.AddServices(thisBuilder);
            }
        }
    }
    extension(WebApplication thisApp)
    {
        public void UseCore()
        {
            thisApp.MigrateDatabase();
            thisApp.SeedDatabase();
            thisApp.UseBlazor();
            thisApp.UseFeatures();
        }
        public void MigrateDatabase()
        {
            using IServiceScope scope = thisApp.Services.CreateScope();
            AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }
        public void SeedDatabase()
        {
            using IServiceScope scope = thisApp.Services.CreateScope();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            ErrorOr<Unit> errorOrValue = mediator.Send(new DbSeedingCommand(), CancellationToken.None).AsTask().GetAwaiter().GetResult();
            if(errorOrValue.IsError)
            {
                string message = string.Join(", ", errorOrValue.Errors);
                throw new Exception($"Unexpected error occured while seeding database: {message}");
            }
        }
        public void UseBlazor()
        {
            if(!thisApp.Environment.IsDevelopment())
            {
                thisApp.UseExceptionHandler("/Error", createScopeForErrors: true);
                thisApp.UseHsts();
            }
            thisApp.UseHttpsRedirection();
            thisApp.UseAntiforgery();
            thisApp.MapStaticAssets();
            thisApp.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        }
        public void UseFeatures()
        {
            foreach(FeatureProvider provider in Features)
            {
                provider.UseMiddleware(thisApp);
            }
        }
    }
}