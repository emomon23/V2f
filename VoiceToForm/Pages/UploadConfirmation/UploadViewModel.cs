using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Model;
using VoiceToForm.Services;

namespace VoiceToForm.Pages.UploadConfirmation
{
    public class UploadViewModel :BaseModelView
    {
        private VoiceToFormUpload _data;
        private bool _uploading;
        private string _message;
        private bool _uploadedComplete;

        public UploadViewModel(V2FNavigationService navigation, DataStore ds)
        {
            _dataStore = ds;
            _v2FNavigation = navigation;
        }

        public override async Task InitializeAsync()
        {
            _data = await _dataStore.GetDataToUpload();
            UploadMessage = $"There are {_data.RowCount} total rows of data to be uploaded.";
        }

        public bool DeleteRowsAfterUpload { get; set; }

        public bool Uploading
        {
            get
            {
                return _uploading;
            }
            set
            {
                _uploading = value;
                OnPropertyChanged();
            }
        }

        public bool DisplayCheckBox => !_uploadedComplete;

        public string UploadMessage
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public async Task UploadData()
        {
            var yoBigonServer = new YoBigonServer(_dataStore);
            Uploading = true;
            UploadMessage = "Uploading...";
            var confNumber = await yoBigonServer.UploadFormData(_data, DeleteRowsAfterUpload);
            Uploading = false;
            _uploadedComplete = true;
            OnPropertyChanged("DisplayCheckBox");

            var nl = Environment.NewLine;
            UploadMessage =
                $"Your data has been uploaded. {nl}Conf #: {confNumber}{nl}{nl}Visit www.YoBigOn.com to download your data";
        }
    }
}
