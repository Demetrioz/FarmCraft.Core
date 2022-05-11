using FarmCraft.Core.Messages;
using FarmCraft.Core.Services.Messaging.Handler;

namespace FarmCraft.Core.Services.Messaging.Consumer
{
    public class RabbitMQConsumer : IMessageConsumer
    {
        public Task BeginProcessing()
        {
            throw new NotImplementedException();
        }

        public void Register<T, H>()
            where T : IMessage
            where H : IMessageHandler
        {
            throw new NotImplementedException();
        }

        public void Register<T>(Action<FarmCraftMessage> handler) where T : IMessage
        {
            throw new NotImplementedException();
        }
    }
}
