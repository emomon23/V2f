using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Utils;

namespace VoiceToForm.Model
{
    public class FormData : IStringable
    {
        private List<Dictionary<string, string>> _rows = new List<Dictionary<string, string>>();
        private Form _form;
        public const string V2f_IdKey = "V2f.Id";
        public const string V2f_SaveDateKey = "v2f.DateSaved";
        public const string V2f_UploadedDateKey = "v2f.DateUploaded";

        public FormData(Form form)
        {
            _form = form;
        }

        public List<Dictionary<string, string>> Rows => _rows;

        public string FormName => _form.FormName;

        public string DataKey => _form.FormId;

        public string Extension => ".data";

        public void DeleteRow(Dictionary<string, string> fieldValueRows)
        {
            _rows.Remove(fieldValueRows);
        }

        public void TrackChanges(Dictionary<string, string> fieldValuesRow)
        {
            if (!ReplaceExistingRow(fieldValuesRow))
            {
                //Add a v2f.Id and v2f.DateSaved to the rows
                fieldValuesRow.CreateV2F_Id_andDateSaved();
                _rows.Add(fieldValuesRow);
            }

            
        }

        public bool ContainsDataToUpload
        {
            get
            {
                return Rows.Any(r => r.GetDateUploaded() == "");
                
            }
        }

        public string Serialize()
        {
            var nl = Environment.NewLine;
            StringBuilder blder = new StringBuilder();
          
            //FirstName:Mike,LastName:Emo,Age:50
            foreach (var r in _rows)
            {
                string temp = "";
                foreach (var fieldName in _form.FieldNames)
                {
                    var value = r.ContainsKey(fieldName) ? r[fieldName] : "";
                    temp += $"{fieldName}:{value},";
                }

                temp += r.SerializeV2fFields();

                blder.Append(temp.Substring(0, temp.Length - 1) + nl);
            }
            return blder.ToString();
        }

        public void Deserialize(string value)
        {
            //First Name:Mike,Last Name:EmoCRLF
            //First Name:Jon,Last Name:NelsonCRLF
            var lines = value.SplitOnNLCR();
           
            foreach (var line in lines)
            {
                var dataFound = false;
                var row = new Dictionary<string, string>();

                if (line.IsNotNullOrEmpty())
                {
                    var fields = line.Split(','); //FirstName:Mike
                    foreach (var field in fields)
                    {
                        if (field.IsNotNullOrEmpty())
                        {
                            var nameValuePairString = field.Split(':');
                            if (nameValuePairString.Length == 2)
                            {
                                var isFormField = _form.FieldNames.Any(f => f == nameValuePairString[0]);
                                var isV2fField = nameValuePairString[0].ContainsOr(FormData.V2f_IdKey, FormData.V2f_SaveDateKey, FormData.V2f_UploadedDateKey);

                                if (isFormField || isV2fField)
                                {
                                    var npValue = nameValuePairString[0].ContainsOr(FormData.V2f_UploadedDateKey, FormData.V2f_SaveDateKey) ? nameValuePairString[1].DeserializeV2fDateSaved() : nameValuePairString[1];
                                    row.Add(nameValuePairString[0], npValue);
                                    dataFound = true;
                                }
                            }

                        }
                    }
                }

                if (dataFound)
                {
                    _rows.Add(row);
                }
            }

        }

        public bool MarkDataAsUploaded()
        {
            var updated = false;
            foreach (var r in this.Rows)
            {
                if (!r.ContainsKey(FormData.V2f_UploadedDateKey) || r[FormData.V2f_UploadedDateKey] == "")
                {
                    r[FormData.V2f_UploadedDateKey] = DateTime.Now.ToString("g");
                    updated = true;
                }
            }

            return updated;
        }

        private bool ReplaceExistingRow(Dictionary<string, string> newRow)
        {
            if (!newRow.ContainsKey(V2f_IdKey))
            {
                return false;
            }

            var lookFor = newRow[V2f_IdKey];
            if (lookFor.IsNullOrEmpty())
            {
                return false;
            }

            var result = false;
            for (int i = 0; i < _rows.Count; i++)
            {
                if (_rows[i][V2f_IdKey] == lookFor)
                {
                    _rows.Remove(_rows[i]);
                    _rows.Insert(i, newRow);
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
