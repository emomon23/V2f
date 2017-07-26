using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace VoiceToForm.Pages.Settings
{
    public partial class AppSettings : ContentPage
    {
        private AppSettingViewModel _vm;

        public AppSettings(AppSettingViewModel vm)
        {
            InitializeComponent();
            this._vm = vm;
            this.BindingContext = vm;
        }
    }
}
