using System.IO;
using System.Threading.Tasks;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EventstoreConsumer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                await CreateHostBuilder(args).RunConsoleAsync();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<StartupService>();
                })
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", false);
                    builder.AddJsonFile("appsettings.local.json", true);
                    builder.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    builder.AddEnvironmentVariables("CONSUMER_");
                    builder.AddCommandLine(args);
                })
                .UseSerilog((hostingContext, loggerConfiguration) => 
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext())
                .UseLamar<ConsumerRegistry>();
    }
}
