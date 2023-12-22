using System.Collections.ObjectModel;
using System.Reflection;

using GTRC_Basics;
using GTRC_WPF;

namespace GTRC_Database_Viewer.Models
{
    public class DataRowM : ObservableObject
    {
        private ObservableCollection<DataFieldM> list = [];

        public dynamic Object;

        public DataRowM(dynamic _obj, bool retID, bool retJsonIgnore)
        {
            Object = _obj;
            Dictionary<PropertyInfo, dynamic> dict = _obj.ReturnAsDict(retID, retJsonIgnore, true, true);
            foreach (KeyValuePair<PropertyInfo, dynamic> item in dict) { List.Add(new DataFieldM(this, item)); }
            RaisePropertyChanged(nameof(List));
        }

        public ObservableCollection<DataFieldM> List { get { return list; } set { list = value; RaisePropertyChanged(); } }

        public List<string> Names
        {
            get
            {
                List<string> _list = [];
                foreach (DataFieldM _dataField in List) { _list.Add(_dataField.Name); }
                return _list;
            }
        }

        public List<dynamic> Values
        {
            get
            {
                List<dynamic> _list = [];
                foreach (DataFieldM _dataField in List) { _list.Add(Scripts.CastValue(_dataField.Property, _dataField.Value)); }
                return _list;
            }
        }

        public DataFieldM GetDataFieldByPropertyName(string _name)
        {
            foreach (DataFieldM _dataField in List)
            {
                if (_dataField.Name == _name) { return _dataField; }
            }
            return new DataFieldM(null, new KeyValuePair<PropertyInfo, dynamic>());
        }
    }
}
