namespace FarmCraft.Core.Messaging
{
    /// <summary>
    /// The base of all FarmCraft messages
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// The UTC time at which a message was published
        /// </summary>
        DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// The type of message being sent. Used to determine which handler
        /// should be used when receiving messages of this type
        /// </summary>
        Type MessageType { get; set; }

        /// <summary>
        /// The actual data contained within the message
        /// </summary>
        object Data { get; set; }
    }


}
