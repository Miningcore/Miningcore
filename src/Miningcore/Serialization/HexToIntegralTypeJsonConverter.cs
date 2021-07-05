using System;
using System.Globalization;
using System.Numerics;
using Miningcore.Extensions;
using NBitcoin;
using Newtonsoft.Json;

namespace Miningcore.Serialization
{
    public class HexToIntegralTypeJsonConverter<T> : JsonConverter
    {
        private readonly Type underlyingType = Nullable.GetUnderlyingType(typeof(T));

        public override bool CanConvert(Type objectType)
        {
            return typeof(T) == objectType;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value == null)
                writer.WriteValue("null");
            else
            {
                if(value == null)
                    writer.WriteValue("null");
                else
                {
                    // Remove all 0 at the beginning. 0 after 0x is not allowed when the value is not 0
                    object valueToHex = $"{value:x}".TrimStart(new Char[] { '0' });
                    // If value was 0, after trim it is null. Correcting it to 0x0.
                    if(object.Equals(valueToHex, ""))
                    {
                        writer.WriteValue($"0x{value:x}");
                    }
                    else
                    {
                        writer.WriteValue($"0x{valueToHex}");
                    }
                }
            }    
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var str = (string) reader.Value;

            if(string.IsNullOrEmpty(str))
                return default(T);

            if(str.StartsWith("0x"))
                str = str[2..];

            if(typeof(T) == typeof(BigInteger))
                return BigInteger.Parse("0" + str, NumberStyles.HexNumber);

            if(typeof(T) == typeof(uint256))
                return new uint256(str.HexToReverseByteArray());

            var val = ulong.Parse("0" + str, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            return Convert.ChangeType(val, underlyingType ?? typeof(T));
        }
    }
}
