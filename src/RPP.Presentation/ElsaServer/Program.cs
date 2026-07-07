using Elsa.Extensions;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.MongoDb.Extensions;
using Elsa.Persistence.MongoDb.Modules.Management;
using Elsa.Persistence.MongoDb.Modules.Runtime;
using Elsa.Workflows.Runtime.Distributed.Extensions;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Mvc;
using RPP.Application.BusinessEngine;
using RPP.Application.Interfaces;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

var services = builder.Services;
var configuration = builder.Configuration;

var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb")
    ?? throw new InvalidOperationException("Connection string 'MongoDb' not found.");

var rabbitConnectionString = builder.Configuration.GetConnectionString("RabbitMq")
    ?? "amqp://guest:guest@localhost:5672/";

var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
    ?? "localhost:6379";

#region Distributed Clustering Configuration
//var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
#endregion

builder.Services.AddHangfire(config =>
{
    config.UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseMongoStorage(mongoConnectionString, "ElsaHangfireDB", new MongoStorageOptions
          {
              MigrationOptions = new MongoMigrationOptions
              {
                  MigrationStrategy = new MigrateMongoMigrationStrategy(),
                  BackupStrategy = new CollectionMongoBackupStrategy()
              },
              Prefix = "hangfire",
              CheckConnection = true,
              //Enables instant job handling on standalone local MongoDB
              CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
          });
});
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 1;
});

services
    .AddElsa(elsa => elsa
        .UseIdentity(identity =>
        {
            identity.TokenOptions = options => options.SigningKey = "large-signing-key-for-signing-JWT-tokens";
            identity.UseAdminUserProvider();
        })
        .UseDefaultAuthentication()
        .UseMongoDb(mongoConnectionString)
        .UseWorkflowManagement(management => management.UseMongoDb())// Configure workflow management (definitions, instances)
        .UseWorkflowRuntime(runtime => runtime.UseMongoDb())// Configure workflow runtime (bookmarks, inbox, execution logs)
    #region Distributed Clustering Configuration
        ////DISTRIBUTED RUNTIME & LOCKING (Ensures only 1 node ever resumes a specific instance!)
        //.UseWorkflowRuntime(runtime =>
        //{
        //    runtime.UseMongoDb();
        //    runtime.UseDistributedRuntime();
        //    runtime.DistributedLockProvider = _ => new RedisDistributedSynchronizationProvider(redisConnection.GetDatabase());
        //    runtime.UseMassTransitDispatcher();
        //})
        ////DISTRIBUTED CACHE SYNC (Clears RAM caches across all 3 nodes on updates)
        //.UseDistributedCache(distributedCaching =>
        //{
        //    distributedCaching.UseMassTransit();
        //})
        ////MASSTRANSIT & RABBITMQ(Handles clustered messaging & worker execution dispatch)
        //.UseMassTransit(massTransit =>
        //{
        //    massTransit.UseRabbitMq(rabbitConnectionString, rabbit =>
        //    {
        //        rabbit.ConfigureTransportBus = (context, bus) =>
        //        {
        //            bus.PrefetchCount = 1;
        //            bus.ConcurrentMessageLimit = 1;
        //        };
        //    });
        //})
    #endregion
        .UseScheduling(scheduling => scheduling.UseHangfireScheduler())


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

//UseHttpsRedirection must be disabled to allow Docker HTTP-only network
app.UseHttpsRedirection();// diable this for local distributed environment

app.UseHangfireDashboard();//http://localhost:{StudioPort}/hangfire

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