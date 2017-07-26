using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.PlatformInterfaces;
using VoiceToForm.Services;
using VoiceToForm.Utils;
using Xamarin.Forms;

namespace VoiceToForm.Model
{
    public class DataStore
    {
        private IFileHelper _file = DependencyService.Get<IFileHelper>();
        private V2FSettings _appSettings = new V2FSettings();

        public DataStore()
        {
            GetFormDefinitionsAsync();
            ReadFile(_appSettings);
        }

        public async Task SaveFormDefinitionAsync(Form formDefinition)
        {
            SaveFile(formDefinition);
        }

        public async Task DeleteFormAsync(string formName)
        {
            var frm = await GetForm(formName);
           
            if (frm != null)
            {
                var formData = new FormData(frm);

                DeleteFile(frm);
                DeleteFile(formData);
            }
        }

        public async Task<bool> CheckIfDataIsAvailableToUploadAsync()
        {
            var formDefintions = await this.GetFormDefinitionsAsync();

            foreach (var formDef in formDefintions)
            {
                var formData = await this.GetFormDataAsync(formDef.FormName);
              
                if (formData.ContainsDataToUpload)
                {
                    return true;
                }
            }

            return false;
        }

        public void DeleteAllData()
        {
            var dataFiles = _file.GetFileNamesByExtension(_file.GetLocalFilePath(""), ".data");
            foreach (var fileToDelete in dataFiles)
            {
                _file.DeleteFile(fileToDelete);
            }

        }

        public async Task<VoiceToFormUpload> GetDataToUpload()
        {
            VoiceToFormUpload result =  new VoiceToFormUpload();
            var forms = await this.GetFormDefinitionsAsync();
            foreach (var formDef in forms)
            {
                var dataFile = await this.GetFormDataAsync(formDef.FormName);
                if (dataFile != null)
                {
                    foreach (var row in dataFile.Rows)
                    {
                        if (row.WasNotUploaded())
                        {
                            result.AddRowToUpload(formDef.FormName, row);
                        }
                    }
                }
            }

            return result;
        }

        public async Task MarkAllDataAsUploaded()
        {
            var forms = await this.GetFormDefinitionsAsync();
            foreach (var f in forms)
            {
                var dataFile = await this.GetFormDataAsync(f.FormName);
                foreach (var r in dataFile.Rows)
                {
                    r.SetDateUploaded();
                }

                SaveFile(dataFile);
            }
        }

        public async Task SaveFormDataAsync(FormData formNewData)
        {
            if (formNewData.Rows.Count == 0)
            {
                DeleteFile(formNewData);
            }
            else
            {
                SaveFile(formNewData);
            }
        }

        public V2FSettings AppSettings
        {
            get
            {
                return _appSettings;
            }
        }

        public void SaveAppSettings()
        {
            SaveFile(_appSettings);
        }

        public bool DataFilesExist => GetFileNamesByExtension(".data").Length > 0;

        public async Task<List<Form>> GetFormDefinitionsAsync()
        {
            var forms = new List<Form>();

            var folder = _file.GetLocalFilePath("");
            var formDefinitions = _file.GetFileNamesByExtension(_file.GetLocalFilePath(""), ".def");

            foreach (var formDefinitionPath in formDefinitions)
            {
                if (formDefinitionPath.IsNotNullOrEmpty())
                {
                    Form form = new Form() {FormId = formDefinitionPath.Remove(folder, "/", ".def").Trim()};
                    ReadFile(form);
                    form.HasData = CheckIfFormHasData(form);
                    forms.Add(form);
                }
            }

            return forms;
        }

        public async Task<FormData> GetFormDataAsync(string formName)
        {
            var form = (await GetFormDefinitionsAsync()).FirstOrDefault(f => f.FormName == formName);
            if (form == null)
            {
                throw new Exception($"Unable to find form {formName}");
            }

            FormData result = new FormData(form);
            ReadFile(result);

            return result;
        }


        public async Task<string> CreateAppInstanceIdAsync()
        {
            if (_appSettings.AppIdentifier.IsNull())
            {
                var g1 = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 4);
                var g2 = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 4);
                _appSettings.AppIdentifier = $"{g1}-{g2}";
                SaveFile(_appSettings);
                return _appSettings.AppIdentifier;
            }

            return _appSettings.AppIdentifier;
        }

        public void InitializeNewInstallation()
        {
            var files = GetFileNamesByExtension(".data", ".def", ".setting");
            DeleteFiles(files);
            _appSettings = new V2FSettings();
        }

        private void DeleteFiles(string[] files)
        {
            foreach (var f in files)
            {
                _file.DeleteFile(f);
            }
        }

        private string[] GetFileNamesByExtension(params string[] extensions)
        {
            List<string> result = new List<string>();

            foreach (var extension in extensions)
            {
                var files = _file.GetFileNamesByExtension(_file.GetLocalFilePath(""), extension).ToList();
                result.AddRange(files);
            }

            return result.ToArray();
        }

        private bool CheckIfFormHasData(Form theForm)
        {
            FormData fData = new FormData(theForm);
            var path = _file.GetLocalFilePath(fData.DataKey.Replace(" ", "_") + fData.Extension);

            return _file.Exists(path);
        }
        private void SaveFile(IStringable obj)
        {
            var contents = obj.Serialize();
            var path = _file.GetLocalFilePath(obj.DataKey.Replace(" ", "_") + obj.Extension);

            _file.WriteAllText(contents, path);
        }

        public async Task<Form> GetForm(string lookFor)
        {
            var forms = await this.GetFormDefinitionsAsync();
            Form result = null;

            if (lookFor.IsGuid())
            {
                result = forms?.FirstOrDefault(f => f.FormId == lookFor);
            }
            else
            {
                result = forms?.FirstOrDefault(f => f.FormName.Equals(lookFor, StringComparison.OrdinalIgnoreCase));
            }
            return result;
        }

        private void ReadFile(IStringable obj)
        {
            var path = _file.GetLocalFilePath(obj.DataKey.Replace(" ", "_") + obj.Extension);
            if (_file.Exists(path))
            {
                var contents = _file.ReadAllText(path);
                obj.Deserialize(contents);
            }
        }

        private void DeleteFile(IStringable obj)
        {
            var path = _file.GetLocalFilePath(obj.DataKey.Replace(" ", "_") + obj.Extension);
            if (_file.Exists(path))
            {
                _file.DeleteFile(path);
            }
        }
    }
}
