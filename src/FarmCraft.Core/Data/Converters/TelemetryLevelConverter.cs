using FarmCraft.Core.Data.Entities;
using Newtonsoft.Json;

namespace FarmCraft.Core.Data.Converters
{
    public class TelemetryLevelConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        )
        {
            try
            {
                string value = reader.Value == null
                    ? ""
                    : (string)reader.Value;

                switch (value)
                {
                    case "Info":
                        return TelemetryLevel.Info;
                    case "Warning":
                        return TelemetryLevel.Warning;
                    case "Error":
                        return TelemetryLevel.Error;
                    default:
                        return TelemetryLevel.Info;
                }
            }
            catch (Exception)
            {
                return LogLevel.Info;
            }
        }

        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer
            serializer
        )
        {
            try
            {
                TelemetryLevel level = value != null
                    ? (TelemetryLevel)value
                    : TelemetryLevel.Info;

                switch (level)
                {
                    case TelemetryLevel.Info:
                        writer.WriteValue("Info");
                        break;
                    case TelemetryLevel.Warning:
                        writer.WriteValue("Warning");
                        break;
                    case TelemetryLevel.Error:
                        writer.WriteValue("Error");
                        break;
                    default:
                        writer.WriteNull();
                        break;
                }
            }
            catch (Exception)
            {
                writer.WriteNull();
            }
        }
    }
}
