using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace LibraryOnlineRentalSystem
{
    public class Program
    // test pipeline
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}