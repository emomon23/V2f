using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Utils;

namespace VoiceToForm.Model
{
    public class Form : IStringable
    {
        public Form()
        {
            this.FieldNames = new List<string>();
            this.FormId = Guid.NewGuid().ToString();
        }

        public string FormId { get; set; }
        public string FormName { get; set; }
        public List<string> FieldNames { get; set; }
        public string PostUrl { get; set; }
        public bool HasData { get; set; }

        public void Deserialize(string value)
        {
            var lines = value.SplitOnNLCR();
            foreach (var line in lines)
            {
                string fieldValue = "";
                if (line.IsPropertyValue("FormName: ", out fieldValue))
                {
                    this.FormName = fieldValue;
                }
                else if (line.IsPropertyValue("PostUrl: ", out fieldValue))
                {
                    this.PostUrl = fieldValue;
                }
                else if (line.IsPropertyValue("FieldNames: ", out fieldValue))
                {
                    FieldNames = fieldValue.Split('`').ToList();
                }
                else if (line.IsPropertyValue("Id:", out fieldValue))
                {
                    FormId = fieldValue.Replace(" ", "");
                }

            }
        }

        public string Extension
        {
            get { return ".def"; }
        }

        public string Serialize()
        {
            string nl = Environment.NewLine;
            string fieldNames = string.Join("`", FieldNames);

            var result = $"FormName: {FormName}{nl}FieldNames: {fieldNames}{nl}PostUrl: {PostUrl}{nl}Id: {FormId}";
            return result;
        }

        public string DataKey => FormId.Replace("-", "").Replace(" ", "");
    }
}
