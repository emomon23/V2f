using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Utils;
using Xamarin.Forms;

namespace VoiceToForm.Pages.FormDefinition
{
    public partial class FormDefintionPage : ContentPage
    {
        private FormDefinitionViewModel _vm;

        public FormDefintionPage(FormDefinitionViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            this.BindingContext = _vm;
            BuildUI();
        }

        private void BuildUI()
        {
            FieldSetGenerator generator = new FieldSetGenerator(this.formStackLayout);
            generator.CreateFieldSet("formName", "Form Name", "FormName", textChangeEventHandler:this.Form_TextChanged);

            var entry = generator.CreateVoiceToTextFieldSet("formFields", "Fields (A comma seperated list of the fields you want to capture)", "FormFields", textChangeEventHandler:this.Form_TextChanged, voiceToTextConvertedCallback:
                (value) =>
                {
                    if (!value.EndsWith(",") || value.EndsWith(" ,"))
                    {
                        _vm.FormFields = _vm.FormFields + ", ";
                    }

                }, overrideExistingText:false);


            if (_vm.AllowDelete)
            {
                this.CreateSaveToolbarButton(_vm.SaveForm, _vm.DeleteForm);
            }
            else
            {
                this.CreateSaveToolbarButton(_vm.SaveForm);
            }
        }

        public void Form_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm.ClearErrorMessage();
        }
    }
}
