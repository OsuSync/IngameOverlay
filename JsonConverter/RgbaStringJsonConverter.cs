using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IngameOverlay.JsonConverter
{
    class RgbaStringJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            float[] rgba = (float[]) value;
            byte r, g, b, a;
            r = (byte)(rgba[0] * 255);
            g = (byte)(rgba[1] * 255);
            b = (byte)(rgba[2] * 255);
            a = (byte)(rgba[3] * 255);
            writer.WriteValue($"#{r:X2}{g:X2}{b:X2}{a:X2}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(string))
            {
                string rgbaStr = reader.Value as string;
                if (rgbaStr == null || !rgbaStr.StartsWith("#"))
                    throw new JsonSerializationException();
                float[] rgba = new float[4];
                rgba[0] = Convert.ToByte(rgbaStr.Substring(1, 2), 16) / 255.0f;
                rgba[1] = Convert.ToByte(rgbaStr.Substring(3, 2), 16) / 255.0f; ;
                rgba[2] = Convert.ToByte(rgbaStr.Substring(5, 2), 16) / 255.0f; ;
                rgba[3] = Convert.ToByte(rgbaStr.Substring(7, 2), 16) / 255.0f; ;
                return rgba;
            }

            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(float[]) == objectType;
        }
    }
}
