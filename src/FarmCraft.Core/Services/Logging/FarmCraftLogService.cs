using FarmCraft.Core.Data.Context;
using FarmCraft.Core.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace FarmCraft.Core.Services.Logging
{
    public class FarmCraftLogService : ILogService
    {
        private readonly IFarmCraftContext _dbContext;

        /// <summary>
        /// Sets the LogService's context and source
        /// </summary>
        /// <param name="context"></param>
        public FarmCraftLogService(IFarmCraftContext dbContext) 
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Logs the given exception as an error event. The message property 
        /// contains both the top level and inner exception message, while 
        /// the data property contains the stack trace
        /// </summary>
        /// <param name="ex">The exception that should be logged</param>
        /// <returns></returns>
        public async Task LogAsync(Exception ex, string? source = null)
        {
            if(_dbContext.Logs != null)
            {
                await _dbContext.AddAsync(new FarmCraftLog
                {
                    LogId = Guid.NewGuid().ToString(),
                    Timestamp = DateTimeOffset.UtcNow,
                    LogLevel = LogLevel.Error,
                    Source = source,
                    Message = $"{ex.Message} || {ex.InnerException?.Message}",
                    Data = ex.StackTrace
                });

                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Logs a given message and data object at the level provided.
        /// </summary>
        /// <param name="level">The level of the logged event</param>
        /// <param name="message">The message that should be logged</param>
        /// <param name="data">Optional data that accompanies the logged event</param>
        /// <returns></returns>
        public async Task LogAsync(LogLevel level, string message, object? data, string? source = null)
        {
            string? dataString = data != null
                ? JsonConvert.SerializeObject(data)
                : null;

            if(_dbContext.Logs != null)
            {
                await _dbContext.AddAsync(new FarmCraftLog
                {
                    LogId = Guid.NewGuid().ToString(),
                    Timestamp = DateTimeOffset.UtcNow,
                    LogLevel = level,
                    Source = source,
                    Message = message,
                    Data = dataString
                });

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
