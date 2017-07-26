using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Utils;
using Xamarin.Forms;

namespace VoiceToForm.Pages.RecordData
{
    public partial class DataRecorder : ContentPage
    {
        private readonly DataRecorderViewModel _vm;
       
        public DataRecorder(DataRecorderViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            this.BindingContext = vm;
            BuildUI();
        }

        private void BuildUI()
        {
            this.CreateSaveToolbarButton(_vm.SaveData);
            this.CreateToolbarButton(_vm.CancelClick, "Done");

            var fieldSetGenerator = new FieldSetGenerator(mainContentLayout);
            var counter = 0;

            var mainRecordBtn = fieldSetGenerator.CreateVoiceToTextFieldSet("main", "Record all the data at once...", "WholeFormBinding",
                (value) =>
                {
                    if (value != null)
                    {
                        _vm.ProcessVoiceToTextOnForm(value);
                    }
                });

            mainRecordBtn.InputElement.IsVisible = false;


            foreach (var fieldName in _vm.V2Form.FieldNames)
            {
                var binding = $"Binding{counter}";
                _vm.MapBinding(fieldName, binding);

                var identifier = fieldName.Replace(" ", "_");
                fieldSetGenerator.CreateFieldSet(identifier, fieldName, binding);
                
                counter += 1;
            }
        }




    }
}
