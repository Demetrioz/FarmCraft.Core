namespace FarmCraft.Core.Messages
{
    /// <summary>
    /// The type of messages sent and received by FarmCraft services
    /// </summary>
    public class FarmCraftMessage : IMessage
    {
        public DateTimeOffset Timestamp { get; set; }
        public Type MessageType { get; set; }
        public object Data { get; set; }
    }
}
