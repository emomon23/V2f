using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoiceToForm.Model;
using VoiceToForm.Services;
using Xamarin.Forms;

namespace VoiceToForm.Pages.FormsList
{
    public class FormsListViewModel : BaseModelView
    {
        public enum FormListDestinationEnumeration
        {
            EditForm,
            RecordData,
            ViewData,
            UploadData
        }

        private FormListDestinationEnumeration _destination;
        private DataStore _db = new DataStore();
        private ObservableCollection<Form> _forms;

        public FormsListViewModel(V2FNavigationService navService, FormListDestinationEnumeration destination)
        {
            _v2FNavigation = navService;
            _destination = destination;
        }

        public async Task InitializeAsync(List<Form> list = null)
        {
            if (list == null)
            {
                list = await _db.GetFormDefinitionsAsync();
            }
            FormList = new ObservableCollection<Form>(list.OrderBy(f => f.FormName));
        }

        public async Task DefineNewForm()
        {
            await _v2FNavigation.NavigateToNewForm(false);
        }

        public ICommand FormSelected
        {
            get
            {
                return new Command<Form>(async (Form theForm) =>
                {
                    if (_destination == FormListDestinationEnumeration.EditForm)
                    {
                        await _v2FNavigation.NavigateToFormEdit(theForm);
                    }
                    else if (_destination == FormListDestinationEnumeration.RecordData)
                    {
                        await _v2FNavigation.NavigateToVoiceCaptre(theForm, _dataStore);
                    }
                    else if (_destination == FormListDestinationEnumeration.ViewData)
                    {
                        await _v2FNavigation.NavigateToDataReview(theForm);
                    }
                });
            }
        }

        public ObservableCollection<Form> FormList
        {
            get
            {
                return _forms;
            }
            set
            {
                _forms = value;
                OnPropertyChanged("FormList");
            }
        }
    }
}
