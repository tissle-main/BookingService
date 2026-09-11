using Aspire.Hosting.Azure;
using BookingService.AppHost;
using Microsoft.Extensions.Hosting;
using Arshid.Aspire.ApiDocs.Extensions;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);
IResourceBuilder<AzureSqlServerResource> sqlserver = builder.AddAzureSqlServer(AppHostResources.SqlServer);
IResourceBuilder<AzureSqlDatabaseResource> database = sqlserver.AddDatabase(AppHostResources.AppDatabase);
IResourceBuilder<ProjectResource> web = builder.AddProject<Projects.BookingService_Web>(AppHostResources.Web);

if(!builder.Environment.IsProduction())
{
    sqlserver.RunAsContainer();
}
web.WithReference(database).WaitFor(database).WithScalar(true).WithSwagger(true).WithOpenApi(true);
await builder.Build().RunAsync();