//-----------------------------------------------------------------------
// <copyright file="Startup.cs" company="ThingTrax UK Ltd">
// Copyright (c) ThingTrax Ltd. All rights reserved.
// </copyright>
// <summary>The startup class.</summary>
//-----------------------------------------------------------------------

namespace TT.Core.Api
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;
    using Newtonsoft.Json;
    using Serilog;
    using Serilog.Events;
    using Serilog.Formatting.Elasticsearch;
    using Serilog.Sinks.Elasticsearch;
    using System;
    using System.Globalization;
    using System.Threading;
    using TT.Core.Api.AdSecurity;
    using TT.Core.Api.SecurityModels;
    using TT.Core.Models.Configurations;
    using TT.Core.Models.Constants;
    using TT.Core.Repository;
    using TT.Core.Repository.Sql;
    using TT.Core.Services;
    using TT.Core.Services.Hubs;
    using TT.Core.Services.Interfaces;

    /// <summary>
    /// The startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine(environmentName);
            this.Configuration = configuration;

            var logEnviornmentName = environmentName.ToLower() == "devdocker" ? "LahoreOffice" : environmentName;
            var logIndexer = $"applogs_api_{logEnviornmentName.ToLower()}";
            Console.WriteLine(logEnviornmentName);
            Console.WriteLine(logIndexer);
            var elasticUri = $"{this.Configuration["ElasticSearchSettings:ElasticUri"]}";
            var elasticUser = $"{this.Configuration["ElasticSearchSettings:ElasticUser"]}";
            var elasticPassword = $"{this.Configuration["ElasticSearchSettings:ElasticPassword"]}";
            Log.Logger = new Serilog.LoggerConfiguration()
                 .Enrich.WithProperty("Client", logEnviornmentName)
                 .Enrich.WithProperty("Source", "TT.Core.Api")
                 .Enrich.FromLogContext()
                 .MinimumLevel.Information()
                 .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                 .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                 {
                     MinimumLogEventLevel = LogEventLevel.Debug,
                     IndexDecider = (@event, offset) =>
                     {
                         return logIndexer;
                     },
                     AutoRegisterTemplate = true,
                     AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                     CustomFormatter = new ElasticsearchJsonFormatter(inlineFields: true),
                     CustomDurableFormatter = new ElasticsearchJsonFormatter(inlineFields: true),
                     ModifyConnectionSettings = settings => settings
                .BasicAuthentication(elasticUser, elasticPassword)
                .DisableAutomaticProxyDetection()
                 }).ReadFrom.Configuration(Configuration)
                 .CreateLogger();
        }

        /// <summary>
        /// Gets or sets the client version.
        /// </summary>
        /// <value>
        /// The client version.
        /// </value>
        public static string ClientVersion { get; set; }

        /// <summary>
        /// Gets or sets the production storage connection string.
        /// </summary>
        /// <value>
        /// The production storage connection string.
        /// </value>
        public static string ProductionStorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the API error table.
        /// </summary>
        /// <value>
        /// The name of the API error table.
        /// </value>
        public static string ApiErrorTableName { get; set; }

        /// <summary>
        /// Gets or sets the email settings.
        /// </summary>
        /// <value>
        /// The email settings.
        /// </value>
        public static EmailSettings EmailSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [email errors].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [email errors]; otherwise, <c>false</c>.
        /// </value>
        public static bool EmailErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is downtime included.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is downtime included; otherwise, <c>false</c>.
        /// </value>
        public static bool ISDowntimeIncluded { get; set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddLogging(builder => { builder.AddSerilog(); });

            var clientList = this.Configuration.GetSection("webClient").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder => builder
                    .WithOrigins(clientList)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "ThingTrax Api",
                    Description = "ThingTrax was founded in 2015 to help manufacturers transform the way their labour, processes and machinery connect.",
                    Contact = new OpenApiContact
                    {
                        Name = "ThingTrax",
                        Email = "info@thingtrax.com"
                    }
                });
            });

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddAzureAd(adOptions => this.Configuration.Bind("AzureAd", adOptions))
            .AddCookie(cookieOptions => cookieOptions.SlidingExpiration = true);

            // Register SQL context
            services.AddDbContext<CoreContext>(
                                                opt => opt.UseSqlServer(this.Configuration.GetConnectionString("SqlCore"), o => o.CommandTimeout(180))
                                                          .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking), ServiceLifetime.Transient);

            services.AddDbContext<TelemetryContext>(
                                                    opt => opt.UseSqlServer(this.Configuration.GetConnectionString("Telemetry"), o => o.CommandTimeout(180))
                                                              .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking), ServiceLifetime.Transient);

            services.AddAuthorization(option =>
            {
                option.AddPolicy(RolesConstant.Admin, policy => policy.RequireClaim("Level", RolesConstant.Admin));
                option.AddPolicy(RolesConstant.Supervisor, policy => policy.RequireClaim("Level", RolesConstant.Admin, RolesConstant.Supervisor));
                option.AddPolicy(RolesConstant.Setter, policy => policy.RequireClaim("Level", RolesConstant.Admin, RolesConstant.Supervisor, RolesConstant.Setter));
                option.AddPolicy("CustomAuthorization", policy => policy.Requirements.Add(new CustomAuthorizeRequirement()));
            });

            // Add session services.
            services.AddTransient<IAuthorizationHandler, CustomAuthorizeHandler>();
            bool isAnonymousUser = Convert.ToBoolean(this.Configuration.GetSection("anonymousUser").Value);

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
            });

            services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                if (!isAnonymousUser)
                {
                    config.Filters.Add(new AuthorizeFilter(policy));
                    config.Filters.Add(new RequireHttpsAttribute());
                }

                config.Filters.Add(new GlobalExceptionLogger(
                                                         ProductionStorageConnectionString,
                                                        ApiErrorTableName,
                                                        services.BuildServiceProvider().GetService<IEmailService>(),
                                                        services.BuildServiceProvider().GetService<IMemoryCache>(),
                                                        Log.Logger));
            }).AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // Configurations
            services.AddOptions();

            ClientVersion = this.Configuration.GetSection("ClientVersion").Value;
            if (string.IsNullOrEmpty(ClientVersion))
            {
                // Local dev machine will have a new version everytime.
                ClientVersion = DateTime.Now.ToString("yyyyMMdd.hhmm", CultureInfo.CurrentCulture);
            }

            ProductionStorageConnectionString = this.Configuration.GetSection("productionStorageConnectionString").Value;
            ApiErrorTableName = this.Configuration.GetSection("apiErrorTableName").Value;

            services.Configure<ApplicationSettings>(this.Configuration.GetSection("ApplicationSettings"));
            services.Configure<HealthCheckSettings>(this.Configuration.GetSection("HealthCheckSettings"));
            services.Configure<TokenConfig>(this.Configuration.GetSection("AzureAd"));
            services.Configure<EmailSettings>(this.Configuration.GetSection("EmailSettings"));
            services.Configure<ElasticSearchSettings>(this.Configuration.GetSection("ElasticSearchSettings"));
            services.Configure<EnergySettings>(this.Configuration.GetSection("EnergySettings"));
            services.Configure<JobSettings>(this.Configuration.GetSection("JobSettings"));
            services.Configure<RecognitionSettings>(this.Configuration.GetSection("RecognitionSettings"));
            services.AddSingleton(provider => this.Configuration);
            services.AddHealthChecks();
            bool.TryParse(this.Configuration.GetSection("emailErrors").Value, out bool emailFlag);
            EmailErrors = emailFlag;

            // Register dependencies
            services.AddHttpContextAccessor();
            services.RegisterDependencyForServices();
            services.RegisterDependencyForRepository();

            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, ILoggerFactory loggerFactory)
        {
            lifetime.ApplicationStopping.Register(() =>
            {
                string stack = $"api container {Environment.StackTrace}";
                Console.WriteLine(stack);
                Log.Logger.Information(stack);
                Thread.Sleep(200);
            });

            app.UseDeveloperExceptionPage();
            loggerFactory.AddSerilog();
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //// Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            //// Enable middleware to serve swagger - ui(HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "ThingTrax Api V1");
            });

            var rewriteOptions = new RewriteOptions().AddRedirectToHttps();
            app.UseRewriter(rewriteOptions);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = true
            });
            app.UseWebSockets();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<CoreContext>();
                var telemeryContext = serviceScope.ServiceProvider.GetService<TelemetryContext>();
                Console.WriteLine("Database Started");
                if (!context.AllMigrationsApplied())
                {
                    Console.WriteLine("Database Context");
                    context.Database.Migrate();
                }

                if (!telemeryContext.AllMigrationsApplied())
                {
                    Console.WriteLine("Database tele");
                    telemeryContext.Database.Migrate();
                }

                context.EnsureSeeded();

                Console.WriteLine("Database Created");
            }

            app.UseEndpoints(routes =>
            {
                routes.MapControllers();
                routes.MapHealthChecks("/health");
                routes.MapHub<NotificationHub>("/hub/notification");
                routes.MapHub<AuxiliaryDashboardHub>("/hub/auxiliarydashboard");
                routes.MapHub<SilosDashboardHub>("/hub/silosdashboard");
                routes.MapHub<AuxiliaryEquipmentDashboardHub>("/hub/auxiliaryequipmentdashboard");
                routes.MapHub<DetailOeeDashboardHub>("/hub/detailoeedashboard");
                routes.MapHub<CircleOeeDashboardHub>("/hub/circleoeedashboard");
                routes.MapHub<AssemblyDashboardHub>("/hub/assemblyDashboard");
                routes.MapHub<DetailAssemblyDashboardHub>("/hub/detailAssemblyDashboard");
                routes.MapHub<OeeGridDashboardHub>("/hub/oeegriddashboard");
                routes.MapHub<SilosEquipmentDashboardHub>("/hub/silosequipmentdashboard");
                routes.MapHub<LiveLabourDashboardHub>("/hub/livelabourdashboard");
                routes.MapHub<ActiveNotificationHub>("/hub/activenotification");
                routes.MapHub<KpiDashboardHub>("/hub/kpidashboard");
                routes.MapHub<OeeEquipmentDashboardHub>("/hub/oeeequipmentdashboard");
                routes.MapHub<EnergyDashboardHub>("/hub/energydashboard");
            });
        }
    }
}
