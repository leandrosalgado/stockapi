using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockApi.Converters
{
    /// <summary>
    /// Customer converter for DateOnly as the System.Text.Json doesn't do it out of the box
    /// The code is based on: https://github.com/marcominerva/TinyHelpers/blob/master/src/TinyHelpers/Json/Serialization/ShortDateConverter.cs
    /// </summary>
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private readonly string _serializationFormat;

        public DateOnlyJsonConverter() : this(null)
        {
        }

        public DateOnlyJsonConverter(string? serializationFormat)
        {
            this._serializationFormat = serializationFormat ?? "yyyy-MM-dd";
        }

        public override DateOnly Read(ref Utf8JsonReader reader,
                                Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value,
                                            JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(_serializationFormat));
    }
}
