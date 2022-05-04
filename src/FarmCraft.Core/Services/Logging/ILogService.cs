using FarmCraft.Core.Data.Entities;

namespace FarmCraft.Core.Services.Logging
{
    /// <summary>
    /// An interface for logging information
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Logs the given exception as an error.
        /// </summary>
        /// <param name="ex">The exception that should be logged</param>
        /// <returns></returns>
        Task LogAsync(Exception ex);

        /// <summary>
        /// Logs a custom message at the given log level.
        /// </summary>
        /// <param name="level">The level of log to write.</param>
        /// <param name="message">The message that should be logged</param>
        /// <param name="data">Optional data that accompanies the logged event</param>
        /// <returns></returns>
        Task LogAsync(LogLevel level, string message, object? data);
    }
}
