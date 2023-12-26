using System.Reflection;

using GTRC_Basics;
using GTRC_Basics.Models.Common;
using GTRC_WPF;

namespace GTRC_Database_Viewer.Models
{
    public class DataField<ModelType> : ObservableObject where ModelType : class, IBaseModel, new()
    {
        private string name = "";
        private dynamic val = GlobalValues.NoId;
        private List<KeyValuePair<string, int>> idList = [];
        private string? path;

        public DataRow<ModelType>? DataRow;
        public PropertyInfo? Property;

        public DataField(DataRow<ModelType>? dataRow, PropertyInfo property, dynamic value) { Initialize(dataRow, property.Name, value); Property = property; }

        public DataField(DataRow<ModelType>? dataRow, string propertyName, dynamic value) { Initialize(dataRow, propertyName, value); }

        public void Initialize(DataRow<ModelType>? dataRow, string propertyName, dynamic value)
        {
            DataRow = dataRow;
            Name = propertyName;
            Value = value;
            idList.Clear();
            foreach (var _obj in new List<dynamic>()) // checken, ob PropertyName Id-Prop ist und durch die ensprechende Tabelle iterieren.
            {
                idList.Add(new KeyValuePair<string, int>(_obj.ToString(), _obj.Id));
            }
            if (Name == "Logo") { Path = Value.ToString(); } else { Path = null; }
        }

        public string Name { get { return name; } set { name = value; RaisePropertyChanged(); } }

        public dynamic Value { get { return val; } set { val = value; RaisePropertyChanged(); } }

        public List<KeyValuePair<string, int>> IdList { get { return idList; } set { idList = value; RaisePropertyChanged(); } }

        public dynamic? Path { get { return path; } set { path = value; RaisePropertyChanged(); } }
    }
}
