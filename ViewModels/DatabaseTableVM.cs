using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

using GTRC_Basics;
using GTRC_Basics.Models.Common;
using GTRC_Database_Client;
using GTRC_Database_Viewer.Models;
using GTRC_WPF;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_Viewer.ViewModels
{
    public class DatabaseTableVM<ModelType> : ObservableObject where ModelType : class, IBaseModel, new()
    {
        private string pathJson = GlobalValues.DataDirectory + typeof(ModelType).Name.ToLower() + ".json";
        private ConnectionSettings? ConnectionSettings;
        private HttpRequest<ModelType>? httpRequest;
        private ObservableCollection<DataRow<ModelType>> filteredList = [];
        private DataRow<ModelType>? current;
        private DataRow<ModelType>? selected;
        private bool forceDelete = false;
        private ObservableCollection<DatabaseFilter<ModelType>> filters = [];
        private int selectedId = GlobalValues.NoId;
        private SortState sortState = new();

        public DatabaseTableVM()
        {
            if (!File.Exists(pathJson)) { WriteJson(); }
            foreach (PropertyInfo property in typeof(ModelType).GetProperties()) { Filters.Add(new(property)); }
            Filters.Add(new("Nr"));
            AddCmd = new UICmd(async (o) => await Add());
            DelCmd = new UICmd(async (o) => await Del());
            ClearCurrentCmd = new UICmd(async (o) => await ClearCurrent());
            UpdateCmd = new UICmd(async (o) => await Update());
            LoadSqlCmd = new UICmd(async (o) => await LoadSql());
            WriteSqlCmd = new UICmd(async (o) => await WriteSql());
            LoadJsonCmd = new UICmd(async (o) => await LoadJson());
            WriteJsonCmd = new UICmd((o) => WriteJson());
            ClearSqlCmd = new UICmd(async (o) => await ClearSql());
            ClearJsonCmd = new UICmd(async (o) => await ClearJson());
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

        public bool ForceDelete
        {
            get { return forceDelete; }
            set { forceDelete = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<DatabaseFilter<ModelType>> Filters { get { return filters; } set { filters = value; RaisePropertyChanged(); } }

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
                else if (response.Item1 == HttpStatusCode.BadRequest || response.Item1 == HttpStatusCode.InternalServerError) { await LoadSql(); }
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
                else if (response.Item1 == HttpStatusCode.NotFound || response.Item1 == HttpStatusCode.InternalServerError) { await ClearCurrent(); }
            }
        }

        public async Task LoadSql()
        {
            if (httpRequest is not null)
            {
                Tuple<HttpStatusCode, List<ModelType>> response = await httpRequest.GetAll();
                if (response.Item1 == HttpStatusCode.OK) { await ResetLists(response.Item2); }
                else { await ResetLists([]); }
            }
            else { await ResetLists([]); }
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
                    if (found) { UpdateDto<ModelType> updateDto = new(); updateDto.Dto.ReMap(newObj); await httpRequest.Update(updateDto); }
                    else { AddDto<ModelType> addDto = new(); addDto.Dto.ReMap(newObj); await httpRequest.Add(addDto); }
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

        public async Task LoadJson()
        {
            await ResetLists(JsonConvert.DeserializeObject<List<ModelType>>(File.ReadAllText(pathJson, Encoding.Unicode)) ?? []);
            OnPublishList();
        }

        public void WriteJson()
        {
            File.WriteAllText(pathJson, JsonConvert.SerializeObject(ObjList, Formatting.Indented), Encoding.Unicode);
            OnPublishList();
        }

        public async Task ClearSql()
        {
            if (httpRequest is not null) { foreach (ModelType obj in ObjList) { await httpRequest.Delete(obj.Id, ForceDelete); } UseForceDel(); }
            await LoadSql();
        }

        public async Task ClearJson()
        {
            File.WriteAllText(pathJson, JsonConvert.SerializeObject(new List<ModelType>(), Formatting.Indented), Encoding.Unicode);
            await LoadJson();
        }

        public void ClearFilter()
        {
            for (int filterNr = Filters.Count - 1; filterNr >= 0; filterNr--) { Filters[filterNr].Filter = ""; }
        }

        public bool UseForceDel() { if (ForceDelete) { ForceDelete = false; return true; } else { return false; } }

        public async Task ResetLists(List<ModelType> _list, int index = 0)
        {
            ObjList = _list;
            await FilterList(index);
        }

        public async Task FilterList(int index = 0)
        {
            FilteredList.Clear();
            if (httpRequest is not null)
            {
                Tuple<HttpStatusCode, List<ModelType>> response = await httpRequest.GetByFilter(DatabaseFilter<ModelType>.GetFilterDtos(Filters));
                if (response.Item1 == HttpStatusCode.OK)
                {
                    for (int objNr = 0; objNr < response.Item2.Count; objNr++) { FilteredList.Add(new DataRow<ModelType>(response.Item2[objNr], true, objNr + 1)); }
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
                    dynamic? val1 = Scripts.GetCastedValue(FilteredList[rowNr1].Object, property);
                    dynamic? val2 = Scripts.GetCastedValue(FilteredList[rowNr2].Object, property);
                    if (stringCompare) { isAscending = String.Compare(val1?.ToString(), val2?.ToString()) < 0; }
                    else { isAscending = val1 < val2; }
                    if ((sortState.SortAscending && !isAscending) || (!sortState.SortAscending && isAscending))
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
