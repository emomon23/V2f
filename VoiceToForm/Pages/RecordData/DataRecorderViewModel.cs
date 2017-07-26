using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Model;
using VoiceToForm.Services;
using VoiceToForm.Utils;

namespace VoiceToForm.Pages.RecordData
{
    public class DataRecorderViewModel :BaseModelView
    {
        private Form _form;
        private FormData _formData;
        private Dictionary<string, string> _bindingMap = new Dictionary<string, string>();
        private List<string> _bindings = new List<string>();
        private Dictionary<string, string> _newExistingRow;
        private int _currentIndex;

        public DataRecorderViewModel(V2FNavigationService navigationService, Form form, DataStore ds, Dictionary<string, string> editRow = null, int currentIndex = -1)
        {
            _v2FNavigation = navigationService;
            _form = form;
            _dataStore = ds;

            for (int i = 0; i < 30; i++)
            {
                _bindings.Add("");
            }

            _newExistingRow = editRow ?? new Dictionary<string, string>();
            _currentIndex = currentIndex;
        }

        public override async Task InitializeAsync()
        {
            base.InitializeAsync();
            _formData = await _dataStore.GetFormDataAsync(_form.FormName);
        }

        public Form V2Form => _form;

        public async Task CancelClick()
        {
            var dataExists = await DoesDataExistToSave();
            if (dataExists)
            {
                SaveData();
            }

            if (_currentIndex == -1)
            {
                await _v2FNavigation.BackToHomePage();
            }
        }

        private async Task<bool> DoesDataExistToSave()
        {
            foreach (var fieldName in _form.FieldNames)
            {
                if (fieldName.IsNotNull())
                {
                    var value = GetBindingValueForField(fieldName);
                    if (value.IsNotNull())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task SaveData()
        {
            var dataExists = await DoesDataExistToSave();

            if (dataExists)
            {
                foreach (var fieldName in _form.FieldNames)
                {
                    if (fieldName.IsNotNull())
                    {
                        var value = GetBindingValueForField(fieldName);

                        if (_newExistingRow.ContainsKey(fieldName))
                        {
                            _newExistingRow[fieldName] = value;
                        }
                        else
                        {
                            _newExistingRow.Add(fieldName, value);
                        }
                    }
                }

                _newExistingRow.ClearDateUploaded();
                _formData.TrackChanges(_newExistingRow);
                await _dataStore.SaveFormDataAsync(_formData);

                //Data's been saved locally, check if we should 'auto upload'
                if (_dataStore.AppSettings.AttempToUploadImmediately)
                {
                    YoBigonServer server = new YoBigonServer(_dataStore);
                    var uploadData = await _dataStore.GetDataToUpload();
                    await server.UploadFormData(uploadData, _dataStore.AppSettings.DeleteDataUponUpload);
                }

                if (_currentIndex != -1)
                {
                    //We're editing a record, got back to the data review
                    await _v2FNavigation.NavigateBackToDataReview();
                }
                else
                {
                    //Set up for a new row of data
                    _newExistingRow = new Dictionary<string, string>();
                    ClearForm();
                }
            }
        }
        
        public async Task ProcessVoiceToTextOnForm(string formText)
        {
            var parser = new VoiceCaptureParser();
            var parsedData = parser.ParseVoiceCapture(formText, _form.FieldNames.ToArray());

            foreach (var x in parsedData)
            {
                UpdateBindingForField(x.Key, x.Value);
            }
        }

        public void MapBinding(string fieldName, string bindingName)
        {
            _bindingMap.Add(fieldName, bindingName);

            if (_newExistingRow.ContainsKey(fieldName))
            {
                UpdateBindingForField(fieldName, _newExistingRow[fieldName]);
            }
        }
        
        public string WholeFormBinding { get; set; }

        #region BindingProperties 
        //Need to bind to something, not ideal, but will do for now
        public string Binding0
        {
            get { return _bindings[0]; }
            set
            {
                _bindings[0] = value;
                OnPropertyChanged();
            }
        }

        public string Binding1
        {
            get { return _bindings[1]; }
            set
            {
                _bindings[1] = value;
                OnPropertyChanged();
            }
        }

        public string Binding2
        {
            get { return _bindings[2]; }
            set
            {
                _bindings[2] = value;
                OnPropertyChanged();
            }
        }

        public string Binding3
        {
            get { return _bindings[3]; }
            set
            {
                _bindings[3] = value;
                OnPropertyChanged();
            }
        }

        public string Binding4
        {
            get { return _bindings[4]; }
            set
            {
                _bindings[4] = value;
                OnPropertyChanged();
            }
        }

        public string Binding5
        {
            get { return _bindings[5]; }
            set
            {
                _bindings[5] = value;
                OnPropertyChanged();
            }
        }

        public string Binding6
        {
            get { return _bindings[6]; }
            set
            {
                _bindings[6] = value;
                OnPropertyChanged();
            }
        }

        public string Binding7
        {
            get { return _bindings[7]; }
            set
            {
                _bindings[7] = value;
                OnPropertyChanged();
            }
        }

        public string Binding8
        {
            get { return _bindings[8]; }
            set
            {
                _bindings[8] = value;
                OnPropertyChanged();
            }
        }

        public string Binding9
        {
            get { return _bindings[9]; }
            set
            {
                _bindings[9] = value;
                OnPropertyChanged();
            }
        }

        public string Binding10
        {
            get { return _bindings[10]; }
            set
            {
                _bindings[10] = value;
                OnPropertyChanged();
            }
        }

        public string Binding11
        {
            get { return _bindings[11]; }
            set
            {
                _bindings[11] = value;
                OnPropertyChanged();
            }
        }

        public string Binding12
        {
            get { return _bindings[12]; }
            set
            {
                _bindings[12] = value;
                OnPropertyChanged();
            }
        }

        public string Binding13
        {
            get { return _bindings[13]; }
            set
            {
                _bindings[13] = value;
                OnPropertyChanged();
            }
        }

        public string Binding14
        {
            get { return _bindings[14]; }
            set
            {
                _bindings[14] = value;
                OnPropertyChanged();
            }
        }

        public string Binding15
        {
            get { return _bindings[15]; }
            set
            {
                _bindings[15] = value;
                OnPropertyChanged();
            }
        }

        public string Binding16
        {
            get { return _bindings[16]; }
            set
            {
                _bindings[16] = value;
                OnPropertyChanged();
            }
        }

        public string Binding17
        {
            get { return _bindings[17]; }
            set
            {
                _bindings[17] = value;
                OnPropertyChanged();
            }
        }

        public string Binding18
        {
            get { return _bindings[18]; }
            set
            {
                _bindings[18] = value;
                OnPropertyChanged();
            }
        }

        public string Binding19
        {
            get { return _bindings[1]; }
            set
            {
                _bindings[19] = value;
                OnPropertyChanged();
            }
        }

        public string Binding20
        {
            get { return _bindings[20]; }
            set
            {
                _bindings[20] = value;
                OnPropertyChanged();
            }
        }

        public string Binding21
        {
            get { return _bindings[21]; }
            set
            {
                _bindings[21] = value;
                OnPropertyChanged();
            }
        }

        public string Binding22
        {
            get { return _bindings[22]; }
            set
            {
                _bindings[22] = value;
                OnPropertyChanged();
            }
        }

        public string Binding23
        {
            get { return _bindings[23]; }
            set
            {
                _bindings[23] = value;
                OnPropertyChanged();
            }
        }

        public string Binding24
        {
            get { return _bindings[24]; }
            set
            {
                _bindings[24] = value;
                OnPropertyChanged();
            }
        }

        public string Binding25
        {
            get { return _bindings[25]; }
            set
            {
                _bindings[25] = value;
                OnPropertyChanged();
            }
        }

        public string Binding26
        {
            get { return _bindings[26]; }
            set
            {
                _bindings[26] = value;
                OnPropertyChanged();
            }
        }

        public string Binding27
        {
            get { return _bindings[27]; }
            set
            {
                _bindings[27] = value;
                OnPropertyChanged();
            }
        }

        public string Binding28
        {
            get { return _bindings[2]; }
            set
            {
                _bindings[28] = value;
                OnPropertyChanged();
            }
        }

        public string Binding29
        {
            get { return _bindings[29]; }
            set
            {
                _bindings[29] = value;
                OnPropertyChanged();
            }
        }
    #endregion

        private string GetBindingValueForField(string fieldName)
        {
            var result = "";

            try
            {
                var bindingName = _bindingMap[fieldName];
                var prop = this.GetType().GetRuntimeProperty(bindingName);
                result = prop.GetValue(this).ToString().Replace(",", ".");
            }
            catch (Exception exp)
            {
                //TBD?
            }

            return result;
        }

        private void ClearForm()
        {
            foreach (var fieldName in _form.FieldNames)
            {
                UpdateBindingForField(fieldName, "");
            }
        }

        private void UpdateBindingForField(string fieldName, string value)
        {
            try
            {
                var bindingName = _bindingMap[fieldName];
                var prop = this.GetType().GetRuntimeProperty(bindingName);
                prop.SetValue(this, value);
            }
            catch (Exception exp)
            {
                //TBD?
            }
        }
    }
}
