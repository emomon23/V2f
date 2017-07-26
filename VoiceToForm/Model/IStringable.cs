using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VoiceToForm.Model
{
    public interface IStringable
    {
        string Serialize();
        void Deserialize(string value);
        string DataKey { get; }
        string Extension { get; }
    }
}
