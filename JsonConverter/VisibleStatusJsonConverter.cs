using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IngameOverlay.JsonConverter
{
    class VisibleStatusJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            uint flags = 0;
            IEnumerable<string> enums = value as IEnumerable<string>;
            foreach (var  field in typeof(VisibleStatus).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (enums.Contains(field.Name))
                    flags |= (uint)field.GetValue(null);
            }

            writer.WriteValue(flags);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(Int64))
            {
                VisibleStatus flag = (VisibleStatus)(Int64)reader.Value;
                return flag.ToString().Split(',').Select(s => s.Trim()).ToList();
            }

            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IList<string>) == objectType;
        }
    }
}
