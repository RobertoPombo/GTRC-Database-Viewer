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
using GTRC_Database_API.Helpers;

namespace GTRC_Database_Viewer.ViewModels
{
    public class DatabaseTableVM<ModelType> : ObservableObject where ModelType : class, IBaseModel, new()
    {
        private static readonly int noDbVersionNr = -1;
        private string pathJson = GlobalValues.DataDirectory + typeof(ModelType).Name.ToLower() + ".json";
        private DbApiConnectionConfig? DbApiConnectionConfig;
        private SqlConnectionConfig? SqlConnectionConfig;
        private HttpRequest<ModelType>? httpRequest;
        private ObservableCollection<DataRow<ModelType>> filteredList = [];
        private DataRow<ModelType>? current;
        private DataRow<ModelType>? selected;
        private ObservableCollection<string> dbVersionList = [];
        private string? dbVersion;
        private Brush stateIdComparisonJson = GlobalWinValues.StateOff;
        private Brush stateIdComparisonConvertedJson = GlobalWinValues.StateOff;
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
            ClearSqlCmd = new UICmd((o) => ClearSql());
            LoadJsonCmd = new UICmd((o) => LoadJson());
            WriteJsonCmd = new UICmd((o) => WriteJson());
            ClearJsonCmd = new UICmd((o) => ClearJson());
            ExportJsonCmd = new UICmd((o) => ExportJson());
            LoadConvertedJsonCmd = new UICmd((o) => LoadConvertedJson());
            ExportConvertedJsonCmd = new UICmd((o) => ExportConvertedJson());
            LoadDbVersionListCmd = new UICmd((o) => LoadDbVersionList());
            ClearFilterCmd = new UICmd((o) => ClearFilter());
            DbApiConnectionConfigVM.ConfirmApiConnectionEstablished += SetActiveDbApiConnection;
            SqlConnectionConfigVM.ConfirmSqlConnectionEstablished += SetActiveSqlConnection;
            DatabaseTableVM<GTRC_Basics.Models.Color>.PublishList += UpdateStateColors;
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

        public string DbVersionName { get { return "Json V" + DbVersionNr.ToString(); } } // Sollte am besten im Frontend konvertiert werden

        public Brush StateIdComparisonJson
        {
            get { return stateIdComparisonJson; }
            set
            {
                stateIdComparisonJson = value;
                RaisePropertyChanged();
                if (MainVM.Instance?.DatabaseVM is not null) { MainVM.Instance.DatabaseVM.StateIdComparisonJson = GlobalWinValues.StateRunWait; }
            }
        }

        public Brush StateIdComparisonConvertedJson
        {
            get { return stateIdComparisonConvertedJson; }
            set
            {
                stateIdComparisonConvertedJson = value;
                RaisePropertyChanged();
                if (MainVM.Instance?.DatabaseVM is not null) { MainVM.Instance.DatabaseVM.StateIdComparisonConvertedJson = GlobalWinValues.StateRunWait; }
            }
        }

        public ObservableCollection<DatabaseFilter<ModelType>> Filters { get { return filters; } set { filters = value; RaisePropertyChanged(); } }

        public async Task Add()
        {
            if (httpRequest is not null && Current is not null)
            {
                Current.UpdateObject();
                AddDto<ModelType> dto = new();
                dto.Dto.Model2Dto(Current.Object);
                Tuple<HttpStatusCode, ModelType?> response = await httpRequest.Add(dto);
                if (response.Item1 == HttpStatusCode.OK) { await GetByUniqProps(Current.Object); }
                else if (response.Item1 == HttpStatusCode.NotFound) { _ = LoadSql(); }
                else if (response.Item1 == HttpStatusCode.Conflict && response.Item2 is not null) { Current = new DataRow<ModelType>(response.Item2, false); }
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
                Tuple<HttpStatusCode, ModelType?> response = await httpRequest.GetTemp();
                if (response.Item1 == HttpStatusCode.OK && response.Item2 is not null) { Current = new DataRow<ModelType>(response.Item2, false); }
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
                Tuple<HttpStatusCode, ModelType?> response = await httpRequest.Update(updateDto);
                if (response.Item1 == HttpStatusCode.OK) { await GetById(Selected.Object.Id); }
                else if (response.Item1 == HttpStatusCode.Conflict && response.Item2 is not null) { Current = new DataRow<ModelType>(response.Item2, false); }
                else if (response.Item1 == HttpStatusCode.NotFound || response.Item1 == HttpStatusCode.InternalServerError) { await ClearCurrent(); }
            }
        }

        public async Task GetById(int id)
        {
            if (httpRequest is not null)
            {
                Tuple<HttpStatusCode, ModelType?> response = await httpRequest.GetById(id);
                if (response.Item1 == HttpStatusCode.OK && response.Item2 is not null)
                {
                    for (int modelNr = 0; modelNr < ObjList.Count; modelNr++)
                    {
                        if (ObjList[modelNr].Id == id)
                        {
                            ObjList[modelNr] = response.Item2;
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
                Tuple<HttpStatusCode, ModelType?> response = await httpRequest.GetByUniqProps(dto);
                if (response.Item1 == HttpStatusCode.OK && response.Item2 is not null)
                {
                    if (ObjList.Contains(obj)) { ObjList[ObjList.IndexOf(obj)] = response.Item2; }
                    else { ObjList.Add(response.Item2); }
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
                Tuple<HttpStatusCode, List<ModelType>> response = await httpRequest.GetAll();
                if (response.Item1 == HttpStatusCode.OK) { _ = ResetLists(response.Item2); }
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
                Tuple<HttpStatusCode, List<ModelType>> response = await httpRequest.GetAll();
                if (response.Item1 == HttpStatusCode.OK) { oldList = response.Item2; }
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
                    if (found) { UpdateDto<ModelType> updateDto = new(); updateDto.Dto.Model2Dto(newObj); await httpRequest.Update(updateDto); }
                    else { int deletedId = GlobalValues.NoId; while (deletedId < newObj.Id) { deletedId = await AddSqlForceId(newObj); } }
                }
                _ = LoadSql();
                DatabaseVM.UseForceDelete(keepValue);
                DatabaseVM.UseForceSameId(keepValue);
            }
        }

        public void ClearSql()
        {
            if (DatabaseVM.UseForceReseed(true))
            {
                if (SqlConnectionConfig?.Connectivity() ?? false) { SqlConnectionConfig.Reseed(typeof(ModelType)); }
                DatabaseVM.UseForceReseed();
            }
            else if (httpRequest is not null)
            {
                foreach (ModelType obj in ObjList) { _ = httpRequest.Delete(obj.Id, DatabaseVM.UseForceDelete(true)); }
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

        public void ExportJson(bool keepValue = false)
        {
            if (DatabaseVM.UseForceReseed(true)) { if (SqlConnectionConfig?.Connectivity() ?? false) { SqlConnectionConfig.Reseed(typeof(ModelType)); }; }
            ObjList = GetJsonList();
            _ = WriteSql(keepValue);
            DatabaseVM.UseForceReseed(keepValue);
        }

        public void LoadConvertedJson()
        {
            allowFilter = false;
            _ = ResetLists(GetConvertedJsonList());
            allowFilter = true;
            OnPublishList();
        }

        public void ExportConvertedJson(bool keepValue = false)
        {
            if (DatabaseVM.UseForceReseed(true)) { if (SqlConnectionConfig?.Connectivity() ?? false) { SqlConnectionConfig.Reseed(typeof(ModelType)); }; }
            ObjList = GetConvertedJsonList();
            _ = WriteSql(keepValue);
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
            if (httpRequest is not null && allowFilter)
            {
                Tuple<HttpStatusCode, List<ModelType>> response = await httpRequest.GetByFilter(DatabaseFilter<ModelType>.GetFilterDtos(Filters));
                if (response.Item1 == HttpStatusCode.OK)
                {
                    for (int objNr = 0; objNr < response.Item2.Count; objNr++) { FilteredList.Add(new DataRow<ModelType>(response.Item2[objNr], true, objNr + 1)); }
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
            string fileNameToFind = typeof(ModelType).Name.ToLower() + ".json";
            foreach (string directoryPath in Directory.EnumerateDirectories(GlobalValues.DataDirectory))
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
            else { dbVersion = null; DbVersion = DbVersionList[^1]; }
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
                string path = GlobalValues.DataDirectory + dbVersion + "\\" + typeof(ModelType).Name.ToLower() + ".json";
                List<object> oldList = JsonConvert.DeserializeObject<List<object>>(File.ReadAllText(path, Encoding.Unicode)) ?? [];
                foreach (IEnumerable<KeyValuePair<string, JToken>> oldObj in oldList.Cast<IEnumerable<KeyValuePair<string, JToken>>>())
                {
                    dynamic newObj = Activator.CreateInstance(MainVM.DictOldDbVersionModels[typeof(ModelType)][DbVersionNr])!;
                    foreach (PropertyInfo newProperty in newObj.GetType().GetProperties())
                    {
                        foreach (KeyValuePair<string, JToken> oldProperty in oldObj)
                        {
                            if (oldProperty.Key.Replace("_","").Equals(newProperty.Name.Replace("_", ""), StringComparison.CurrentCultureIgnoreCase))
                            {
                                var newValue = Scripts.CastValue(newProperty, oldProperty.Value);
                                if (newValue is not null && newValue.GetType() == newProperty.PropertyType) { newProperty.SetValue(newObj, newValue); }
                                break;
                            }
                        }
                    }
                    newList.Add(Mapper<ModelType>.Map(newObj, new ModelType()));
                }
            }
            return newList;
        }

        public async Task SetStateIdComparison()
        {
            StateIdComparisonJson = GlobalWinValues.StateOff;
            StateIdComparisonConvertedJson = GlobalWinValues.StateOff;

            if (DbVersion is null || httpRequest is null) { return; }
            List<ModelType> listJson = GetJsonList();
            bool allJsonInSql = await AllJsonInSql(listJson);
            if (allJsonInSql) { StateIdComparisonJson = GlobalWinValues.StateRun; }
            else { StateIdComparisonJson = GlobalWinValues.StateWait; }

            if (DbVersion is null || httpRequest is null) { return; }
            List<ModelType> listConvertedJson = GetConvertedJsonList();
            bool allConvertedJsonInSql = await AllJsonInSql(listConvertedJson);
            if (allConvertedJsonInSql) { StateIdComparisonConvertedJson = GlobalWinValues.StateRun; }
            else { StateIdComparisonConvertedJson = GlobalWinValues.StateWait; }

            if (DbVersion is null || httpRequest is null) { return; }
            Tuple<HttpStatusCode, List<ModelType>> getAllResponse = await httpRequest.GetAll();
            if (getAllResponse.Item1 == HttpStatusCode.OK)
            {
                bool allSqlInJson = AllSqlInJson(getAllResponse.Item2, listJson);
                if (!allSqlInJson && allJsonInSql) { StateIdComparisonJson = GlobalWinValues.StateOn; }

                bool allSqlInConvertedJson = AllSqlInJson(getAllResponse.Item2, listConvertedJson);
                if (!allSqlInConvertedJson && allConvertedJsonInSql) { StateIdComparisonConvertedJson = GlobalWinValues.StateOn; }
            }
        }

        public async Task<bool> AllJsonInSql(List<ModelType> list)
        {
            foreach (ModelType obj in list)
            {
                UniqPropsDto<ModelType> uniqPropsDto = new();
                uniqPropsDto.Dto.Model2Dto(obj);

                if (DbVersion is null || httpRequest is null) { return false; }
                Tuple<HttpStatusCode, ModelType?> getResponse = await httpRequest.GetByUniqProps(uniqPropsDto);
                if (getResponse.Item1 != HttpStatusCode.OK || getResponse.Item2 is null || getResponse.Item2.Id != obj.Id) { return false; }
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
                Tuple<HttpStatusCode, ModelType?> addResponse = await httpRequest.Add(addDto);
                if (addResponse.Item1 == HttpStatusCode.OK && DatabaseVM.UseForceSameId(true))
                {
                    UniqPropsDto<ModelType> uniqPropsDto = new();
                    uniqPropsDto.Dto.Model2Dto(obj);
                    Tuple<HttpStatusCode, ModelType?> getResponse = await httpRequest.GetByUniqProps(uniqPropsDto);
                    if (getResponse.Item1 == HttpStatusCode.OK && getResponse.Item2 is not null && getResponse.Item2.Id != obj.Id)
                    {
                        HttpStatusCode deleteResponse = await httpRequest.Delete(getResponse.Item2.Id, true);
                        if (deleteResponse == HttpStatusCode.OK) { return getResponse.Item2.Id; }
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
