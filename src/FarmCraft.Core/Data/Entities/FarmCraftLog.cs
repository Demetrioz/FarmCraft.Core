using FarmCraft.Core.Data.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FarmCraft.Core.Data.Entities
{
    /// <summary>
    /// Levels of log events
    /// </summary>
    public enum LogLevel
    {
        None,
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    public class FarmCraftLog
    {
        [Key]
        public string LogId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        [JsonConverter(typeof(LogLevelConverter))]
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public string? Source { get; set; }
        public string? Data { get; set; }
    }
}
