using Microsoft.AspNetCore.Mvc;
using Elsa.Extensions;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;
using Elsa.Persistence.MongoDb;
using Elsa.Persistence.MongoDb.Extensions;
using Elsa.Persistence.MongoDb.Modules.Management;
using Elsa.Persistence.MongoDb.Modules.Runtime;
using Elsa.Studio.Shell;
using Elsa.Workflows;
using Elsa.Workflows.Runtime.Distributed.Extensions;
using Medallion.Threading.SqlServer;
using RPP.Application.Interfaces;
using RPP.Application.BusinessEngine;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

var services = builder.Services;
var configuration = builder.Configuration;
//var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=ElsaQuartzLocal;Integrated Security=True;MultipleActiveResultSets=True;Encrypt=False;TrustServerCertificate=True;";

var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb")
    ?? throw new InvalidOperationException("Connection string 'MongoDb' not found.");

var sqlConnectionString = builder.Configuration.GetConnectionString("SqlServer")
    ?? "Data Source=(localdb)\\MSSQLLocalDB;Database=ElsaQuartzLocal;Integrated Security=True;MultipleActiveResultSets=True;Encrypt=False;TrustServerCertificate=True;";

var rabbitConnectionString = builder.Configuration.GetConnectionString("RabbitMq")
    ?? "amqp://guest:guest@localhost:5672/";

services
    .AddElsa(elsa => elsa
        .UseIdentity(identity =>
        {
            identity.TokenOptions = options => options.SigningKey = "large-signing-key-for-signing-JWT-tokens";
            identity.UseAdminUserProvider();
        })
        .UseDefaultAuthentication()
        //.UseWorkflowRuntime(runtime => runtime.UseDistributedRuntime()) // Enable distributed runtime
        //.UseScheduling(scheduling => scheduling.UseQuartzScheduler()) // Comment out to Disable Quartz scheduling
        //.UseQuartz(quartz => quartz.UseSqlServer(connectionString)) // Comment out to Disable Quartz scheduling
        .UseMongoDb(mongoConnectionString)
        .UseWorkflowManagement(management => management.UseMongoDb())// Configure workflow management (definitions, instances)
        .UseWorkflowRuntime(runtime => runtime.UseMongoDb())// Configure workflow runtime (bookmarks, inbox, execution logs)
        //.UseWorkflowRuntime(runtime =>
        //{
        //    runtime.UseMongoDb(); // Keep state in Mongo

        //    // Enable Distributed Runtime
        //    runtime.UseDistributedRuntime();

        //    // Use SQL Server for Distributed Locking (prevents duplicate processing!)
        //    runtime.DistributedLockProvider = _ => new SqlDistributedSynchronizationProvider(sqlConnectionString);
        //})
        //.UseDistributedCache(distributedCaching =>
        //{
        //    distributedCaching.UseMassTransit();
        //})
        //.UseMassTransit(massTransit =>
        //{
        //    // Connect to RabbitMQ container
        //    massTransit.UseRabbitMq(rabbitConnectionString, rabbit =>
        //    {
        //        // Tune transport parameters for clustered performance
        //        rabbit.ConfigureTransportBus = (context, bus) =>
        //        {
        //            bus.PrefetchCount = 50;
        //            bus.Durable = true;
        //            bus.AutoDelete = false;
        //            bus.ConcurrentMessageLimit = 32;
        //        };
        //    });
        //})
        //.UseScheduling(scheduling => scheduling.UseQuartzScheduler())
        //.UseQuartz(quartz => quartz.UseSqlServer(sqlConnectionString))
        .UseJavaScript()
        .UseLiquid()
        .UseCSharp()
        .UseHttp(http => http.ConfigureHttpOptions = options => configuration.GetSection("Http").Bind(options))
        .UseWorkflowsApi()
        // Tells Elsa to scan the RPP.Orchestration Activities folder assembly
        .AddActivitiesFrom<RPP.Orchestration.AssemblyMarker.IOrchestrationAssemblyMarker>()
        // Tells Elsa to scan the RPP.Orchestration Workflows folder assembly
        .AddWorkflowsFrom<RPP.Orchestration.AssemblyMarker.IOrchestrationAssemblyMarker>()
    );
services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("*")));
services.AddRazorPages(options => options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));

// Register business engine
builder.Services.AddScoped<IEnrichmentService, EnrichmentService>();
builder.Services.AddScoped<IPreEnrichmentService, PreEnrichmentService>();
builder.Services.AddScoped<IShiftBaselineGenerationService, ShiftBaselineGenerationService>();
builder.Services.AddScoped<IAssetAllocationService, AssetAllocationService>();
builder.Services.AddScoped<IShiftOutputBuilderService, ShiftOutputBuilderService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseRouting();
app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.MapFallbackToPage("/_Host");

app.Run();