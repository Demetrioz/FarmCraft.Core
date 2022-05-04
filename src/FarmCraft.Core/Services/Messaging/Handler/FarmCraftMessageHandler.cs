using FarmCraft.Core.Messaging;
using Newtonsoft.Json;

namespace FarmCraft.Core.Services.Messaging.Handler
{
    /// <summary>
    /// A base class that can be used to create message handlers within
    /// FarmCraft services
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FarmCraftMessageHandler<T> : IMessageHandler where T : class
    {
        /// <summary>
        /// Receives a message from the message broker and creates a typed
        /// data object based on the handler's generic type, T
        /// </summary>
        /// <param name="message">The message from the message broker</param>
        /// <returns></returns>
        public async Task Receive(object message)
        {
            FarmCraftMessage typedMessage = message as FarmCraftMessage;
            var stringData = JsonConvert.SerializeObject(typedMessage.Data);
            var typedData = JsonConvert.DeserializeObject<T>(stringData);
            await HandleMessage(typedData);
        }

        /// <summary>
        /// A placeholder for logic that runs after generating the typed data in 
        /// Receive(). This needs to be implemented manually for each handler.
        /// </summary>
        /// <param name="data">The typed data the handler deals with</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual async Task HandleMessage(T data)
        {
            throw new NotImplementedException($"No handler found for {typeof(T).Name}. " +
                "Did you override the HandleMessage function?");
        }
    }
}
