using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoiceToForm.Model;
using VoiceToForm.Pages.FormsList;
using VoiceToForm.Services;
using Xamarin.Forms;

namespace VoiceToForm.Pages.MainPage
{
    public class MainPageViewModel : BaseModelView
    {
        private V2FNavigationService _navigation;
        private YoBigonServer _yoBigonServer;
        private bool _displayUploadButton = false;
        private bool _dataFilesExist = false;
        private string _appIdAndPhrase = "";

        public MainPageViewModel(V2FNavigationService navigationService, DataStore ds)
        {
            _dataStore = ds;
            _yoBigonServer = new YoBigonServer(_dataStore);
            _navigation = navigationService;
        }

        public override async Task InitializeAsync()
        {
            this.DisplayUploadButton = await _dataStore.CheckIfDataIsAvailableToUploadAsync();
            this.Settings = _dataStore.AppSettings;
            this.AppIdAndPhrase = $"App ID: {Settings.AppIdentifier} {Environment.NewLine} Phase: {Settings.AppPhrase}";
            this.DataFilesExist = _dataStore.DataFilesExist;

            var forms = await _dataStore.GetFormDefinitionsAsync();
            if (forms == null || forms.Count == 0)
            {
                await this.DefineFirstInitialForm();
            }
        }

        public V2FSettings Settings { get; private set; }
        public ICommand ViewForms
        {
            get
            {
                return new Command(async () =>
                {
                    var forms = await _dataStore.GetFormDefinitionsAsync();
                    if (forms.Count > 1)
                    {
                        await _navigation.NavigateToFormList(FormsListViewModel.FormListDestinationEnumeration.EditForm);
                    }
                    else if (forms.Count == 1)
                    {
                        await _navigation.NavigateToFormEdit(forms[0]);
                    }
                });
            }
        }

        public async Task EditSettings()
        {
            await _navigation.NavigateToAppSettings(_dataStore);
        }

        public bool DataFilesExist
        {
            get { return _dataFilesExist;  }
            set
            {
                _dataFilesExist = value;
                OnPropertyChanged();
            }
        }

        public ICommand RecordData
        {
            get
            {
                return new Command(async () =>
                {
                    var forms = await _dataStore.GetFormDefinitionsAsync();
                    if (forms.Count == 1)
                    {
                        await _navigation.NavigateToVoiceCaptre(forms[0], _dataStore);
                    }
                    else if (forms.Count > 1)
                    {
                        await _navigation.NavigateToFormList(
                            FormsListViewModel.FormListDestinationEnumeration.RecordData);
                    }
                });
            }
        }

        public async Task DefineFirstInitialForm()
        {
            await _navigation.NavigateToNewForm(true);
        }

        public async Task DefineNewForm()
        {
            await _navigation.NavigateToNewForm(false);
        }
        
        public ICommand ReviewData
        {
            get
            {
                return new Command(async () =>
                {
                    var forms = (await _dataStore.GetFormDefinitionsAsync()).Where(f => f.HasData).ToList();

                    if (forms.Count > 1)
                    {
                        await _navigation.NavigateToFormList(FormsListViewModel.FormListDestinationEnumeration.ViewData, forms);
                    }
                    else if (forms.Count == 1)
                    {
                        await _navigation.NavigateToDataReview(forms[0]);
                    }
                });
            }
        }

        public ICommand UploadData
        {
            get
            {
                return new Command(async () =>
                {
                    await _navigation.NavigateToUploadDataForm(_dataStore);
                });
            }
        }

        public bool DisplayUploadButton
        {
            get { return _displayUploadButton; }
            set
            {
                _displayUploadButton = value;
                OnPropertyChanged();
            }
        }

        public string AppIdAndPhrase
        {
            get
            {
                return _appIdAndPhrase;
            }
            set
            {
                _appIdAndPhrase = value;
                OnPropertyChanged();
            }
        }
    }
}
