using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Model;
using VoiceToForm.Pages.MainPage;
using VoiceToForm.Services;
using VoiceToForm.Utils;
using Xamarin.Forms;

namespace VoiceToForm
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel _vm;
        public MainPage(DataStore ds)
        {
            InitializeComponent();
            
            _vm = new MainPageViewModel(new V2FNavigationService(this.Navigation), ds);
            this.BindingContext = _vm;

            CreateControls();
        }

        protected override async void OnAppearing()
        {
            await _vm.InitializeAsync();
        }

        public async Task NewInstallationInitializedAsync()
        {
            await _vm.InitializeAsync();
        }

        private void CreateControls()
        {
            ImgBtnGenerator.AddButton(btnContainer, _vm.ViewForms, 100, 50, "Forms", "forms.gif");
            ImgBtnGenerator.AddButton(btnContainer, _vm.RecordData, 100, 50, "Record Data", "record1.png");
            var btn =ImgBtnGenerator.AddButton(btnContainer, _vm.ReviewData, 100, 50, "Review Data", "reviewIcon.png");
            btn.SetBinding(View.IsVisibleProperty, "DataFilesExist");

            btn = ImgBtnGenerator.AddButton(btnContainer, _vm.UploadData, 100, 50, "Upload Data", "upload1.png");
            btn.SetBinding(View.IsVisibleProperty, "DisplayUploadButton");

            btn = ImgBtnGenerator.AddButton(btnContainer, _vm.EditSettings, 100, 50, "Settings", "settingsIcon1.ico");

            btnContainer.AddLabel("AppIdAndPhrase", 30, 80);

            this.CreateAddToolbarButton(_vm.DefineNewForm);
        }
    }
}
