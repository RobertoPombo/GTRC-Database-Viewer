using GTRC_Basics;
using GTRC_WPF;
using System.Collections.ObjectModel;

namespace GTRC_Database_Viewer.ViewModels
{
    public class DatabaseVM : ObservableObject
    {
        public static readonly Dictionary<Type, dynamic> DictGenericDatabaseVM = [];
        private static Type modelType = GlobalValues.ModelTypeList[0];
        private dynamic genericDatabaseVM;

        public DatabaseVM()
        {
            foreach (Type modelType in GlobalValues.ModelTypeList)
            {
                Type typeDatabaseList = typeof(GenericDatabaseVM<>).MakeGenericType(modelType);
                DictGenericDatabaseVM[modelType] = Activator.CreateInstance(typeDatabaseList)!;
            }
            genericDatabaseVM = DictGenericDatabaseVM[modelType];
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
                GenericDatabaseVM = DictGenericDatabaseVM[modelType];
                RaisePropertyChanged();
            }
        }

        public dynamic GenericDatabaseVM
        {
            get { return genericDatabaseVM; }
            set { genericDatabaseVM = value; RaisePropertyChanged(); }
        }
    }
}
