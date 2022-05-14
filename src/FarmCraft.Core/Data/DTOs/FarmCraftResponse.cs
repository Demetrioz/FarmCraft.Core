namespace FarmCraft.Core.Data.DTOs
{
    /// <summary>
    /// The possible results of any FarmCraft request
    /// </summary>
    public enum ResponseStatus
    {
        Success,
        Failure
    }

    /// <summary>
    /// Properties contained within every response from
    /// a FarmCraft system or service
    /// </summary>
    public abstract class FarmCraftResponse
    {
        /// <summary>
        /// The unique Id of a request
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// Whether or not the request was successful
        /// </summary>
        public ResponseStatus Status { get; set; }

        /// <summary>
        /// Data returned from the api or actor
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// An error message if something goes wrong
        /// </summary>
        public string Error { get; set; }
    }
}
