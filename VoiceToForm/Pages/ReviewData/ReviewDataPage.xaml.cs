using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Utils;
using Xamarin.Forms;

namespace VoiceToForm.Pages.ReviewData
{
    public partial class ReviewDataPage : ContentPage
    {
        private ReviewDataViewModel _vm;

        public ReviewDataPage(ReviewDataViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            this.BindingContext = vm;

            AddNextPrevButtons();
        }
        
        private void AddNextPrevButtons()
        {
            var width = App.Current.MainPage.Width / 2 - 10;

            var btn = (Layout)ImgBtnGenerator.AddButton(prevButtonContainer, _vm.MovePreviousAsync, width, 75, "Prev", "Prev.png");
            btn.SetBinding(Button.IsVisibleProperty, "DisplayPrevButton");
            
            btn = (Layout)ImgBtnGenerator.AddButton(nextButtonContainer, _vm.MoveNextAsync, width, 75, "Next", "Next.png");
            btn.SetBinding(Button.IsVisibleProperty, "DisplayNextButton");

            if (_vm.Count > 0) {
                this.CreateToolbarButton(_vm.DeleteRow, "Delete", false);
                this.CreateToolbarButton(_vm.EditCurrent, "Edit", true);
            }
        }

        protected override async void OnAppearing()
        {
            await _vm.InitializeAsync();
        }
    }
}
