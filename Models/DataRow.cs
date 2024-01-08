using GTRC_Basics;
using GTRC_Basics.Models.Common;
using GTRC_Basics.Models.DTOs;
using GTRC_Database_Viewer.ViewModels;
using GTRC_WPF;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Media.Media3D;

namespace GTRC_Database_Viewer.Models
{
    public class DataRow<ModelType> : ObservableObject where ModelType : class, IBaseModel, new()
    {
        private ObservableCollection<DataField<ModelType>> list = [];
        private int indexNr = 0;

        public ModelType Object;
        public dynamic? ObjectDto;

        public DataRow(ModelType obj, bool retFull, int index = -1)
        {
            Object = obj;
            ObjectDto = Mapper<ModelType>.MapToFull(obj);
            if (retFull)
            {
                foreach (DatabaseFilter<ModelType> filter in DatabaseVM.DictDatabaseTableVM[typeof(ModelType)].Filters)
                {
                    if (filter.Property is not null) { List.Add(new DataField<ModelType>(this, filter.Property, filter.Property?.GetValue(ObjectDto))); }
                }
            }
            else
            {
                foreach (PropertyInfo property in Object.GetType().GetProperties())
                {
                    if (property.Name != GlobalValues.Id || retFull)
                    {
                        List.Add(new DataField<ModelType>(this, property, property.GetValue(Object)));
                    }
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
