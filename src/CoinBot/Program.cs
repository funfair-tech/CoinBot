using System.Threading.Tasks;

namespace CoinBot;

internal static class Program
{
    private static Task Main()
    {
        Startup startup = new();

        return startup.StartAsync();
    }
}