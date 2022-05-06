using Newtonsoft.Json;
using FarmCraft.Core.Data.Converters;

namespace FarmCraft.Core.Data.Entities
{
    public enum TelemetryLevel
    {
        Info,
        Warning,
        Error,
    }

    public class FarmCraftTelemetry
    {
        public string DeviceId { get; set; }
        [JsonConverter(typeof(TelemetryLevelConverter))]
        public TelemetryLevel Level { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public string? PumpStatus { get; set; }
    }
}
