using FarmCraft.Core.Data.Converters;
using System.Text.Json.Serialization;

namespace FarmCraft.Core.Data.Entities
{
    public enum LogLevel
    {
        None,
        Info,
        Warning,
        Error,
        Critical
    }

    public class FarmCraftLog : FarmCraftBase
    {
        public int LogId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        [JsonConverter(typeof(LogLevelConverter))]
        public LogLevel LogLevel { get; set; }
        public string? Source { get; set; }
        public string? Data { get; set; }
    }
}
