using Microsoft.AspNetCore;
using DotNetEnv;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace LibraryOnlineRentalSystem
{
    public class Program
    // test pipeline
    {
        public static void Main(string[] args)
        {
            // Load environment variables
            DotNetEnv.Env.Load();

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}