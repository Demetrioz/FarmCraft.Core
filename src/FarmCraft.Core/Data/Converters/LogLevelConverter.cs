using FarmCraft.Core.Data.Entities;
using Newtonsoft.Json;

namespace FarmCraft.Core.Data.Converters
{
    public class LogLevelConverter : JsonConverter
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
                        return LogLevel.Info;
                    case "Warning":
                        return LogLevel.Warning;
                    case "Error":
                        return LogLevel.Error;
                    case "Critical":
                        return LogLevel.Critical;
                    default:
                        return LogLevel.None;
                }
            }
            catch (Exception)
            {
                return LogLevel.None;
            }
        }

        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer
        )
        {
            try
            {
                LogLevel level = value != null
                    ? (LogLevel)value
                    : LogLevel.None;

                switch (level)
                {
                    case LogLevel.Info:
                        writer.WriteValue("Info");
                        break;
                    case LogLevel.Warning:
                        writer.WriteValue("Warning");
                        break;
                    case LogLevel.Error:
                        writer.WriteValue("Error");
                        break;
                    case LogLevel.Critical:
                        writer.WriteValue("Critical");
                        break;
                    case LogLevel.None:
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
