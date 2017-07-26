using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Utils;
using Xamarin.Forms;

namespace VoiceToForm.Pages.UploadConfirmation
{
    public partial class UploadPage : ContentPage
    {
        private UploadViewModel _vm;
        public UploadPage(UploadViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            this.BindingContext = vm;
            BuildUI();
        }

        private void BuildUI()
        {
            ImgBtnGenerator btnGenerator = ImgBtnGenerator.GetInstance();
            var chkBoxLayout = btnGenerator.CreateCheckbox(this.checkboxLayout, "Delete data after upload", "DeleteRowsAfterUpload", false);
            checkboxLayout.SetBinding(StackLayout.IsVisibleProperty, "DisplayCheckBox");

            this.CreateToolbarButton(_vm.UploadData, "Upload", false);
        }
    }
}
