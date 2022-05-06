using Akka.Actor;
using FarmCraft.Core.Actors;
using FarmCraft.Core.Messages;
using FarmCraft.Core.Messages.Telemetry;
using FarmCraft.Core.Services.Logging;
using FarmCraft.Core.Services.Messaging;
using FarmCraft.Core.Services.Messaging.Publisher;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FarmCraft.Core.Tests.ActorTests.Actors
{
    public class PublisherActor : FarmCraftActor
    {
        private IServiceScope _scope { get; set; }
        private ServiceBusPublisher _publisher { get; set; }

        public PublisherActor(IServiceProvider provider) : base(provider)
        {
            _scope = provider.CreateScope();

            MessageBusService service = _scope.ServiceProvider
                    .GetRequiredService<MessageBusService>();

            _publisher = new ServiceBusPublisher(
                service,
                "poc_alerts",
                new FarmCraftLogService(provider)
            );

            Receive<AskToPublishTelemetry>(message => PublishTelemetry(message));
        }

        protected override void PostStop()
        {
            _publisher.Dispose();
            _scope.Dispose();

            base.PostStop();
        }

        private async Task PublishTelemetry(AskToPublishTelemetry message)
        {
            IActorRef sender = Sender;
            
            await _publisher.PublishTelemetry(message.Telemetry);
            sender.Tell(ActorResponse.Success(Guid.NewGuid().ToString(), true));
        }
    }
}
