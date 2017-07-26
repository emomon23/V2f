using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Model;
using VoiceToForm.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoiceToForm.Pages
{
    public abstract class BaseModelView : INotifyPropertyChanged
    {

        protected bool _isBusy = false;
        protected V2FNavigationService _v2FNavigation;
        protected DataStore _dataStore;

        public event PropertyChangedEventHandler PropertyChanged;

        public BaseModelView() { }
        
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        protected async Task<bool> AreYouSureAsync(string prompt)
        {
            var answer = await App.Current.MainPage.DisplayAlert("Are you sure?",
                       prompt, "Yes", "No");

            return answer;
        }

        public virtual async Task InitializeAsync(){}

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void OnPropertiesChanged(params string [] names)
        {
            foreach (var name in names)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

    }

    //Found this work around at: https://forums.xamarin.com/discussion/comment/105285/#Comment_105285
    //When a list view is bound to a collection and for each collection a element with a Command={Binding SomeCommand} is specified
    //The call won't call it on the parent viewmodel, it expects to find it on the context for the list item
    [ContentProperty("ElementName")]
    public class ElementSource : IMarkupExtension
    {
        public string ElementName { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var rootProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
            if (rootProvider == null)
                return null;
            var root = rootProvider.RootObject as Element;
            return root?.FindByName<Element>(ElementName);
        }
    }
}
