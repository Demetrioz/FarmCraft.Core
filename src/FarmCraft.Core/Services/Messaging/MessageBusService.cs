using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace FarmCraft.Core.Services.Messaging
{
    public class MessageBusService : IDisposable
    {
        private static ServiceBusClient? _client;

        public MessageBusService(IOptions<MessageBusOptions> options)
        {
            MessageBusOptions mbOptions = options.Value;
            if (_client == null)
                _client = new ServiceBusClient(mbOptions.Host);
        }

        public void Dispose()
        {
            if(_client != null)
                _client.DisposeAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates a ServiceBusProcessor for the given queue that does
        /// not auto complete messages. Processor must be disposed prior to
        /// exiting.
        /// </summary>
        /// <param name="queue">The queue name that should be processed</param>
        /// <returns>A ServiceBusProcessor object</returns>
        public ServiceBusProcessor CreateConsumer(string queue)
        {
            if (_client == null)
                throw new Exception("Service Bus client not initialized");

            return _client.CreateProcessor(
                queue,
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false
                }
            );
        }

        /// <summary>
        /// Creates a ServiceBusSender for the given queue. Sender must be disposed
        /// prior to exiting
        /// </summary>
        /// <param name="queue">The queue name that messages should be published to</param>
        /// <returns>A ServiceBusSender object</returns>
        public ServiceBusSender CreatePublisher(string queue)
        {
            if (_client == null)
                throw new Exception("Service Bus client not initialized");

            return _client.CreateSender(queue);
        }
    }
}
