using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows;

using GTRC_Basics;
using GTRC_WPF;

namespace GTRC_Database_Viewer.ViewModels
{
    public class DatabaseVM : ObservableObject
    {
        private static readonly string pathJson = GlobalValues.ConfigDirectory + "config filters.json";
        public static readonly Dictionary<Type, dynamic> DictDatabaseTableVM = [];
        private static Type modelType = GlobalValues.ModelTypeList[0];
        private string codeExpertMode = string.Empty;
        private dynamic databaseTableVM;
        private bool forceDelete = false;
        private bool forceSameId = false;
        private bool forceReseed = false;
        private bool allowIdComparison = false;

        public DatabaseVM()
        {
            foreach (Type modelType in GlobalValues.ModelTypeList)
            {
                Type typeDatabaseList = typeof(DatabaseTableVM<>).MakeGenericType(modelType);
                DictDatabaseTableVM[modelType] = Activator.CreateInstance(typeDatabaseList)!;
            }
            databaseTableVM = DictDatabaseTableVM[modelType];
            if (!File.Exists(pathJson)) { SaveFilters(); }
            RestoreFilters();
            WriteJsonCmd = new UICmd((o) => WriteJson());
            ExportJsonCmd = new UICmd(async (o) => await ExportJson());
            ExportConvertedJsonCmd = new UICmd(async (o) => await ExportConvertedJson());
            GlobalWinValues.StateBackgroundWorkerColorsUpdated += RefreshStateColor;
        }

        public ObservableCollection<KeyValuePair<Type, string>> ModelTypeList
        {
            get
            {
                ObservableCollection<KeyValuePair<Type, string>> list = [];
                foreach (Type type in GlobalValues.ModelTypeList) { list.Add(new KeyValuePair<Type, string>(type, GlobalValues.SqlTableNames[type])); }
                return list;
            }
        }

        public Type ModelType
        {
            get { return modelType; }
            set
            {
                modelType = value;
                DatabaseTableVM = DictDatabaseTableVM[modelType];
                RaisePropertyChanged();
            }
        }

        public string CodeExpertMode
        {
            get { return codeExpertMode; }
            set { codeExpertMode = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(ExpertModeVisibility)); }
        }

        public Visibility ExpertModeVisibility
        {
            get { if (CodeExpertMode == "4242") { return Visibility.Visible; } else { return Visibility.Collapsed; } }
        }

        public dynamic DatabaseTableVM
        {
            get { return databaseTableVM; }
            set { databaseTableVM = value; RaisePropertyChanged(); }
        }

        public bool ForceDelete
        {
            get { return forceDelete; }
            set { if (value != forceDelete) { forceDelete = value; if (!value) { ForceReseed = false; } RaisePropertyChanged(); } }
        }

        public bool ForceSameId
        {
            get { return forceSameId; }
            set { if (value != forceSameId) { forceSameId = value; RaisePropertyChanged(); } }
        }

        public bool ForceReseed
        {
            get { return forceReseed; }
            set { if (value != forceReseed) { forceReseed = value; ForceDelete = forceReseed; RaisePropertyChanged(); } }
        }

        public bool AllowIdComparison
        {
            get { return allowIdComparison; }
            set { if (value != allowIdComparison) { allowIdComparison = value; RaisePropertyChanged(); } }
        }

        public Brush StateColorIdComparisonJson
        {
            get { return GlobalWinValues.ColorsStateBackgroundWorker[GetStateIdComparisonJson()]; }
            set { RaisePropertyChanged(); }
        }

        public Brush StateColorIdComparisonConvertedJson
        {
            get { return GlobalWinValues.ColorsStateBackgroundWorker[GetStateIdComparisonConvertedJson()]; }
            set { RaisePropertyChanged(); }
        }

        public void RestoreFilters()
        {
            try
            {
                dynamic? obj = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(pathJson, Encoding.Unicode));
                foreach (Type modelType in GlobalValues.ModelTypeList)
                {
                    var filter = obj?[modelType.Name] ?? null;
                    if (filter is not null && filter is IList)
                    {
                        foreach (var _databaseFilter in DictDatabaseTableVM[modelType].Filters)
                        {
                            foreach (var property in filter)
                            {
                                string? propertyName = property.PropertyName?.ToString() ?? null;
                                string? filterValue = property.Filter?.ToString() ?? null;
                                if (propertyName == _databaseFilter.PropertyName && filterValue is not null) { _databaseFilter.Filter = filterValue; break; }
                            }
                        }
                    }
                }
                GlobalValues.CurrentLogText = "Filter settings restored.";
            }
            catch { GlobalValues.CurrentLogText = "Restore filter settings failed!"; }
        }

        public static void SaveFilters()
        {
            Dictionary<string, List<dynamic>> filterList = [];
            foreach (KeyValuePair<Type, dynamic> _databaseTable in DictDatabaseTableVM)
            {
                List<dynamic> filters = [];
                foreach (dynamic _filter in _databaseTable.Value.Filters) { filters.Add(_filter); }
                filterList[_databaseTable.Key.Name.ToString()] = filters;
            }
            string text = JsonConvert.SerializeObject(filterList, Formatting.Indented);
            File.WriteAllText(pathJson, text, Encoding.Unicode);
            GlobalValues.CurrentLogText = "Filter settings saved.";
        }

        public static bool UseForceDelete(bool keepValue = false)
        {
            if (MainVM.Instance?.DatabaseVM?.ForceDelete ?? false) { if (!keepValue) { MainVM.Instance.DatabaseVM.ForceDelete = false; } return true; }
            else { return false; }
        }

        public static bool UseForceSameId(bool keepValue = false)
        {
            if (MainVM.Instance?.DatabaseVM?.ForceSameId ?? false) { if (!keepValue) { MainVM.Instance.DatabaseVM.ForceSameId = false; } return true; }
            else { return false; }
        }

        public static bool UseForceReseed(bool keepValue = false)
        {
            if (MainVM.Instance?.DatabaseVM?.ForceReseed ?? false) { if (!keepValue) { MainVM.Instance.DatabaseVM.ForceReseed = false; } return true; }
            else { return false; }
        }

        public static bool IsAllowedIdComparison() { return MainVM.Instance?.DatabaseVM?.AllowIdComparison ?? false; }

        public static StateBackgroundWorker GetStateIdComparisonJson()
        {
            StateBackgroundWorker stateId = StateBackgroundWorker.Off;
            List<StateBackgroundWorker> states = [StateBackgroundWorker.Run, StateBackgroundWorker.On, StateBackgroundWorker.Wait];
            foreach (StateBackgroundWorker state in states)
            {
                foreach (dynamic databaseTable in DictDatabaseTableVM.Values)
                {
                    if (databaseTable.StateIdComparisonJson == state) { stateId = state; }
                }
            }
            return stateId;
        }

        public static StateBackgroundWorker GetStateIdComparisonConvertedJson()
        {
            StateBackgroundWorker stateId = StateBackgroundWorker.Off;
            List<StateBackgroundWorker> states = [StateBackgroundWorker.Run, StateBackgroundWorker.On, StateBackgroundWorker.Wait];
            foreach (StateBackgroundWorker state in states)
            {
                foreach (dynamic databaseTable in DictDatabaseTableVM.Values)
                {
                    if (databaseTable.StateIdComparisonConvertedJson == state) { stateId = state; }
                }
            }
            return stateId;
        }

        public void RefreshStateColor() { RaisePropertyChanged(nameof(StateColorIdComparisonJson)); RaisePropertyChanged(nameof(StateColorIdComparisonConvertedJson)); }

        public void WriteJson()
        {
            foreach (dynamic databaseTable in DictDatabaseTableVM.Values) { databaseTable.WriteJson(); }
        }

        public async Task ExportJson()
        {
            foreach (dynamic databaseTable in DictDatabaseTableVM.Values) { await databaseTable.ExportJson(true, true); }
            UseForceSameId();
            UseForceReseed();
        }

        public async Task ExportConvertedJson()
        {
            foreach (dynamic databaseTable in DictDatabaseTableVM.Values) { await databaseTable.ExportConvertedJson(true, true); }
            UseForceSameId();
            UseForceReseed();
        }

        public UICmd WriteJsonCmd { get; set; }
        public UICmd ExportJsonCmd { get; set; }
        public UICmd ExportConvertedJsonCmd { get; set; }
    }
}
