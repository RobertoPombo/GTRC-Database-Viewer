using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Media;

using GTRC_Basics;
using GTRC_Basics.Models.Common;
using GTRC_Database_Client;
using GTRC_Database_Viewer.Models;
using GTRC_WPF;
using GTRC_Basics.Models.DTOs;
using GTRC_WPF_UserControls.ViewModels;
using GTRC_Basics.Configs;
using GTRC_Database_Client.Requests;
using GTRC_Database_Client.Responses;

namespace GTRC_Database_Viewer.ViewModels
{
    public class DatabaseTableVM<ModelType> : ObservableObject where ModelType : class, IBaseModel, new()
    {
        private static readonly int noDbVersionNr = -1;
        private string pathJson = Directories.DbQuickBackup + typeof(ModelType).Name + ".json";
        private DbApiConnectionConfig? DbApiConnectionConfig;
        private SqlConnectionConfig? SqlConnectionConfig;
        private DbApiRequest<ModelType>? httpRequest;
        private ObservableCollection<DataRow<ModelType>> filteredList = [];
        private DataRow<ModelType>? current;
        private DataRow<ModelType>? selected;
        private ObservableCollection<string> dbVersionList = [];
        private string? dbVersion;
        private StateBackgroundWorker stateIdComparisonJson = StateBackgroundWorker.Off;
        private StateBackgroundWorker stateIdComparisonConvertedJson = StateBackgroundWorker.Off;
        private ObservableCollection<DatabaseFilter<ModelType>> filters = [];
        private int selectedId = GlobalValues.NoId;
        private SortState sortState = new();
        private bool allowFilter = true;

        public DatabaseTableVM()
        {
            if (!File.Exists(pathJson)) { WriteJson(); }
            foreach (PropertyInfo property in GlobalValues.DictDtoModels[typeof(ModelType)][DtoType.Full].GetProperties()) { Filters.Add(new(property)); }
            int newIndex = 0;
            for (int propertyIndex = 0; propertyIndex < Filters.Count; propertyIndex++)
            {
                foreach (PropertyInfo property in typeof(ModelType).GetProperties())
                {
                    if (Filters[propertyIndex].PropertyName == property.Name) { Filters.Move(propertyIndex, newIndex); newIndex++; break; }
                }
            }
            Filters.Add(new("Nr"));
            LoadDbVersionList();
            AddCmd = new UICmd(async (o) => await Add());
            DelCmd = new UICmd((o) => _ = Del());
            ClearCurrentCmd = new UICmd(async (o) => await ClearCurrent());
            UpdateCmd = new UICmd((o) => _ = Update());
            LoadSqlCmd = new UICmd((o) => _ = LoadSql());
            WriteSqlCmd = new UICmd((o) => _ = WriteSql());
            ClearSqlCmd = new UICmd((o) => _ = ClearSql());
            LoadJsonCmd = new UICmd((o) => LoadJson());
            WriteJsonCmd = new UICmd((o) => WriteJson());
            ClearJsonCmd = new UICmd((o) => ClearJson());
            ExportJsonCmd = new UICmd(async (o) => await ExportJson());
            LoadConvertedJsonCmd = new UICmd((o) => LoadConvertedJson());
            ExportConvertedJsonCmd = new UICmd(async (o) => await ExportConvertedJson());
            LoadDbVersionListCmd = new UICmd((o) => LoadDbVersionList());
            ClearFilterCmd = new UICmd((o) => ClearFilter());
            DbApiConnectionConfigVM.ConfirmApiConnectionEstablished += SetActiveDbApiConnection;
            SqlConnectionConfigVM.ConfirmSqlConnectionEstablished += SetActiveSqlConnection;
            DatabaseTableVM<GTRC_Basics.Models.Color>.PublishList += UpdateStateColors;
            GlobalWinValues.StateBackgroundWorkerColorsUpdated += RefreshStateColor;
        }

        public async void SetActiveDbApiConnection()
        {
            httpRequest = null;
            DbApiConnectionConfig = DbApiConnectionConfig.GetActiveConnection();
            if (DbApiConnectionConfig is not null) { httpRequest = new(DbApiConnectionConfig); }
            await ClearCurrent();
            _ = LoadSql();
        }

        public void SetActiveSqlConnection() { SqlConnectionConfig = SqlConnectionConfig.GetActiveConnection(); }

        public List<ModelType> ObjList { get; set; } = [];

        public ObservableCollection<DataRow<ModelType>> FilteredList { get { return filteredList; } set { filteredList = value; RaisePropertyChanged(); } }

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

        public ObservableCollection<string> DbVersionList
        {
            get { return dbVersionList; }
            set { dbVersionList = value; RaisePropertyChanged(); }
        }

        public string? DbVersion
        {
            get { return dbVersion; }
            set { dbVersion = value; RaisePropertyChanged(); _ = SetStateIdComparison(); }
        }

        public int DbVersionNr
        {
            get { if (int.TryParse(DbVersion?.ToLower().Replace("v", "").Split(" ")[0], out int versionNr)) { return versionNr; } else { return noDbVersionNr; } }
        }

        public string DbVersionName { get { return "Json v" + DbVersionNr.ToString(); } } // Sollte am besten im Frontend konvertiert werden

        public StateBackgroundWorker StateIdComparisonJson
        {
            get { return stateIdComparisonJson; }
            set
            {
                stateIdComparisonJson = value;
                RaisePropertyChanged(nameof(StateColorIdComparisonJson));
                if (MainVM.Instance?.DatabaseVM is not null)
                {
                    MainVM.Instance.DatabaseVM.StateColorIdComparisonJson = GlobalWinValues.ColorsStateBackgroundWorker[StateBackgroundWorker.RunWait];
                }
            }
        }

        public StateBackgroundWorker StateIdComparisonConvertedJson
        {
            get { return stateIdComparisonConvertedJson; }
            set
            {
                stateIdComparisonConvertedJson = value;
                RaisePropertyChanged(nameof(StateColorIdComparisonConvertedJson));
                if (MainVM.Instance?.DatabaseVM is not null)
                {
                    MainVM.Instance.DatabaseVM.StateColorIdComparisonConvertedJson = GlobalWinValues.ColorsStateBackgroundWorker[StateBackgroundWorker.RunWait];
                }
            }
        }

        public Brush StateColorIdComparisonJson
        {
            get { return GlobalWinValues.ColorsStateBackgroundWorker[StateIdComparisonJson]; }
        }

        public Brush StateColorIdComparisonConvertedJson
        {
            get { return GlobalWinValues.ColorsStateBackgroundWorker[StateIdComparisonConvertedJson]; }
        }

        public ObservableCollection<DatabaseFilter<ModelType>> Filters { get { return filters; } set { filters = value; RaisePropertyChanged(); } }

        public async Task Add()
        {
            if (httpRequest is not null && Current is not null)
            {
                Current.UpdateObject();
                AddDto<ModelType> dto = new();
                dto.Dto.Model2Dto(Current.Object);
                DbApiObjectResponse<ModelType> response = await httpRequest.Add(dto);
                if (response.Status == HttpStatusCode.OK) { await GetByUniqProps(Current.Object); }
                else if (response.Status == HttpStatusCode.NotFound) { _ = LoadSql(); }
                else if ((response.Status == HttpStatusCode.NotAcceptable || response.Status == HttpStatusCode.AlreadyReported))
                {
                    Current = new DataRow<ModelType>(response.Object, false);
                }
            }
        }

        public async Task Del()
        {
            if (httpRequest is not null)
            {
                HttpStatusCode response = await httpRequest.Delete(Selected?.Object.Id ?? GlobalValues.NoId, DatabaseVM.UseForceDelete());
                if (response == HttpStatusCode.OK) { _ = LoadSql(); }
            }
        }

        public async Task ClearCurrent()
        {
            Current = null;
            if (httpRequest is not null)
            {
                DbApiObjectResponse<ModelType> response = await httpRequest.GetTemp();
                if (response.Status == HttpStatusCode.OK) { Current = new DataRow<ModelType>(response.Object, false); }
            }
        }

        public async Task Update()
        {
            if (httpRequest is not null && Current is not null && Selected is not null)
            {
                Selected.UpdateObject();
                Current.UpdateObject();
                UpdateDto<ModelType> updateDto = new();
                updateDto.Dto.Model2Dto(Selected.Object);
                updateDto.Dto.Model2Dto(Current.Object);
                updateDto.Dto.Id = Selected.Object.Id;
                DbApiObjectResponse<ModelType> response = await httpRequest.Update(updateDto);
                if (response.Status == HttpStatusCode.OK) { await GetById(Selected.Object.Id); }
                else if ((response.Status == HttpStatusCode.NotAcceptable || response.Status == HttpStatusCode.AlreadyReported))
                {
                    Current = new DataRow<ModelType>(response.Object, false);
                }
                else { await ClearCurrent(); }
            }
        }

        public async Task GetById(int id)
        {
            if (httpRequest is not null)
            {
                DbApiObjectResponse<ModelType> response = await httpRequest.GetById(id);
                if (response.Status == HttpStatusCode.OK)
                {
                    for (int modelNr = 0; modelNr < ObjList.Count; modelNr++)
                    {
                        if (ObjList[modelNr].Id == id)
                        {
                            ObjList[modelNr] = response.Object;
                            _ = ResetLists(ObjList);
                            OnPublishList();
                            break;
                        }
                    }
                }
            }
        }

        public async Task GetByUniqProps(ModelType obj)
        {
            if (httpRequest is not null)
            {
                UniqPropsDto<ModelType> dto = new();
                dto.Dto.Model2Dto(obj);
                DbApiObjectResponse<ModelType> response = await httpRequest.GetByUniqProps(dto);
                if (response.Status == HttpStatusCode.OK)
                {
                    if (ObjList.Contains(obj)) { ObjList[ObjList.IndexOf(obj)] = response.Object; }
                    else { ObjList.Add(response.Object); }
                    _ = ResetLists(ObjList);
                    OnPublishList();
                }
                else { _ = LoadSql(); }
            }
        }

        public async Task LoadSql()
        {
            if (httpRequest is not null)
            {
                DbApiListResponse<ModelType> response = await httpRequest.GetAll();
                if (response.Status == HttpStatusCode.OK) { _ = ResetLists(response.List); }
                else { _ = ResetLists([]); }
            }
            else { _ = ResetLists([]); }
            OnPublishList();
        }

        public async Task WriteSql(bool keepValue = false)
        {
            if (httpRequest is not null)
            {
                List<ModelType> oldList = [];
                DbApiListResponse<ModelType> response = await httpRequest.GetAll();
                if (response.Status == HttpStatusCode.OK) { oldList = response.List; }
                foreach (ModelType oldObj in oldList)
                {
                    bool found = false;
                    foreach (ModelType newObj in ObjList) { if (newObj.Id == oldObj.Id) { found = true; break; } }
                    if (!found) { await httpRequest.Delete(oldObj.Id, DatabaseVM.UseForceDelete(true)); }
                }
                foreach (ModelType newObj in ObjList)
                {
                    bool found = false;
                    foreach (ModelType oldObj in oldList) { if (newObj.Id == oldObj.Id) { found = true; break; } }
                    if (found)
                    {
                        UpdateDto<ModelType> updateDto = new();
                        updateDto.Dto.Model2Dto(newObj);
                        DbApiObjectResponse<ModelType> updateResponse = await httpRequest.Update(updateDto);
                        if (updateResponse.Status == HttpStatusCode.NotAcceptable)
                        {
                            updateDto.Dto.Model2Dto(updateResponse.Object);
                            await httpRequest.Update(updateDto);
                        }
                    }
                    else { int deletedId = GlobalValues.NoId; while (deletedId < newObj.Id) { deletedId = await AddSqlForceId(newObj); } }
                }
                _ = LoadSql();
                DatabaseVM.UseForceDelete(keepValue);
                DatabaseVM.UseForceSameId(keepValue);
            }
        }

        public async Task ClearSql()
        {
            if (httpRequest is not null)
            {
                foreach (ModelType obj in ObjList) { await httpRequest.Delete(obj.Id, DatabaseVM.UseForceDelete(true)); }
                if (DatabaseVM.UseForceReseed(true))
                {
                    if (SqlConnectionConfig?.Connectivity() ?? false) { SqlConnectionConfig.Reseed(typeof(ModelType)); }
                    DatabaseVM.UseForceReseed();
                }
                DatabaseVM.UseForceDelete();
            }
            _ = LoadSql();
        }

        public void LoadJson()
        {
            allowFilter = false;
            _ = ResetLists(GetJsonList());
            allowFilter = true;
            OnPublishList();
        }

        public void WriteJson()
        {
            File.WriteAllText(pathJson, JsonConvert.SerializeObject(ObjList, Formatting.Indented), Encoding.Unicode);
            _ = SetStateIdComparison();
        }

        public void ClearJson()
        {
            File.WriteAllText(pathJson, JsonConvert.SerializeObject(new List<ModelType>(), Formatting.Indented), Encoding.Unicode);
            LoadJson();
        }

        public async Task ExportJson(bool keepValue = false, bool waitForWriteSql = false)
        {
            if (DatabaseVM.UseForceReseed(true) && (SqlConnectionConfig?.Connectivity() ?? false) && httpRequest is not null)
            {
                foreach (ModelType obj in ObjList) { await httpRequest.Delete(obj.Id, DatabaseVM.UseForceDelete(true)); }
                SqlConnectionConfig.Reseed(typeof(ModelType));
            }
            ObjList = GetJsonList();
            if (waitForWriteSql) { await WriteSql(keepValue); } else { _ = WriteSql(keepValue); }
            DatabaseVM.UseForceReseed(keepValue);
        }

        public void LoadConvertedJson()
        {
            allowFilter = false;
            _ = ResetLists(GetConvertedJsonList());
            allowFilter = true;
            OnPublishList();
        }

        public async Task ExportConvertedJson(bool keepValue = false, bool waitForWriteSql = false)
        {
            if (DatabaseVM.UseForceReseed(true) && (SqlConnectionConfig?.Connectivity() ?? false) && httpRequest is not null)
            {
                foreach (ModelType obj in ObjList) { await httpRequest.Delete(obj.Id, DatabaseVM.UseForceDelete(true)); }
                SqlConnectionConfig.Reseed(typeof(ModelType));
            }
            ObjList = GetConvertedJsonList();
            if (waitForWriteSql) { await WriteSql(keepValue); } else { _ = WriteSql(keepValue); }
            DatabaseVM.UseForceReseed(keepValue);
        }

        public void ClearFilter()
        {
            allowFilter = false;
            bool isFiltered = false;
            for (int filterNr = Filters.Count - 1; filterNr >= 0; filterNr--) { if (Filters[filterNr].Filter.Length > 0) { isFiltered = true; break; } }
            if (isFiltered) { for (int filterNr = Filters.Count - 1; filterNr >= 0; filterNr--) { Filters[filterNr].Filter = ""; } }
            else if (Filters.Count > 0) { Filters[0].Filter = DatabaseFilter<ModelType>.NoIdFilter; }
            allowFilter = true;
            _ = FilterList();
        }

        public async Task ResetLists(List<ModelType> _list, int index = 0)
        {
            ObjList = _list;
            _ = SetStateIdComparison();
            await FilterList(index);
        }

        public async Task FilterList(int index = 0)
        {
            filteredList.Clear();
            if (allowFilter && httpRequest is not null)
            {
                DbApiListResponse<ModelType> response = await httpRequest.GetByFilter(DatabaseFilter<ModelType>.GetFilterDtos(Filters));
                if (response.Status == HttpStatusCode.OK)
                {
                    for (int objNr = 0; objNr < response.List.Count; objNr++) { FilteredList.Add(new DataRow<ModelType>(response.List[objNr], true, objNr + 1)); }
                }
            }
            else if (!allowFilter)
            {
                for (int objNr = 0; objNr < ObjList.Count; objNr++)
                {
                    FilteredList.Add(new DataRow<ModelType>(ObjList[objNr], true, objNr + 1));
                }
            }
            Selected = SetSelected(index);
        }

        public void SortFilteredList(PropertyInfo property)
        {
            bool stringCompare = true;
            if (GlobalValues.numericalTypes.Contains(property.PropertyType)) { stringCompare = false; }
            if (sortState.Property == property) { sortState.SortAscending = !sortState.SortAscending; }
            else { sortState = new SortState { Property = property }; }
            for (int rowNr1 = 0; rowNr1 < FilteredList.Count - 1; rowNr1++)
            {
                for (int rowNr2 = rowNr1 + 1; rowNr2 < FilteredList.Count; rowNr2++)
                {
                    bool isAscending;
                    dynamic? val1 = Scripts.GetCastedValue(Mapper<ModelType>.Model2FullDto(FilteredList[rowNr1].Object), property);
                    dynamic? val2 = Scripts.GetCastedValue(Mapper<ModelType>.Model2FullDto(FilteredList[rowNr2].Object), property);
                    if (stringCompare) { isAscending = String.Compare(val1?.ToString(), val2?.ToString()) <= 0; }
                    else { isAscending = val1 <= val2; }
                    if (sortState.SortAscending != isAscending)
                    {
                        (FilteredList[rowNr1], FilteredList[rowNr2]) = (FilteredList[rowNr2], FilteredList[rowNr1]);
                        (FilteredList[rowNr1].Nr, FilteredList[rowNr2].Nr) = (FilteredList[rowNr2].Nr, FilteredList[rowNr1].Nr);
                    }
                }
            }
            RaisePropertyChanged(nameof(FilteredList));
        }

        public DataRow<ModelType>? SetSelected(int index = 0)
        {
            index = Math.Min(Math.Max(0, index), FilteredList.Count - 1);
            if (index == 0 && selectedId != GlobalValues.NoId)
            {
                foreach (DataRow<ModelType> dataRow in FilteredList) { if (dataRow.Object.Id == selectedId) { return dataRow; } }
            }
            if (index >= 0 && index < FilteredList.Count) { return FilteredList[index]; }
            if (FilteredList.Count > 0) { return FilteredList[0]; }
            return null;
        }

        public void LoadDbVersionList()
        {
            dbVersionList = [];
            string fileNameToFind = typeof(ModelType).Name + ".json";
            foreach (string directoryPath in Directory.EnumerateDirectories(Directories.DbMigrations))
            {
                dbVersion = directoryPath.Split('\\')[^1];
                if (DbVersionNr > noDbVersionNr)
                {
                    foreach (string filePath in Directory.EnumerateFiles(directoryPath))
                    {
                        string fileName = filePath.Split('\\')[^1];
                        if (fileName == fileNameToFind) { dbVersionList.Add(dbVersion); break; }
                    }
                }
            }
            RaisePropertyChanged(nameof(DbVersionList));
            if (DbVersionList.Count == 0) { _ = SetStateIdComparison(); }
            else { dbVersion = null; DbVersion = DbVersionList[^1]; _ = GetConvertedJsonList(); }
        }

        public List<ModelType> GetJsonList()
        {
            return JsonConvert.DeserializeObject<List<ModelType>>(File.ReadAllText(pathJson, Encoding.Unicode)) ?? [];
        }

        public List<ModelType> GetConvertedJsonList()
        {
            List<ModelType> newList = [];
            if (dbVersion is not null && DbVersionNr > noDbVersionNr && MainVM.DictOldDbVersionModels[typeof(ModelType)].Count > DbVersionNr)
            {
                string path = Directories.DbMigrations + dbVersion + "\\" + typeof(ModelType).Name + ".json";
                if (File.Exists(path))
                {
                    List<object> oldList = JsonConvert.DeserializeObject<List<object>>(File.ReadAllText(path, Encoding.Unicode)) ?? [];
                    foreach (IEnumerable<KeyValuePair<string, JToken>> oldObj in oldList.Cast<IEnumerable<KeyValuePair<string, JToken>>>())
                    {
                        dynamic newObj = Activator.CreateInstance(MainVM.DictOldDbVersionModels[typeof(ModelType)][DbVersionNr])!;
                        foreach (PropertyInfo newProperty in newObj.GetType().GetProperties())
                        {
                            foreach (KeyValuePair<string, JToken> oldProperty in oldObj)
                            {
                                if (oldProperty.Key.Replace("_", "").Equals(newProperty.Name.Replace("_", ""), StringComparison.CurrentCultureIgnoreCase))
                                {
                                    var newValue = Scripts.CastValue(newProperty, oldProperty.Value);
                                    if (newValue is not null && newValue.GetType() == newProperty.PropertyType) { newProperty.SetValue(newObj, newValue); }
                                    break;
                                }
                            }
                        }
                        newList.Add(Scripts.Map(newObj, new ModelType()));
                    }
                }
            }
            return newList;
        }

        public async Task SetStateIdComparison()
        {
            StateIdComparisonJson = StateBackgroundWorker.Off;
            StateIdComparisonConvertedJson = StateBackgroundWorker.Off;
            
            if (!DatabaseVM.IsAllowedIdComparison()) { return; }
            
            if (DbVersion is null || httpRequest is null) { return; }
            List<ModelType> listJson = GetJsonList();
            bool allJsonInSql = await AllJsonInSql(listJson);
            if (allJsonInSql) { StateIdComparisonJson = StateBackgroundWorker.Run; }
            else { StateIdComparisonJson = StateBackgroundWorker.Wait; }

            if (DbVersion is null || httpRequest is null) { return; }
            List<ModelType> listConvertedJson = GetConvertedJsonList();
            bool allConvertedJsonInSql = await AllJsonInSql(listConvertedJson);
            if (allConvertedJsonInSql) { StateIdComparisonConvertedJson = StateBackgroundWorker.Run; }
            else { StateIdComparisonConvertedJson = StateBackgroundWorker.Wait; }

            if (DbVersion is null || httpRequest is null) { return; }
            DbApiListResponse<ModelType> getAllResponse = await httpRequest.GetAll();
            if (getAllResponse.Status == HttpStatusCode.OK)
            {
                bool allSqlInJson = AllSqlInJson(getAllResponse.List, listJson);
                if (!allSqlInJson && allJsonInSql) { StateIdComparisonJson = StateBackgroundWorker.On; }

                bool allSqlInConvertedJson = AllSqlInJson(getAllResponse.List, listConvertedJson);
                if (!allSqlInConvertedJson && allConvertedJsonInSql) { StateIdComparisonConvertedJson = StateBackgroundWorker.On; }
            }
        }

        public void RefreshStateColor() { RaisePropertyChanged(nameof(StateIdComparisonJson)); RaisePropertyChanged(nameof(StateIdComparisonConvertedJson)); }

        public async Task<bool> AllJsonInSql(List<ModelType> list)
        {
            foreach (ModelType obj in list)
            {
                UniqPropsDto<ModelType> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);

                if (DbVersion is null || httpRequest is null) { return false; }
                DbApiObjectResponse<ModelType> getResponse = await httpRequest.GetByUniqProps(uniqPropsDto);
                if (getResponse.Status != HttpStatusCode.OK || getResponse.Object.Id != obj.Id) { return false; }
            }
            return true;
        }

        public bool AllSqlInJson(List<ModelType> listSql, List<ModelType> listJson)
        {
            foreach (ModelType objSql in listSql)
            {
                bool isInListJson = false;
                foreach (ModelType objJson in listJson) { if (objSql.Id == objJson.Id) { isInListJson = true; } }
                if (!isInListJson) { return false; }
            }
            return true;
        }

        public void UpdateStateColors() { _ = SetStateIdComparison(); }

        public async Task<int> AddSqlForceId(ModelType obj)
        {
            if (httpRequest is not null)
            {
                AddDto<ModelType> addDto = new();
                addDto.Dto.Model2Dto(obj);
                DbApiObjectResponse<ModelType> addResponse = await httpRequest.Add(addDto);
                if (addResponse.Status == HttpStatusCode.NotAcceptable)
                {
                    addDto.Dto.Model2Dto(addResponse.Object);
                    addResponse = await httpRequest.Add(addDto);
                }
                if (addResponse.Status == HttpStatusCode.OK && DatabaseVM.UseForceSameId(true))
                {
                    UniqPropsDto<ModelType> uniqPropsDto = new();
                    uniqPropsDto.Dto.Model2Dto(obj);
                    DbApiObjectResponse<ModelType> getResponse = await httpRequest.GetByUniqProps(uniqPropsDto);
                    if (getResponse.Status == HttpStatusCode.OK && getResponse.Object.Id != obj.Id)
                    {
                        HttpStatusCode deleteResponse = await httpRequest.Delete(getResponse.Object.Id, true);
                        if (deleteResponse == HttpStatusCode.OK) { return getResponse.Object.Id; }
                    }
                }
            }
            return int.MaxValue;
        }


        public static event Notify? PublishList;

        public static void OnPublishList() { PublishList?.Invoke(); }

        public UICmd AddCmd { get; set; }
        public UICmd DelCmd { get; set; }
        public UICmd ClearCurrentCmd { get; set; }
        public UICmd UpdateCmd { get; set; }
        public UICmd LoadSqlCmd { get; set; }
        public UICmd WriteSqlCmd { get; set; }
        public UICmd ClearSqlCmd { get; set; }
        public UICmd LoadJsonCmd { get; set; }
        public UICmd WriteJsonCmd { get; set; }
        public UICmd ClearJsonCmd { get; set; }
        public UICmd ExportJsonCmd { get; set; }
        public UICmd LoadConvertedJsonCmd { get; set; }
        public UICmd ExportConvertedJsonCmd { get; set; }
        public UICmd LoadDbVersionListCmd { get; set; }
        public UICmd ClearFilterCmd { get; set; }
    }
}
