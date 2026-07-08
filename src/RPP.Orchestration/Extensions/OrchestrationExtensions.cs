using Elsa.Extensions;
using Elsa.Persistence.MongoDb.Extensions;
using Elsa.Persistence.MongoDb.Modules.Management;
using Elsa.Persistence.MongoDb.Modules.Runtime;
using Elsa.Workflows.Runtime.Distributed.Extensions;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RPP.Core.BusinessEngine;
using RPP.Core.Interfaces;
using RPP.Orchestration.AssemblyMarker;
using StackExchange.Redis;

namespace RPP.Orchestration.Extensions
{
    public static class OrchestrationExtensions
    {
        //Services Configuration Dependency Injection
        public static IServiceCollection AddOrchestration(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnectionString = configuration.GetConnectionString("MongoDb")
                ?? throw new InvalidOperationException("Connection string 'MongoDb' not found.");

            var rabbitConnectionString = configuration.GetConnectionString("RabbitMq")
                ?? "amqp://guest:guest@localhost:5672/";

            var redisConnectionString = configuration.GetConnectionString("Redis")
                ?? "localhost:6379";

            var hangfireDBName = configuration.GetValue<string>("HangfireDBName") ?? "ElsaHangfireDB";

            #region Distributed Clustering Configuration
            //var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
            #endregion

            services
                .AddHangfire(config =>
                {
                    config.UseSimpleAssemblyNameTypeSerializer()
                          .UseRecommendedSerializerSettings()
                          .UseMongoStorage(mongoConnectionString, hangfireDBName, new MongoStorageOptions
                          {
                              MigrationOptions = new MongoMigrationOptions
                              {
                                  MigrationStrategy = new MigrateMongoMigrationStrategy(),
                                  BackupStrategy = new CollectionMongoBackupStrategy()
                              },
                              Prefix = "hangfire",
                              CheckConnection = true,
                              CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                          });
                })
                .AddHangfireServer();

            services
                .AddElsa(elsa => elsa
                    .UseIdentity(identity =>
                    {
                        identity.TokenOptions = options => options.SigningKey = "large-signing-key-for-signing-JWT-tokens";
                        identity.UseAdminUserProvider();
                    })
                    .UseDefaultAuthentication()
                    .UseMongoDb(mongoConnectionString)
                    .UseWorkflowManagement(management => management.UseMongoDb())
                    .UseWorkflowRuntime(runtime => runtime.UseMongoDb())
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
                    .UseCSharp()
                    .UseHttp(http => http.ConfigureHttpOptions = options => configuration.GetSection("Http").Bind(options))
                    .UseWorkflowsApi()
                    .AddActivitiesFrom<IOrchestrationAssemblyMarker>()
                    .AddWorkflowsFrom<IOrchestrationAssemblyMarker>()
                );

            services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("*")));
            services.AddRazorPages(options => options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));

            // Register business engine
            services.AddScoped<IEnrichmentService, EnrichmentService>();
            services.AddScoped<IPreEnrichmentService, PreEnrichmentService>();
            services.AddScoped<IShiftBaselineGenerationService, ShiftBaselineGenerationService>();
            services.AddScoped<IAssetAllocationService, AssetAllocationService>();
            services.AddScoped<IShiftOutputBuilderService, ShiftOutputBuilderService>();

            return services;
        }

        //Elsa and Hangfire Middleware Pipeline
        public static IApplicationBuilder UseOrchestration(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
            app.UseWorkflowsApi();
            app.UseWorkflows();

            return app;
        }
    }
}
