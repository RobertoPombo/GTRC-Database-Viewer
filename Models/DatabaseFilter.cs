using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;

using GTRC_Basics;
using GTRC_WPF;
using GTRC_Basics.Models.Common;
using GTRC_Database_Viewer.ViewModels;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_Viewer.Models
{
    public class DatabaseFilter<ModelType> : ObservableObject where ModelType : class, IBaseModel, new()
    {
        public static readonly string NoIdFilter = "-1";
        private readonly PropertyInfo? property;
        private string propertyName = "";
        private string filter = string.Empty;
        private byte filterProcessCount = 0;

        public DatabaseFilter(PropertyInfo _property) { property = _property; Initialize(property.Name); }
        public DatabaseFilter(string _propertyName) { Initialize(_propertyName); }
        public void Initialize(string _propertyName)
        {
            SortCmd = new UICmd((o) => Sort());
            propertyName = _propertyName;
            if (propertyName == GlobalValues.Id) { filter = NoIdFilter; }
        }

        [JsonIgnore] public PropertyInfo? Property { get { return property; } }

        public string PropertyName { get { return propertyName; } }

        public string Filter
        {
            get { return filter; }
            set
            {
                if (filter != value)
                {
                    filterProcessCount += 1;
                    filter = value;
                    List<DatabaseFilter<ModelType>> list = [];
                    foreach (DatabaseFilter<ModelType> _filter in DatabaseVM.DictDatabaseTableVM[typeof(ModelType)].Filters) { list.Add(_filter); }
                    if (list[0].PropertyName != PropertyName)
                    {
                        if (filter == string.Empty)
                        {
                            bool noFilter = true;
                            foreach (DatabaseFilter<ModelType> _filter in list) { if (_filter.Filter != filter) { noFilter = false; break; } }
                            if (noFilter) { list[0].Filter = NoIdFilter; }
                        }
                        else { if (list[0].Filter == NoIdFilter) { list[0].Filter = string.Empty; } }
                    }
                    RaisePropertyChanged();
                    new Thread(ThreadFilterList).Start();
                }
            }
        }

        public static FilterDtos<ModelType> GetFilterDtos (ObservableCollection<DatabaseFilter<ModelType>> filters)
        {
            FilterDtos<ModelType> objDto = new();
            foreach (DatabaseFilter<ModelType> filter in filters)
            {
                if (filter.Filter != string.Empty)
                {
                    foreach (PropertyInfo property in objDto.Dto.Filter.GetType().GetProperties())
                    {
                        if (filter.PropertyName == property.Name)
                        {
                            string[] filterMinMax = filter.Filter.Split(':');
                            if (filterMinMax.Length == 1)
                            {
                                var castedValue = Scripts.CastValue(property, filter.Filter);
                                if (castedValue is not null) { property.SetValue(objDto.Dto.Filter, castedValue); }
                            }
                            else if (filterMinMax.Length == 2)
                            {
                                var castedValueMin = Scripts.CastValue(property, filterMinMax[0]);
                                var castedValueMax = Scripts.CastValue(property, filterMinMax[1]);
                                if (castedValueMin is not null) { property.SetValue(objDto.Dto.FilterMin, castedValueMin); }
                                if (castedValueMax is not null) { property.SetValue(objDto.Dto.FilterMax, castedValueMax); }
                            }
                        }
                    }
                }
            }
            return objDto;
        }

        public void Sort()
        {
            if (property is not null) { DatabaseVM.DictDatabaseTableVM[typeof(ModelType)].SortFilteredList(property); }
        }

        public void ThreadFilterList()
        {
            Thread.Sleep(Math.Max(200, 300 - filterProcessCount * 5));
            filterProcessCount -= 1;
            if (filterProcessCount == 0) { Application.Current.Dispatcher.Invoke(new Action(() => DatabaseVM.DictDatabaseTableVM[typeof(ModelType)].FilterList())); }
        }

        [JsonIgnore] public UICmd? SortCmd { get; set; }
    }
}
