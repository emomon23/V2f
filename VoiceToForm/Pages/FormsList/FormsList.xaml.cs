using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Pages.FormsList;
using VoiceToForm.Utils;
using Xamarin.Forms;

namespace VoiceToForm.Pages.EditExistingForms
{
    public partial class FormsList : ContentPage
    {
        private FormsListViewModel _vm;
        public FormsList(FormsListViewModel vm)
        {
            InitializeComponent();
            this.BindingContext = vm;
            _vm = vm;

            this.CreateAddToolbarButton(_vm.DefineNewForm);
        }

    }
}
