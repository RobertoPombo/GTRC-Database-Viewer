using System.Reflection;

using GTRC_Basics;
using GTRC_WPF;

namespace GTRC_Database_Viewer.Models
{
    public class DataFieldM : ObservableObject
    {
        private string name = "";
        private dynamic val = GlobalValues.NoId;
        private List<KeyValuePair<string, int>> idList = [];
        private string? path;

        public DataRowM? DataRow;
        public PropertyInfo Property;

        public DataFieldM(DataRowM? _dataRow, KeyValuePair<PropertyInfo, dynamic> item)
        {
            DataRow = _dataRow;
            Property = item.Key;
            Name = Property.Name;
            Value = item.Value;
            idList.Clear();
            foreach (var _obj in new List<dynamic>()) // checken, ob PropertyName Id-Prop ist und durch die ensprechende Tabelle iterieren.
            {
                idList.Add(new KeyValuePair<string, int>(_obj.ToString(), _obj.ID));
            }
            if (Name == "Logo") { Path = Value.ToString(); } else { Path = null; }
        }

        public string Name { get { return name; } set { name = value; RaisePropertyChanged(); } }

        public dynamic Value { get { return val; } set { val = value; RaisePropertyChanged(); } }

        public List<KeyValuePair<string, int>> IdList { get { return idList; } set { idList = value; RaisePropertyChanged(); } }

        public dynamic? Path { get { return path; } set { path = value; RaisePropertyChanged(); } }
    }
}
