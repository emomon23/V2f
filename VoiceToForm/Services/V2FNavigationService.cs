using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Model;
using VoiceToForm.Pages.EditExistingForms;
using VoiceToForm.Pages.FormDefinition;
using VoiceToForm.Pages.FormsList;
using VoiceToForm.Pages.RecordData;
using VoiceToForm.Pages.ReviewData;
using VoiceToForm.Pages.Settings;
using VoiceToForm.Pages.UploadConfirmation;
using VoiceToForm.Utils;
using Xamarin.Forms;

namespace VoiceToForm.Services
{
    public class V2FNavigationService
    {
        private INavigation _navigation;

        public V2FNavigationService(INavigation navigation)
        {
            _navigation = navigation;
        }

        public async Task NavigateToFormEdit(Form theForm)
        {
            FormDefinitionViewModel vm = new FormDefinitionViewModel(this, false)
            {
                FormName = theForm.FormName,
                FormId = theForm.FormId,
                FormFields = string.Join(",", theForm.FieldNames),
                PostEndPointUrl = theForm.PostUrl,
                AllowDelete = true
            };

            await vm.InitializeAsync();
            var formDefPage = new FormDefintionPage(vm);
            await _navigation.PushAsync(formDefPage);
        }

        public async Task NavigateToDataReview(Form form)
        {
            var vm = new ReviewDataViewModel(this, form);
            await vm.InitializeAsync();

            ReviewDataPage page = new ReviewDataPage(vm);
            await _navigation.PushAsync(page);
        }

        public async Task NavigateToAppSettings(DataStore ds)
        {
            AppSettingViewModel vm = new AppSettingViewModel(ds);
            await vm.InitializeAsync();

            AppSettings appSettingsPage = new AppSettings(vm);
            await _navigation.PushAsync(appSettingsPage);
        }

        public async Task NavigateBackToDataReview()
        {
            _navigation.GoBackOnePage();
        }

        public async Task NavigateToDataEdit(Form form, Dictionary<string, string> rowToEdit, int currentIndex, DataStore ds)
        {
            var vm = new DataRecorderViewModel(this, form, ds, rowToEdit, currentIndex);
            await vm.InitializeAsync();

            DataRecorder page = new DataRecorder(vm);
            await _navigation.PushAsync(page);
        }

        public async Task NavigateToUploadDataForm(DataStore ds)
        {
            var vm = new UploadViewModel(this, ds);
            await vm.InitializeAsync();
            UploadPage page = new UploadPage(vm);

            await _navigation.PushAsync(page);
        }

        public async Task NavigateToFormList(FormsListViewModel.FormListDestinationEnumeration destination, List<Form> formList = null)
        {
            var vm = new FormsListViewModel(this, destination);
            await vm.InitializeAsync(formList);

            var form = new FormsList(vm);
            await _navigation.PushAsync(form);
        }

        public async Task NavigateToVoiceCaptre(Form theForm, DataStore ds)
        {
            var vm = new DataRecorderViewModel(this, theForm, ds);
            await vm.InitializeAsync();

            DataRecorder page = new DataRecorder(vm);
            await _navigation.PushAsync(page);
        }

        public async Task NavigateToNewForm(bool isNewInstallation)
        {
            FormDefinitionViewModel vm = new FormDefinitionViewModel(this, isNewInstallation);
            await vm.InitializeAsync();

            var formDefPage = new FormDefintionPage(vm);
            await _navigation.PushAsync(formDefPage);
        }

        public async Task BackToHomePage(string displayMessage = null)
        {
            var mainPageViewModel = Application.Current.MainPage.BindingContext;
            //not sure if mainPageViewModel will try to get from the naviation page or 'MainPage'?

            await _navigation.PopToRootAsync(true);
        }


    }
}
