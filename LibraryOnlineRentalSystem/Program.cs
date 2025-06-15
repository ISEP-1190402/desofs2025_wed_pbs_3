using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using DotNetEnv;

namespace LibraryOnlineRentalSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Load environment variables from .env file if it exists
                var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
                if (File.Exists(envPath))
                {
                    DotNetEnv.Env.Load(envPath);
                }

                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application start-up failed: {ex.Message}");
                throw;
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();

                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        logging.AddFilter("Microsoft", LogLevel.Information);
                        logging.AddFilter("System", LogLevel.Warning);
                        logging.AddFilter("LibraryOnlineRentalSystem", LogLevel.Debug);
                    }
                    else
                    {
                        logging.AddFilter("Microsoft", LogLevel.Warning);
                        logging.AddFilter("System", LogLevel.Warning);
                        logging.AddFilter("LibraryOnlineRentalSystem", LogLevel.Information);
                    }
                })
                .UseStartup<Startup>();
    }
}