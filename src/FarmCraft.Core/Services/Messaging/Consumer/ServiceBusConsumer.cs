using Azure.Messaging.ServiceBus;
using FarmCraft.Core.Data.Entities;
using FarmCraft.Core.Messages;
using FarmCraft.Core.Services.Logging;
using FarmCraft.Core.Services.Messaging.Handler;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace FarmCraft.Core.Services.Messaging.Consumer
{
    /// <summary>
    /// A Message Consumer for interacting with Azure Service Bus
    /// </summary>
    public class ServiceBusConsumer : IMessageConsumer, IDisposable
    {
        private readonly FarmCraftLogService _logger;
        private readonly ServiceBusProcessor _processor;

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
        public ServiceBusConsumer(
            MessageBusService service,
            string queueName, 
            FarmCraftLogService logger
        )
        {
            if (service == null)
                throw new Exception("MessageBusService missing");

            _logger = logger;
            _processor = service.CreateConsumer(queueName);
            _processor.ProcessMessageAsync += HandleMessage;
            _processor.ProcessErrorAsync += HandleError;
        }

        /// <summary>
        /// Disposes of the client and processor objects prior to being
        /// destroyed
        /// </summary>
        public void Dispose()
        {
            _processor.DisposeAsync().GetAwaiter().GetResult();
        }

        public async Task BeginProcessing()
        {
            await _processor.StartProcessingAsync();
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
            IMessageHandler? handler = Activator.CreateInstance(typeof(H)) as IMessageHandler;
            if (handler == null)
                throw new Exception("Invalid Handler Type. Did you implement IMessageHandler?");

            Type messageType = typeof(T);
            _handlers.TryAdd(messageType, handler);
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
                    await _logger.LogAsync(
                        LogLevel.Warning,
                        $"Handler not found for {message.MessageType}",
                        null,
                        nameof(ServiceBusConsumer)
                    );
            }
            catch(Exception ex)
            {
                await _logger.LogAsync(ex, nameof(ServiceBusConsumer));
                await args.DeadLetterMessageAsync(args.Message);
            }
        }

        /// <summary>
        /// Handler for receiving an error from the Azure Service Bus
        /// </summary>
        /// <param name="args">The event arguments from Azure Service Bus</param>
        /// <returns></returns>
        public async Task HandleError(ProcessErrorEventArgs args)
        {
            await _logger.LogAsync(args.Exception, nameof(ServiceBusConsumer));
        }
    }
}
