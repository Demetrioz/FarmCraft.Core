using FarmCraft.Core.Messaging;

namespace FarmCraft.Core.Services.Messaging
{
    /// <summary>
    /// An interface for consuming messages from a message broker
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// Registers a message handler to handle messages of a given type
        /// </summary>
        /// <typeparam name="T">The message type that should be handled</typeparam>
        /// <typeparam name="H">The handler that should be called when receiving
        /// a message of type T</typeparam>
        void Register<T, H>() where T : IMessage where H : IMessageHandler;

        /// <summary>
        /// Registers a message handler to handle messages of a given type
        /// </summary>
        /// <typeparam name="T">The message type that should be handled</typeparam>
        /// <param name="handler">The action that should be called when receiving
        /// a message of type T</param>
        void Register<T>(Action<FarmCraftMessage> handler) where T : IMessage;
    }
}
