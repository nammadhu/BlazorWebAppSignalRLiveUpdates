using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorWebAppSignalR.Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            await builder.Build().RunAsync();
        }
    }
}