namespace FarmCraft.Core.API
{
    /// <summary>
    /// Properties contained within every API response from
    /// a FarmCraft service
    /// </summary>
    public class FarmCraftApiResponse
    {
        /// <summary>
        /// The unique Id of a request
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// Data returned from the api endpoint
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// An error message if something goes wrong
        /// </summary>
        public string Error { get; set; }
    }
}
