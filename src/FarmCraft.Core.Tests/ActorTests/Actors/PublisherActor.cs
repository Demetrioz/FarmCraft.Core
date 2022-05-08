using Akka.Actor;
using FarmCraft.Core.Data.Context;
using FarmCraft.Core.Messages;
using FarmCraft.Core.Messages.Telemetry;
using FarmCraft.Core.Services.Logging;
using FarmCraft.Core.Services.Messaging;
using FarmCraft.Core.Services.Messaging.Publisher;
using System;
using System.Threading.Tasks;

namespace FarmCraft.Core.Tests.ActorTests.Actors
{
    public class PublisherActor : ReceiveActor
    {
        private ServiceBusPublisher _publisher { get; set; }

        public PublisherActor(MessageBusService service, IFarmCraftContext dbContext)
        {
            _publisher = new ServiceBusPublisher(
                service,
                "poc_alerts",
                new FarmCraftLogService(dbContext)
            );

            Receive<AskToPublishTelemetry>(message => PublishTelemetry(message));
        }

        protected override void PostStop()
        {
            _publisher.Dispose();
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
