using System.Collections.ObjectModel;
using System.Reflection;

using GTRC_Basics;
using GTRC_Basics.Models.Common;
using GTRC_Basics.Models.DTOs;
using GTRC_Database_Viewer.ViewModels;
using GTRC_WPF;

namespace GTRC_Database_Viewer.Models
{
    public class DataRow<ModelType> : ObservableObject where ModelType : class, IBaseModel, new()
    {
        private ObservableCollection<DataField<ModelType>> list = [];
        private readonly int indexNr = 0;
        public ModelType Object;

        public DataRow(ModelType obj, bool retFull, int index = -1)
        {
            Object = obj;
            if (retFull)
            {
                FullDto<ModelType> dto = new() { Dto = Mapper<ModelType>.Model2FullDto(Object) };
                foreach (DatabaseFilter<ModelType> filter in DatabaseVM.DictDatabaseTableVM[typeof(ModelType)].Filters)
                {
                    if (filter.Property is not null) { List.Add(new DataField<ModelType>(this, filter.Property, filter.Property?.GetValue(dto.Dto), retFull)); }
                }
            }
            else
            {
                AddDto<ModelType> dto = new();
                dto.Dto.Model2Dto(Object);
                foreach (PropertyInfo property in GlobalValues.DictDtoModels[typeof(ModelType)][DtoType.Add].GetProperties())
                {
                    List.Add(new DataField<ModelType>(this, property, property.GetValue(dto.Dto), retFull));
                }
            }
            if (index > -1) { indexNr = List.Count; List.Add(new DataField<ModelType>(this, nameof(Nr), index, retFull)); }
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

        public void UpdateObject()
        {
            foreach (DataField<ModelType> dataField in List)
            {
                PropertyInfo? propertyModel = typeof(ModelType).GetProperty(dataField.Property?.Name ?? string.Empty);
                if (dataField.Property is not null && propertyModel is not null)
                {
                    propertyModel.SetValue(Object, Scripts.CastValue(dataField.Property, dataField.Value));
                }
            }
        }
    }
}
