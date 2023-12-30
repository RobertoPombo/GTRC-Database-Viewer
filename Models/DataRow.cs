using System.Collections.ObjectModel;
using System.Reflection;

using GTRC_Basics;
using GTRC_Basics.Models.Common;
using GTRC_WPF;

namespace GTRC_Database_Viewer.Models
{
    public class DataRow<ModelType> : ObservableObject where ModelType : class, IBaseModel, new()
    {
        private ObservableCollection<DataField<ModelType>> list = [];
        private int indexNr = 0;

        public ModelType Object;

        public DataRow(ModelType obj, bool retId, int index = -1)
        {
            Object = obj;
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                if (property.Name != GlobalValues.Id || retId)
                {
                    List.Add(new DataField<ModelType>(this, property, property.GetValue(obj)));
                }
            }
            if (index > -1) { indexNr = List.Count; List.Add(new DataField<ModelType>(this, nameof(Nr), index)); }
            RaisePropertyChanged(nameof(List));
        }

        public ObservableCollection<DataField<ModelType>> List { get { return list; } set { list = value; RaisePropertyChanged(); } }

        public DataField<ModelType>? GetDataFieldByPropertyName(string name)
        {
            foreach (DataField<ModelType> dataField in List)
            {
                if (dataField.Name == name) { return dataField; }
            }
            return null;
        }

        public dynamic Nr { get { return List[indexNr].Value ?? int.MaxValue; } set { List[indexNr].Value = value; } }
    }
}
