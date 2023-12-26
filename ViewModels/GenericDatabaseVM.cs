using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Net;
using System.Text;

using GTRC_Basics;
using GTRC_Basics.Models.Common;
using GTRC_Database_Client;
using GTRC_Database_Viewer.Models;
using GTRC_WPF;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_Viewer.ViewModels
{
    public class GenericDatabaseVM<ModelType> : ObservableObject where ModelType : class, IBaseModel, new()
    {
        private string pathJson = GlobalValues.DataDirectory + typeof(ModelType).Name.ToLower() + ".json";
        private ConnectionSettings? ConnectionSettings;
        private HttpRequest<ModelType>? httpRequest;
        private ObservableCollection<DataRow<ModelType>> list = [];
        private DataRow<ModelType>? current;
        private DataRow<ModelType>? selected;
        private bool forceDelete = false;
        private int selectedId = GlobalValues.NoId;

        public GenericDatabaseVM()
        {
            if (!File.Exists(pathJson)) { WriteJson(); }
            AddCmd = new UICmd(async (o) => await Add());
            DelCmd = new UICmd(async (o) => await Del());
            ClearCurrentCmd = new UICmd(async (o) => await ClearCurrent());
            UpdateCmd = new UICmd(async (o) => await Update());
            LoadSqlCmd = new UICmd(async (o) => await LoadSql());
            WriteSqlCmd = new UICmd(async (o) => await WriteSql());
            LoadJsonCmd = new UICmd((o) => LoadJson());
            WriteJsonCmd = new UICmd((o) => WriteJson());
            ClearSqlCmd = new UICmd(async (o) => await ClearSql());
            ClearJsonCmd = new UICmd((o) => ClearJson());
            ClearFilterCmd = new UICmd((o) => ClearFilter());
            ClientConnectionSettingsVM.ConfirmApiConnectionEstablished += Initialize;
        }

        public async void Initialize()
        {
            httpRequest = null;
            ConnectionSettings = ConnectionSettings.GetActiveConnectionSettings();
            if (ConnectionSettings is not null) { httpRequest = new(ConnectionSettings); }
            await ClearCurrent();
            await LoadSql();
        }

        public ObservableCollection<DataRow<ModelType>> List
        {
            get { return list; }
            set { list = value; RaisePropertyChanged(); }
        }

        public List<ModelType> ObjList { get { List<ModelType> list = []; foreach (DataRow<ModelType> row in List) { list.Add(row.Object); } return list; } }

        public DataRow<ModelType>? Current
        {
            get { return current; }
            set { current = value; RaisePropertyChanged(); }
        }

        public DataRow<ModelType>? Selected
        {
            get { return selected; }
            set
            {
                if (value != null)
                {
                    selected = value;
                    selectedId = selected.Object.Id;
                    Current = new DataRow<ModelType>(selected.Object, false);
                    RaisePropertyChanged();
                }
            }
        }

        public bool ForceDelete
        {
            get { return forceDelete; }
            set { forceDelete = value; RaisePropertyChanged(); }
        }

        public async Task Add()
        {
            if (httpRequest is not null && Current is not null)
            {
                AddDto<ModelType> addDto = new();
                DataRow2Object(Current.Object, Current);
                addDto.Dto.ReMap(Current.Object);
                Tuple<HttpStatusCode, ModelType?> response = await httpRequest.Add(addDto);
                if (response.Item1 == HttpStatusCode.OK) { await LoadSql(); }
                else if (response.Item1 == HttpStatusCode.Conflict && response.Item2 is not null) { Current = new DataRow<ModelType>(response.Item2, false); }
            }
        }

        public async Task Del()
        {
            if (httpRequest is not null)
            {
                HttpStatusCode response = await httpRequest.Delete(Selected?.Object?.Id ?? GlobalValues.NoId, UseForceDel());
                if (response == HttpStatusCode.OK) { await LoadSql(); }
            }
        }

        public async Task ClearCurrent()
        {
            Current = null;
            if (httpRequest is not null)
            {
                Tuple<HttpStatusCode, ModelType?> response = await httpRequest.GetTemp();
                if (response.Item1 == HttpStatusCode.OK && response.Item2 is not null) { Current = new DataRow<ModelType>(response.Item2, false); }
            }
        }

        public async Task Update()
        {
            if (httpRequest is not null && Current is not null && Selected is not null)
            {
                UpdateDto<ModelType> updateDto = new();
                DataRow2Object(Selected.Object, Selected);
                DataRow2Object(Current.Object, Current);
                updateDto.Dto.ReMap(Selected.Object);
                updateDto.Dto.ReMap(Current.Object);
                Tuple<HttpStatusCode, ModelType?> response = await httpRequest.Update(updateDto);
                if (response.Item1 == HttpStatusCode.OK) { await LoadSql(); }
                else if (response.Item1 == HttpStatusCode.Conflict && response.Item2 is not null) { Current = new DataRow<ModelType>(response.Item2, false); }
                else if (response.Item1 == HttpStatusCode.NotFound) { await ClearCurrent(); }
            }
        }

        public async Task LoadSql()
        {
            if (httpRequest is not null)
            {
                Tuple<HttpStatusCode, List<ModelType>> response = await httpRequest.GetAll();
                if (response.Item1 == HttpStatusCode.OK) { ResetList(response.Item2); }
                else { ResetList([]); }
            }
            else { ResetList([]); }
            OnPublishList();
        }

        public async Task WriteSql()
        {
            if (httpRequest is not null)
            {
                List<ModelType> oldList = [];
                Tuple<HttpStatusCode, List<ModelType>> response = await httpRequest.GetAll();
                if (response.Item1 == HttpStatusCode.OK) { oldList = response.Item2; }
                foreach (ModelType newObj in ObjList)
                {
                    bool found = false;
                    foreach (ModelType oldObj in oldList) { if (newObj.Id == oldObj.Id) { found = true; break; } }
                    if (found) { UpdateDto<ModelType> updateDto = new(); updateDto.Dto.Map(newObj); await httpRequest.Update(updateDto); }
                    else { AddDto<ModelType> addDto = new(); addDto.Dto.Map(newObj); await httpRequest.Add(addDto); }
                }
                foreach (ModelType oldObj in oldList)
                {
                    bool found = false;
                    foreach (ModelType newObj in ObjList) { if (newObj.Id == oldObj.Id) { found = true; break; } }
                    if (!found) { await httpRequest.Delete(oldObj.Id, UseForceDel()); }
                }
                await LoadSql();
            }
        }

        public void LoadJson()
        {
            ResetList(JsonConvert.DeserializeObject<List<ModelType>>(File.ReadAllText(pathJson, Encoding.Unicode)) ?? []);
            OnPublishList();
        }

        public void WriteJson()
        {
            File.WriteAllText(pathJson, JsonConvert.SerializeObject(ObjList, Formatting.Indented), Encoding.Unicode);
            OnPublishList();
        }

        public async Task ClearSql()
        {
            if (httpRequest is not null) { foreach (DataRow<ModelType> row in List) { await httpRequest.Delete(row.Object.Id, UseForceDel()); } }
            await LoadSql();
        }

        public void ClearJson()
        {
            File.WriteAllText(pathJson, JsonConvert.SerializeObject(new List<ModelType>(), Formatting.Indented), Encoding.Unicode);
            LoadJson();
        }

        public void ClearFilter()
        {

        }

        public bool UseForceDel() { if (ForceDelete) { ForceDelete = false; return true; } else { return false; } }


        public void ResetList(List<ModelType> _list, int index = 0)
        {
            List.Clear();
            for (int objNr = 0; objNr < _list.Count; objNr++) { List.Add(new DataRow<ModelType>(_list[objNr], true, objNr + 1)); }
            Selected = SetSelected(index);
        }

        public DataRow<ModelType>? SetSelected(int index = 0)
        {
            index = Math.Min(Math.Max(0, index), List.Count - 1);
            if (index == 0 && selectedId != GlobalValues.NoId)
            {
                foreach (DataRow<ModelType> dataRow in List) { if (dataRow.Object.Id == selectedId) { return dataRow; } }
            }
            if (index >= 0 && index < List.Count) { return List[index]; }
            if (List.Count > 0) { return List[0]; }
            return null;
        }

        public static void DataRow2Object(dynamic _obj, DataRow<ModelType> dataRow)
        {
            foreach (DataField<ModelType> dataField in dataRow.List) { dataField.Property?.SetValue(_obj, Scripts.CastValue(dataField.Property, dataField.Value)); }
        }

        public static event Notify? PublishList;

        public static void OnPublishList() { PublishList?.Invoke(); }

        public UICmd AddCmd { get; set; }
        public UICmd DelCmd { get; set; }
        public UICmd ClearCurrentCmd { get; set; }
        public UICmd UpdateCmd { get; set; }
        public UICmd LoadJsonCmd { get; set; }
        public UICmd WriteJsonCmd { get; set; }
        public UICmd LoadSqlCmd { get; set; }
        public UICmd WriteSqlCmd { get; set; }
        public UICmd ClearSqlCmd { get; set; }
        public UICmd ClearJsonCmd { get; set; }
        public UICmd ClearFilterCmd { get; set; }
    }
}
