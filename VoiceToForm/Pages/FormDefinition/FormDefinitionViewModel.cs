using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Model;
using VoiceToForm.Services;
using VoiceToForm.Utils;

namespace VoiceToForm.Pages.FormDefinition
{
    public class FormDefinitionViewModel :BaseModelView
    {
        DataStore _db = new DataStore();
        private bool _isNewInstallation;
        private string _formFields = "";
        private string _formText = "";
        private string _formErrorMessage;

        public FormDefinitionViewModel(V2FNavigationService navigationService, bool isNewInstallation)
        {
            _v2FNavigation = navigationService;
            _isNewInstallation = isNewInstallation;
            FormId = Guid.NewGuid().ToString();
            _dataStore = new DataStore();
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
           
            if (FormName.IsNotNull())
            {
                FormLabelText = $"Edit {FormName}. ";
            }
            else
            {
                FormLabelText = _isNewInstallation == false
                    ? "Create a New Form defintion.  "
                    : $@"Thank you for downloading YoBigOn Voice to Field. {Environment.NewLine}
Start by defining the data you want to record.  ";
            }

           
        }

        public string FormLabelText
        {
            get { return _formText; }
            set
            {
                _formText = value;
                OnPropertyChanged();
            }
        }

        public string FormErrorMessage
        {
            get { return _formErrorMessage; }
            set
            {
                _formErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public string FormName { get; set; }

        public string FormId { get; set; }

        public string FormFields
        {
            get { return _formFields; }
            set
            {
                _formFields = value;
                OnPropertyChanged();
            }
        }

        public void ClearErrorMessage()
        {
            FormErrorMessage = "";
        }

        public string PostEndPointUrl { get; set; }
        public bool AllowDelete { get; set; }

        public async Task SaveForm()
        {
            if ((await ValidateFormAsync()))
            {
                var form = new Form()
                {
                    FormId = this.FormId,
                    FormName = this.FormName.CaptializeFirstLetter(),
                    FieldNames = this.FormFields.SplitAndTrim(','),
                    PostUrl = this.PostEndPointUrl
                };

                await _db.SaveFormDefinitionAsync(form);
                await _v2FNavigation.BackToHomePage("Data Saved");
            }
        }

        private async Task<bool> ValidateFormAsync()
        {
            var message = FormName.IsNullOrEmpty() ? $"Form name is required. {Environment.NewLine}" : "";
            message = FormFields.IsNotNull() ? message : message + $"You must specify at least 1 field name to capture. {Environment.NewLine}";

            if (FormFields.Split(",").ContainsDuplicates())
            {
                message += $"You have duplicate field names.{Environment.NewLine}";
            }

            var existingForm = await _dataStore.GetForm(FormName);
            if (existingForm != null && existingForm.FormId != FormId)
            {
                message += $"There already is a form named {FormName}.  Form name must be unique.";
            }

            if (message.IsNotNull())
            {
                FormErrorMessage = message;
                return false;
            }

            return true;
        }

        public async Task DeleteForm()
        {
            if (await (AreYouSureAsync($"Delete {FormName}")))
            {
                await _db.DeleteFormAsync(FormName);
                await _v2FNavigation.BackToHomePage($"{FormName} Deleted");
            }
        }
    }
}
