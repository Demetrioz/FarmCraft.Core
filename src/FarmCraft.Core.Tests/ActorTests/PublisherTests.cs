using Akka.Actor;
using Akka.TestKit.NUnit;
using FarmCraft.Core.Data.Entities;
using FarmCraft.Core.Messages;
using FarmCraft.Core.Messages.Telemetry;
using FarmCraft.Core.Services.Messaging;
using FarmCraft.Core.Tests.ActorTests.Actors;
using FarmCraft.Core.Tests.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;

namespace FarmCraft.Core.Tests.ActorTests
{
    //https://petabridge.com/blog/how-to-unit-test-akkadotnet-actors-akka-testkit/
    [TestFixture]
    public class PublisherTests : TestKit
    {
        // https://github.com/akkadotnet/akka.net/issues/2130
        private TimeSpan EpsilonValueForWithins => new TimeSpan(0, 0, 1);
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
                 .Configure<MessageBusOptions>(options =>
                 {
                     options.Host = settings.ServiceBusHost;
                     options.Queue = settings.ServiceBusQueue;
                 })
                .AddSingleton<MessageBusService>()
                .BuildServiceProvider();
        }

        [Test]
        public void PublishersCanPublishTelemetry()
        {
            if (ServiceProvider == null)
                Assert.Fail("Null Service Provider");

            try
            {
                IActorRef publisher = Sys.ActorOf(
                    Props.Create(() => new PublisherActor(ServiceProvider)));
                publisher.Tell(
                    new AskToPublishTelemetry(
                        new FarmCraftTelemetry
                        {
                            DeviceId = Guid.NewGuid().ToString(),
                            Level = TelemetryLevel.Info,
                            Timestamp = DateTimeOffset.UtcNow,
                            Temperature = 43.4,
                            Humidity = 43.4,
                        }
                    )
                );

                FarmCraftActorResponse response = ExpectMsg<FarmCraftActorResponse>(
                    TimeSpan.FromSeconds(5));
                
                Assert.True(response != null);

                bool result = (bool)response.Data;

                Assert.True(result);

                publisher.Tell(PoisonPill.Instance);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
