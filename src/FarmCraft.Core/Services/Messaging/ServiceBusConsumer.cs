using Azure.Messaging.ServiceBus;
using FarmCraft.Core.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace FarmCraft.Core.Services.Messaging
{
    /// <summary>
    /// A Message Consumer for interacting with Azure Service Bus
    /// </summary>
    public class ServiceBusConsumer : IMessageConsumer, IDisposable
    {
        private readonly ILogger _logger;
        private static ServiceBusClient _client;
        private static ServiceBusProcessor _processor;

        private ConcurrentDictionary<Type, IMessageHandler> _handlers = 
            new ConcurrentDictionary<Type, IMessageHandler>();
        private ConcurrentDictionary<Type, Action<FarmCraftMessage>> _handlerActions = 
            new ConcurrentDictionary<Type, Action<FarmCraftMessage>>();

        /// <summary>
        /// Creates a singluar ServiceBusClient and SericeBusProcessor if they haven't
        /// already been created, then begins processing messages
        /// </summary>
        /// <param name="options">Options for connecting to the Service Bus and Queue</param>
        /// <param name="logger">A generic logger</param>
        public ServiceBusConsumer(IOptions<ConsumerOptions> options, ILogger logger)
        {
            _logger = logger;

            ConsumerOptions consumerOptions = options.Value;

            if (_client == null)
                _client = new ServiceBusClient(consumerOptions.Host);

            if (_processor == null)
            {
                _processor = _client.CreateProcessor(
                    consumerOptions.Queue,
                    new ServiceBusProcessorOptions
                    {
                        AutoCompleteMessages = false
                    }
                );

                _processor.ProcessMessageAsync += HandleMessage;
                _processor.ProcessErrorAsync += HandleError;

                _processor.StartProcessingAsync().Wait();
            }
        }

        /// <summary>
        /// Disposes of the client and processor objects prior to being
        /// destroyed
        /// </summary>
        public void Dispose()
        {
            _processor.DisposeAsync().GetAwaiter().GetResult();
            _client.DisposeAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Registers a message handler class for the given message type
        /// </summary>
        /// <typeparam name="T">The type of message to handle</typeparam>
        /// <typeparam name="H">The handler for the given message type</typeparam>
        public void Register<T, H>()
            where T : IMessage
            where H : IMessageHandler
        {
            Type messageType = typeof(T);
            _handlers.TryAdd(
                messageType, 
                (IMessageHandler)Activator.CreateInstance(typeof(H))
            );
        }

        /// <summary>
        /// Registers a message handler action for the given message type
        /// </summary>
        /// <typeparam name="T">The type of message to handle</typeparam>
        /// <param name="handler">The handler for the given message type</param>
        public void Register<T>(Action<FarmCraftMessage> handler) where T : IMessage
        {
            Type messageType = typeof(T);
            _handlerActions.TryAdd(
                messageType, 
                handler
            );
        }

        /// <summary>
        /// Handler for receiving a service bus message. Creates the FarmCraftMessage object
        /// and attempts to call the handler if one has been registered
        /// </summary>
        /// <param name="args">The event arguments from Azure Service Bus</param>
        /// <returns></returns>
        public virtual async Task HandleMessage(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            FarmCraftMessage message = JsonConvert.DeserializeObject<FarmCraftMessage>(body);

            try
            {
                if (_handlers.TryGetValue(message.MessageType, out var handler))
                {
                    await handler.Receive(message);
                    await args.CompleteMessageAsync(args.Message);
                }
                else if(_handlerActions.TryGetValue(message.MessageType,out var handlerAction))
                {
                    handlerAction.Invoke(message);
                    await args.CompleteMessageAsync(args.Message);
                }
                else 
                    _logger.LogWarning($"Handler not found for {message.MessageType}");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                await args.DeadLetterMessageAsync(args.Message);
            }
        }

        /// <summary>
        /// Handler for receiving an error from the Azure Service Bus
        /// </summary>
        /// <param name="args">The event arguments from Azure Service Bus</param>
        /// <returns></returns>
        public virtual Task HandleError(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
