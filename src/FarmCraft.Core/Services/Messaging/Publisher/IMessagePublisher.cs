using FarmCraft.Core.Messages;

namespace FarmCraft.Core.Services.Messaging.Publisher
{
    /// <summary>
    /// An interface for publishing messages to a message broker
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publishes a single message to a message broker
        /// </summary>
        /// <param name="message">The message to publish</param>
        /// <returns></returns>
        Task PublishMessage(FarmCraftMessage message);

        /// <summary>
        /// Publishes messages to a message broker in batches
        /// </summary>
        /// <param name="messages">The messages to publish</param>
        /// <param name="batchSize">The size of batches to publish</param>
        /// <returns></returns>
        Task PublishMessageBatch(List<IMessage> messages, int batchSize);
    }
}
