namespace FarmCraft.Core.Services.Messaging
{
    /// <summary>
    /// An Interface for classes that can be registered
    /// with an IMessageConsumer to handle messages of a 
    /// given type
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// An action to take when receiving a message
        /// </summary>
        /// <param name="message">The message object that was received
        /// from a message broker</param>
        /// <returns></returns>
        Task Receive(object message);
    }


}
