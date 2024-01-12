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
        private string name = "";
        private dynamic? value = GlobalValues.NoId;
        private List<dynamic> listDropdown = [];
        private string? path;

        public DataRow<ModelType>? DataRow;
        public PropertyInfo? Property;

        public DataField(DataRow<ModelType>? dataRow, PropertyInfo property, dynamic? val) { Property = property; Initialize(dataRow, property.Name, val); }

        public DataField(DataRow<ModelType>? dataRow, string propertyName, dynamic? val) { Initialize(dataRow, propertyName, val); }

        public void Initialize(DataRow<ModelType>? dataRow, string propertyName, dynamic? value)
        {
            DataRow = dataRow;
            Name = propertyName;
            Value = value;
            listDropdown.Clear();
            if (Property is not null)
            {
                Type? TypeForeignId = GlobalValues.GetTypeForeignId(Name);
                if (Property.PropertyType.IsEnum)
                {
                    foreach (var enumType in Enum.GetValues(Property.PropertyType))
                    {
                        listDropdown.Add(enumType.ToString() ?? string.Empty);
                    }
                    Value = value?.ToString();
                }
                else if (TypeForeignId is not null)
                {
                    var _list = DatabaseVM.DictDatabaseTableVM[TypeForeignId].ObjList;
                    foreach (dynamic obj in _list) { listDropdown.Add(obj); }
                }
                else if (Property.PropertyType == typeof(System.Drawing.Color))
                {
                    Value = new SolidColorBrush(Color.FromArgb(value?.A ?? 0, value?.R ?? 0, value?.G ?? 0, value?.B ?? 0));
                }
                else if (Name == "Logo") { Path = Value?.ToString(); }
            }
            else { Path = null; }
        }

        public string Name { get { return name; } set { name = value; RaisePropertyChanged(); } }

        public dynamic? Value { get { return value; } set { this.value = value; RaisePropertyChanged(); } }

        public List<dynamic> ListDropdown
        {
            get { return listDropdown; }
            set { listDropdown = value; RaisePropertyChanged(); }
        }

        public dynamic? Path { get { return path; } set { path = value; RaisePropertyChanged(); } }
    }
}
