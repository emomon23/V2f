using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Model;
using VoiceToForm.Services;
using VoiceToForm.Utils;
using Xamarin.Forms;

namespace VoiceToForm.Pages.ReviewData
{
    public class ReviewDataViewModel : BaseModelView
    {
        private Form _form;
        private FormData _data;
        private DataStore _db = new DataStore();
        private int _currentRow = 0;
        private bool _displayNext = false;
        private bool _displayPrev = false;
        private double _screenHalfWidth = 50;

        //While formdata.endOfRows
        // var currentRow = formData.currentRow
        // formdata.moveNext()

        public ReviewDataViewModel(V2FNavigationService navigationService, Form form)
        {
            _v2FNavigation = navigationService;
            _form = form;
        }

        public override async Task InitializeAsync()
        {
            base.InitializeAsync();
            _data = await _db.GetFormDataAsync(_form.FormName);
            if (_data.Rows != null)
            {
                DisplayNextButton = _data.Rows.Count > 1;
            }

            ScreenHalfWidth = Application.Current.MainPage.Width / 2;
            OnPropertiesChanged("CurrentRowNameValuePair", "DisplayNoDataPresentMessage", "NoDataExistsMessage");
        }

        public double ScreenHalfWidth
        {
            get {  return _screenHalfWidth;}
            set
            {
                _screenHalfWidth = value;
                OnPropertyChanged();
            }

        }

        public bool EndOfRows => _currentRow >= _data.Rows.Count - 1;

        public string NoDataExistsMessage => $"No data exists for form '{_form.FormName}'";

        public async Task MoveNextAsync()
        {
            if (_currentRow < _data.Rows.Count - 1)
            {
                _currentRow += 1;
                OnPropertyChanged("CurrentRowNameValuePair");
                DisplayPrevButton = true;
                if (_currentRow == _data.Rows.Count - 1)
                {
                    DisplayNextButton = false;
                }
            }
          
        }

        public async Task EditCurrent()
        {
            await _v2FNavigation.NavigateToDataEdit(_form, _data.Rows[_currentRow], _currentRow, _dataStore);
        }

        public async Task DeleteRow()
        {
            if (await AreYouSureAsync("Delete Row!?"))
            {
                _data.DeleteRow(_data.Rows[_currentRow]);
                _db.SaveFormDataAsync(_data);

                if (_currentRow >= _data.Rows.Count && _currentRow != 0)
                {
                    _currentRow -= 1;
                }

                if (_data.Rows.Count - 1 == _currentRow)
                {
                    DisplayNextButton = false;
                }

                OnPropertiesChanged("CurrentRowNameValuePair");
            }
        }

        public async Task MovePreviousAsync()
        {
            if (_currentRow > 0)
            {
                _currentRow -= 1;
                OnPropertyChanged("CurrentRowNameValuePair");
                DisplayNextButton = true;
                if (_currentRow == 0)
                {
                    DisplayPrevButton = false;
                }
            }
        }

        public Dictionary<string, string> CurrentRowNameValuePair => _data.Rows.Any()? _data.Rows[_currentRow].GetDisplayFields() : null;
        
        public int Count => _data.Rows.Count;

        public bool DisplayNoDataPresentMessage => Count == 0;

        public bool DisplayNextButton
        {
            get { return _displayNext; }
            set
            {
                _displayNext = value;
                OnPropertyChanged();
            }
        }

        public bool DisplayPrevButton
        {
            get
            {
                return _displayPrev;
            }
            set
            {
                _displayPrev = value;
                OnPropertyChanged();
            }
        }

        public double DataContentHeight
        {
            get { return App.Current.MainPage.Height - 50; }
        }
    }

    
}
