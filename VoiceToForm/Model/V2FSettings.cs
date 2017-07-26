using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Utils;

namespace VoiceToForm.Model
{
    public class V2FSettings : IStringable
    {
        public string AppIdentifier { get; set; }
        public string AppPhrase { get; set; }
        public DateTime? DateLastUploadOccurred { get; set; }
        public string LastUploadConfirmation { get; set; }
        public string LastUploadError { get; set; }
        public bool AttempToUploadImmediately { get; set; }
        public bool DeleteDataUponUpload { get; set; }
      
        public string Serialize()
        {
            V2FObjectString objectString = new V2FObjectString();
            objectString.SerializeValue("AppIdentifier", AppIdentifier);
            objectString.SerializeValue("AppPhrase", AppPhrase);
            objectString.SerializeDate("DateLastUploadOccurred", DateLastUploadOccurred);
            objectString.SerializeValue("LastUploadConfirmation", LastUploadConfirmation);
            objectString.SerializeValue("LastUploadError", LastUploadError);
            objectString.SerializeValue("AttempToUploadImmediately", AttempToUploadImmediately);
            objectString.SerializeValue("DeleteDataUponUpload", DeleteDataUponUpload);

            var result = objectString.ToString();
            return result;
        }

        public void Deserialize(string value)
        {
            V2FObjectString objectString = new V2FObjectString(value);
            AppIdentifier = objectString.DeserializeString("AppIdentifier");
            AppPhrase = objectString.DeserializeString("AppPhrase");
            DateLastUploadOccurred = objectString.DeserializeDate("DateLastUploadOccurred");
            LastUploadConfirmation = objectString.DeserializeString("LastUploadConfirmation");
            LastUploadError = objectString.DeserializeString("LastUploadError");
            AttempToUploadImmediately = objectString.DeserializeBool("AttempToUploadImmediately");
            DeleteDataUponUpload = objectString.DeserializeBool("DeleteDataUponUpload");
        }

        public string DataKey { get { return "V2FSettings"; } }

        public string Extension { get { return ".setting"; } }

    }
}
