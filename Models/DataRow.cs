using System.Collections.ObjectModel;
using System.Reflection;

using GTRC_Basics;
using GTRC_WPF;

namespace GTRC_Database_Viewer.Models
{
    public class DataRow : ObservableObject
    {
        private ObservableCollection<DataField> list = [];

        public dynamic Object;

        public DataRow(dynamic _obj, bool retID, bool retJsonIgnore)
        {
            Object = _obj;
            Dictionary<PropertyInfo, dynamic> dict = _obj.ReturnAsDict(retID, retJsonIgnore, true, true);
            foreach (KeyValuePair<PropertyInfo, dynamic> item in dict) { List.Add(new DataField(this, item)); }
            RaisePropertyChanged(nameof(List));
        }

        public ObservableCollection<DataField> List { get { return list; } set { list = value; RaisePropertyChanged(); } }

        public List<string> Names
        {
            get
            {
                List<string> _list = [];
                foreach (DataField _dataField in List) { _list.Add(_dataField.Name); }
                return _list;
            }
        }

        public List<dynamic> Values
        {
            get
            {
                List<dynamic> _list = [];
                foreach (DataField _dataField in List) { _list.Add(Scripts.CastValue(_dataField.Property, _dataField.Value)); }
                return _list;
            }
        }

        public DataField GetDataFieldByPropertyName(string _name)
        {
            foreach (DataField _dataField in List)
            {
                if (_dataField.Name == _name) { return _dataField; }
            }
            return new DataField(null, new KeyValuePair<PropertyInfo, dynamic>());
        }
    }
}
