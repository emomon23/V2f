using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceToForm.Model;
using VoiceToForm.Services;
using Xamarin.Forms;

namespace VoiceToForm
{
    public partial class App : Application
    {
        DataStore _ds = new DataStore();
        private VoiceToForm.MainPage _mainPage;

        public App()
        {
           // _ds.InitializeNewInstallation();

            InitializeComponent();
           
            _mainPage = new MainPage(_ds);
            MainPage = new NavigationPage(_mainPage)
            {
                BarBackgroundColor = Color.Black,
                BarTextColor = Color.White
            };
        }

        protected override async void OnStart()
        {
            if (_ds.AppSettings.AppPhrase.IsNull())
            {
                await _ds.CreateAppInstanceIdAsync();
                YoBigonServer service = new YoBigonServer(_ds);
                await service.GetAppPhrase();
                await _mainPage.NewInstallationInitializedAsync();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
