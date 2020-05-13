using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace CoinBot
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Startup startup = new Startup();

            using (var host = CreateHost(args: args, startup: startup))
            {
                Startup.Start(host.Services);

                await host.RunAsync();
            }
        }

        private static IHost CreateHost(string[] args, Startup startup)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureServices(startup.ConfigureServices)
                       .UseWindowsService()
                       .UseSystemd()
                       .Build();
        }
    }
}