using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace AileronAirwaysWeb.Models
{
    /// <summary>
    /// A class to handle converting a datetime from ticks into a .NET DateTime object when reading or writing JSON documents.
    /// </summary>
    public class CustomDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime dt)
            {
                // Ideagen expects the ticks to be surrounded by quotation marks.
                writer.WriteRawValue($"\"{dt.Ticks}\"");
            }
            writer.WriteRaw(string.Empty);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is string value && long.TryParse(value, out long ticks))
            {
                return new DateTime(ticks);
            }
            return DateTime.MinValue;
        }
    }
}