using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VoiceToForm.Model;
using Xamarin.Forms;

namespace VoiceToForm.Services
{
    public class YoBigonServer
    {
        private DataStore _dataStore = null;
        private string _appPhrase;
        private readonly string _appInstanceId;
        private const string _baseEndpointUrl = "http://yobigon2.azurewebsites.net/VoiceToForm"; // @"http://yobigon.com/api/VoiceToForm";
        private HttpClient _httpClient;

        public YoBigonServer(DataStore ds)
        {
            _dataStore = ds;
            _appInstanceId = _dataStore.AppSettings.AppIdentifier;
            _appPhrase = _dataStore.AppSettings.AppPhrase;
            _httpClient = new HttpClient {MaxResponseContentBufferSize = 256000};
        }

        public async Task<string> GetAppPhrase()
        {
            var uri = new Uri($"{_baseEndpointUrl}/GetAppPhrase/{_appInstanceId}");

            try
            {
                var response = await _httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var phrase = await response.Content.ReadAsStringAsync();
                    _dataStore.AppSettings.AppPhrase = phrase.Replace("\"", "");
                    _dataStore.SaveAppSettings();
                    return phrase;
                }
            }
            catch (Exception exp)
            {
                return null;
            }

            return null;
        }

        public async Task<string> UploadFormData(VoiceToFormUpload uploadData, bool deleteWhenFinished)
        {
            uploadData.AppInstanceId = _dataStore.AppSettings.AppIdentifier;
            uploadData.AppPhrase = _dataStore.AppSettings.AppPhrase;

            var url = new Uri($"{_baseEndpointUrl}/SaveFormData");
            var json = JsonConvert.SerializeObject(uploadData);
            var payload = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, payload);
                var result = await response.Content.ReadAsStringAsync();

                _dataStore.AppSettings.LastUploadConfirmation = result;
                _dataStore.AppSettings.DateLastUploadOccurred = DateTime.Now;
                _dataStore.SaveAppSettings();

                if (deleteWhenFinished)
                {
                    _dataStore.DeleteAllData();
                }
                else
                {
                    _dataStore.MarkAllDataAsUploaded();
                }

                return result;
            }
            catch (Exception exp)
            {
                return null;
            }
        }

       
    }

    public class VoiceToFormUpload
    {
        private UploadFormData _currentFormData = null;

        public VoiceToFormUpload()
        {
            this.FormData = new List<UploadFormData>();
        }

        public string AppInstanceId { get; set; }
        public string AppPhrase { get; set; }
        public bool ContainsData { get; private set; }

        public List<UploadFormData> FormData { get; set; }

        public int RowCount { get; private set; }

        public void AddRowToUpload(string formName, Dictionary<string, string> source)
        {
            if (_currentFormData == null || _currentFormData.FormName != formName)
            {
                _currentFormData = FormData.FirstOrDefault(c => c.FormName == formName);
                if (_currentFormData == null)
                {
                    _currentFormData = new UploadFormData()
                    {
                        FormName = formName
                    };

                    this.FormData.Add(_currentFormData);
                }
            }

            _currentFormData.Data.Add(source.Copy());
            RowCount += 1;
            this.ContainsData = true;
        }
    }

    public class UploadFormData
    {
        public UploadFormData()
        {
            this.Data = new List<Dictionary<string, string>>();
        }

        public string FormName { get; set; }
        public List<Dictionary<string, string>> Data { get; set; }
    }
}
