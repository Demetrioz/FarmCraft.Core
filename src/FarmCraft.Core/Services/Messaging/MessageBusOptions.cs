namespace FarmCraft.Core.Services.Messaging
{
    /// <summary>
    /// A generic set of options for dealing with a 
    /// message broker
    /// </summary>
    public class MessageBusOptions
    {
        /// <summary>
        /// The name of the connection host. This could be
        /// the hostname or an entire connection string
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The name of the queue to connect to
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// The name of a topic to connect to
        /// </summary>
        public string Topic { get; set; }
    }
}
