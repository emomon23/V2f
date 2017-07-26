using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Model;
using Xamarin.Forms;

namespace VoiceToForm.Pages.Settings
{
    public class AppSettingViewModel : BaseModelView
    {
        private string _lastUploadDate;
        private string _lastUploadConfirmation;

        private DataStore _ds;

        public AppSettingViewModel(DataStore ds)
        {
            _ds = ds;
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            LastUploadDate = _ds.AppSettings.DateLastUploadOccurred.HasValue
                ? _ds.AppSettings.DateLastUploadOccurred.Value.ToString()
                : "Never";

           LastUploadConfirmation = _ds.AppSettings.LastUploadConfirmation.IsNull()
                ? "NA"
                : _ds.AppSettings.LastUploadConfirmation;

            DeleteAfterUpload = _ds.AppSettings.DeleteDataUponUpload;
        }

        public V2FSettings Settings => _ds.AppSettings;

        public string LastUploadDate
        {
            get { return _lastUploadDate; }
            set
            {
                _lastUploadDate = value;
                OnPropertyChanged();
            }
        }

        public bool DeleteAfterUpload
        {
            get
            {
                return _ds?.AppSettings.DeleteDataUponUpload ?? false;
            }
            set
            {
                if (value != _ds.AppSettings.DeleteDataUponUpload)
                {
                    _ds.AppSettings.DeleteDataUponUpload = value;
                    _ds.SaveAppSettings();
                }

                OnPropertyChanged();
            }
        }

        public bool UploadImmediately
        {
            get
            {
                return _ds?.AppSettings.AttempToUploadImmediately ?? false;
            }
            set
            {
                if (value != _ds.AppSettings.AttempToUploadImmediately)
                {
                    _ds.AppSettings.AttempToUploadImmediately = value;
                    _ds.SaveAppSettings();
                }

                OnPropertyChanged();
            }
        }


        public string LastUploadConfirmation
        {
            get
            {
                return _lastUploadConfirmation;
            }
            set
            {
                _lastUploadConfirmation = value;
                OnPropertyChanged();
            }
        }
    }
}
