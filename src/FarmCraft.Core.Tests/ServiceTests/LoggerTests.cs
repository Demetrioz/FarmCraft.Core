using FarmCraft.Core.Services.Logging;
using FarmCraft.Core.Tests.Config;
using FarmCraft.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using FarmCraft.Core.Data.Context;
using Newtonsoft.Json;

namespace FarmCraft.Core.Tests.ServiceTests
{
    [TestFixture]
    public class LoggerTests
    {
        private IServiceProvider? ServiceProvider { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            TestSettings settings = new TestSettings();
            config.GetSection("TestSettings").Bind(settings);

            ServiceProvider = new ServiceCollection()
                .AddDbContext<IFarmCraftContext, Data.TestContext>(options =>
                    options.UseCosmos(settings.CosmosConnection, settings.CosmosDb))
                .AddSingleton<ILogService, FarmCraftLogService>()
                .BuildServiceProvider();
        }

        [Test]
        public async Task CanCreateContext()
        {
            if (ServiceProvider == null)
                Assert.Fail("Null Service Provider");

            using (IServiceScope scope = ServiceProvider.CreateScope())
            {
                Data.TestContext dbContext = scope.ServiceProvider
                    .GetRequiredService<Data.TestContext>();

                await dbContext.Database.EnsureCreatedAsync();
            }

            Assert.Pass();
        }

        [Test]
        public async Task CanCreateLog()
        {
            if (ServiceProvider == null)
                Assert.Fail("Null Service Provider");

            object testObject = new { MyData = "DummyData" };
            string testObjectString = JsonConvert.SerializeObject(testObject);
            const string message = "Testing a Log";
            const LogLevel level = LogLevel.Debug;

            using (IServiceScope scope = ServiceProvider.CreateScope())
            {
                Data.TestContext dbContext = scope.ServiceProvider
                    .GetRequiredService<Data.TestContext>();

                ILogService logger =
                    scope.ServiceProvider.GetRequiredService<ILogService>();

                await logger.LogAsync(
                    level, 
                    message, 
                    testObject,
                    nameof(LoggerTests)
                );

                FarmCraftLog? log = await dbContext.Logs.FirstOrDefaultAsync();
                Assert.IsNotNull(log);
                Assert.AreEqual(log.Message, message);
                Assert.AreEqual(log.LogLevel, level);
                Assert.AreEqual(log.Source, nameof(LoggerTests));
                Assert.AreEqual(log.Data, testObjectString);

                dbContext.Remove(log);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
