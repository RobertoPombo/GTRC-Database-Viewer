using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

using GTRC_Basics;
using GTRC_WPF;

namespace GTRC_Database_Viewer.ViewModels
{
    public class DatabaseVM : ObservableObject
    {
        private static readonly string pathJson = GlobalValues.DataDirectory + "config filters.json";
        public static readonly Dictionary<Type, dynamic> DictDatabaseTableVM = [];
        private static Type modelType = GlobalValues.ModelTypeList[0];
        private dynamic databaseTableVM;
        private bool forceDelete = false;
        private bool acceptNewId = true;

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
        }

        public ObservableCollection<KeyValuePair<string, Type>> ModelTypeList
        {
            get
            {
                ObservableCollection<KeyValuePair<string, Type>> list = [];
                foreach (Type type in GlobalValues.ModelTypeList) { list.Add(new KeyValuePair<string, Type>(type.Name, type)); }
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

        public dynamic DatabaseTableVM
        {
            get { return databaseTableVM; }
            set { databaseTableVM = value; RaisePropertyChanged(); }
        }

        public bool ForceDelete
        {
            get { return forceDelete; }
            set { forceDelete = value; RaisePropertyChanged(); }
        }

        public bool AcceptNewId
        {
            get { return acceptNewId; }
            set { acceptNewId = value; RaisePropertyChanged(); }
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

        public static bool UseForceDelete(bool keepValue=false)
        {
            if (MainVM.Instance?.DatabaseVM?.ForceDelete ?? false) { if (!keepValue) { MainVM.Instance.DatabaseVM.ForceDelete = false; } return true; }
            else { return false; }
        }

        public static bool UseAcceptNewId(bool keepValue = false)
        {
            if (MainVM.Instance?.DatabaseVM?.AcceptNewId ?? true) { return true; }
            else { if (!keepValue) { MainVM.Instance.DatabaseVM.AcceptNewId = true; } return false; }
        }
    }
}
