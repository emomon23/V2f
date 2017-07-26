using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm;

namespace VoiceToForm.Utils
{
    public class V2FObjectString
    {
        private string _value;
        private List<string> _lines;

        public V2FObjectString(string value = null)
        {
            _value = value ?? "";
            _lines = value.Split(Environment.NewLine);
        }

        public void SerializeValue(string name, object value)
        {
            var v = value == null || value.ToString() == name ? "" : value.ToString();

            _value += $"{name}~:~{v}{Environment.NewLine}";
        }

        public void SerializeDate(string name, DateTime? dt)
        {
            var d = dt.HasValue ? dt.ToString() : "";
            SerializeValue(name, d);
        }

        public string DeserializeString(string name)
        {
            foreach (var l in _lines)
            {
                var trimmed = l.Trim();
                if (trimmed.StartsWith($"{name}~:~"))
                {
                    return trimmed.Substring(trimmed.IndexOf("~:~") + 3);
                }
            }

            return null;
        }

        public bool DeserializeBool(string name)
        {
            var v = DeserializeString(name);
            if (v.IsNotNull() && v.ToLower() == "true")
            {
                return true;
            }

            return false;
        }

        public DateTime? DeserializeDate(string name)
        {
            var v = DeserializeString(name);
            return v.ToNullableDateTime();
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
