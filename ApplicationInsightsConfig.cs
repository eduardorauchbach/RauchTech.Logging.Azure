using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RauchTech.Logging.Azure
{
    public static class ApplicationInsightsConfig
    {
        public static IServiceCollection ConfigureAzureLogging(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"] ?? null;

            if (connectionString != null)
            {
                var applicationName = configuration["ApplicationName"] ?? "Anonymous";

                var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
                var telemetryInitializer = new UserIdTelemetryInitializer(applicationName);

                telemetryConfiguration.TelemetryInitializers.Add(telemetryInitializer);

                services.AddLogging(builder =>
                {
                    builder.AddFilter("Microsoft", LogLevel.Warning)
                           .AddFilter("System", LogLevel.Warning)
                           .AddFilter("Quartz", LogLevel.Warning)
                           .AddFilter("Host.Startup", LogLevel.Warning)
                           .AddFilter("Host.General", LogLevel.Warning)
                           .AddFilter("Host.Triggers.Warmup", LogLevel.Warning)
                           .AddFilter("Host.Results", LogLevel.Warning)
                           .AddFilter("Host.Aggregator", LogLevel.Warning)
                           .AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

                    builder.Services.AddSingleton<ITelemetryInitializer>(telemetryInitializer);
                    builder.Services.AddSingleton(telemetryConfiguration);

                    // Setting up Application Insights with the connection string from configuration
                    builder.AddApplicationInsights(config => config.ConnectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"],
                            configureApplicationInsightsLoggerOptions: (options) => { });
                });
            }

            return services;
        }

        #region Telemetry Helper

        private class UserIdTelemetryInitializer : ITelemetryInitializer
        {
            private readonly string _userId;

            public UserIdTelemetryInitializer(string userId)
            {
                _userId = userId;
            }

            public void Initialize(ITelemetry telemetry)
            {
                telemetry.Context.User.Id = _userId;
            }
        }

        #endregion
    }
}
