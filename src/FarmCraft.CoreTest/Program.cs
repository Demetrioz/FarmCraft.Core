using FarmCraft.CoreTest;
using FarmCraft.CoreTest.Config;

public class Program
{
    private static IConfiguration Configuration { get; set; }

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                Configuration = config
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{env}.json")
                    .Build();
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<TestSettings>(Configuration.GetSection("TestSettings"));

                services.AddHostedService<TestCore>();
            });
}
