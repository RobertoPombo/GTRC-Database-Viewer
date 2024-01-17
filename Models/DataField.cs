using System.Reflection;
using System.Windows.Media;

using GTRC_Basics;
using GTRC_Basics.Models.Common;
using GTRC_Database_Viewer.ViewModels;
using GTRC_WPF;

namespace GTRC_Database_Viewer.Models
{
    public class DataField<ModelType> : ObservableObject where ModelType : class, IBaseModel, new()
    {
        private string name = string.Empty;
        private dynamic? value;
        private Brush? color;
        private List<dynamic> listDropdown = [];
        private DataDisplayType dataType = DataDisplayType.Default;

        public DataRow<ModelType>? DataRow;
        public PropertyInfo? Property;

        public DataField(DataRow<ModelType>? dataRow, PropertyInfo property, dynamic? val, bool retFull) { Property = property; Initialize(dataRow, property.Name, val, retFull); }

        public DataField(DataRow<ModelType>? dataRow, string propertyName, dynamic? val, bool retFull) { Initialize(dataRow, propertyName, val, retFull); }

        public void Initialize(DataRow<ModelType>? dataRow, string propertyName, dynamic? value, bool retFull)
        {
            DataRow = dataRow;
            Name = propertyName;
            Value = value;
            listDropdown.Clear();
            if (Property is not null)
            {
                Type? TypeForeignId = GlobalValues.GetTypeForeignId(Name);
                Type? NullableType = Nullable.GetUnderlyingType(Property.PropertyType);
                if (Property.PropertyType == typeof(bool) || (NullableType is not null && NullableType == typeof(bool))) // Bool properties
                {
                    if (retFull) { if (value) { Color = GlobalWinValues.StateRun; } else { Color = GlobalWinValues.StateWait; } DataType = DataDisplayType.Color; }
                    else { DataType = DataDisplayType.Checkbox; }
                }
                else if (!retFull && ((NullableType is not null && NullableType.IsEnum) || (NullableType is null && Property.PropertyType.IsEnum))) // Enum properties
                {
                    foreach (var enumType in Enum.GetValues(NullableType ?? Property.PropertyType)) { listDropdown.Add(enumType); }
                    DataType = DataDisplayType.Dropdown;
                }
                else if (!retFull && TypeForeignId is not null) // Id properties
                {
                    var _list = DatabaseVM.DictDatabaseTableVM[TypeForeignId].ObjList;
                    foreach (dynamic obj in _list) { listDropdown.Add(obj); }
                    DataType = DataDisplayType.DropdownId;
                }
                else if (retFull && Property.PropertyType == typeof(System.Drawing.Color)) // Color preview
                {
                    Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(value?.A ?? 0, value?.R ?? 0, value?.G ?? 0, value?.B ?? 0));
                    DataType = DataDisplayType.Color;
                }
                /*else if (retFull && value?.ToString().Length > ".png".Length && value?.ToString()[^".png".Length..] == ".png") // Image properties
                {
                    DataType = DataDisplayType.Image;
                }*/
            }
        }

        public string Name { get { return name; } set { name = value; RaisePropertyChanged(); } }

        public dynamic? Value { get { return value; } set { this.value = value; RaisePropertyChanged(); } }

        public Brush? Color { get { return color; } set { color = value; RaisePropertyChanged(); } }

        public List<dynamic> ListDropdown
        {
            get { return listDropdown; }
            set { listDropdown = value; RaisePropertyChanged(); }
        }

        public DataDisplayType DataType { get { return dataType; } set { dataType = value; RaisePropertyChanged(); } }
    }
}
