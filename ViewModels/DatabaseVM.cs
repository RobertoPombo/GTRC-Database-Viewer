using GTRC_Basics;
using GTRC_WPF;
using System.Collections.ObjectModel;

namespace GTRC_Database_Viewer.ViewModels
{
    public class DatabaseVM : ObservableObject
    {
        private readonly Dictionary<Type, dynamic> dictGenericDatabaseVM = [];
        private static Type modelType = GlobalValues.ModelTypeList[0];
        private dynamic genericDatabaseVM;

        public DatabaseVM()
        {
            foreach (Type modelType in GlobalValues.ModelTypeList)
            {
                Type typeDatabaseList = typeof(GenericDatabaseVM<>).MakeGenericType(modelType);
                dictGenericDatabaseVM[modelType] = Activator.CreateInstance(typeDatabaseList)!;
            }
            genericDatabaseVM = dictGenericDatabaseVM[modelType];
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
                GenericDatabaseVM = dictGenericDatabaseVM[modelType];
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
