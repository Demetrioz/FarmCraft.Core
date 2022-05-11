using FarmCraft.Core.Messages;

namespace FarmCraft.Core.Services.Messaging.Publisher
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        public Task PublishMessage(FarmCraftMessage message)
        {
            throw new NotImplementedException();
        }

        public Task PublishMessageBatch(List<IMessage> messages, int batchSize)
        {
            throw new NotImplementedException();
        }
    }
}
