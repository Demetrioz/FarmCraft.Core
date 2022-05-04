using Azure.Messaging.ServiceBus;
using FarmCraft.Core.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FarmCraft.Core.Services.Messaging.Publisher
{
    /// <summary>
    /// A Message Publisher for interacting with Azure Service Bus
    /// </summary>
    public class ServiceBusPublisher : IMessagePublisher, IDisposable
    {
        private readonly ILogger _logger;
        private static ServiceBusSender? _sender;

        /// <summary>
        /// Creates a singular ServiceBusClient and ServiceBusSender if they haven't
        /// arleady been created
        /// </summary>
        /// <param name="options">Options for connecting to the Service Bus and Queue</param>
        /// <param name="logger">A generic logger</param>
        public ServiceBusPublisher(
            MessageBusService service, 
            IOptions<PublisherOptions> options, 
            ILogger logger
        )
        {
            if (service == null)
                throw new Exception("MessageBusService missing");

            _logger = logger;

            PublisherOptions publisherOptions = options.Value;

            if(_sender == null)
                _sender = service.CreatePublisher(publisherOptions.Queue);
        }

        /// <summary>
        /// Disposes of the client and processor objects prior to being
        /// destroyed
        /// </summary>
        public void Dispose()
        {
            if (_sender != null)
                _sender.DisposeAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Publishes a single message to the message queue
        /// </summary>
        /// <param name="message">The message to publish</param>
        /// <returns></returns>
        public async Task PublishMessage(FarmCraftMessage message)
        {
            if (_sender == null)
                throw new Exception("Sender not initialized");

            try
            {
                string stringMessage = JsonConvert.SerializeObject(message);
                ServiceBusMessage sbMessage = new ServiceBusMessage(stringMessage);
                await _sender.SendMessageAsync(sbMessage);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Publishes a batch of messages to the message queue
        /// </summary>
        /// <param name="messages">The messages to publish</param>
        /// <param name="batchSize">The size of the batch to publish</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task PublishMessageBatch(List<IMessage> messages, int batchSize)
        {
            if (_sender == null)
                throw new Exception("Sender not initialized");

            ServiceBusMessageBatch batch = await _sender.CreateMessageBatchAsync();
            for(int i = 0; i < messages.Count; i++)
            {
                string stringMessage = JsonConvert.SerializeObject(messages[i]);
                ServiceBusMessage sbMessage = new ServiceBusMessage(stringMessage);
                if (!batch.TryAddMessage(sbMessage))
                {
                    batch.Dispose();
                    throw new Exception($"Message {i} is too large for the batch!");
                }

                if(batch.Count == batchSize)
                {
                    await TrySendBatch(batch);
                    batch = await _sender.CreateMessageBatchAsync();
                }
            }
                
            await TrySendBatch(batch);
        }

        /// <summary>
        /// A helper method to attempt sending batches to the Service Bus and 
        /// dispose of the object after
        /// </summary>
        /// <param name="batch">The batch to publish</param>
        /// <returns></returns>
        private async Task TrySendBatch(ServiceBusMessageBatch batch)
        {
            if (_sender == null)
                throw new Exception("Sender not initialized");

            try
            {
                await _sender.SendMessagesAsync(batch);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                batch.Dispose();
            }
        }
    }
}
