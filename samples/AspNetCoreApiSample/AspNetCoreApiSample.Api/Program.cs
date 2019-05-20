using System;
using System.Threading.Tasks;
using AspNetCoreApiSample.DomainModel;
using AspNetCoreApiSample.Repositories;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreApiSample.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();

            using (var serviceScope = webHost.Services.CreateScope())
            {
                var appDbContext = serviceScope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                appDbContext.Contracts.Add(
                    new Contract(1, "customer@email")
                );

                await appDbContext.SaveChangesAsync();
            }

            webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
