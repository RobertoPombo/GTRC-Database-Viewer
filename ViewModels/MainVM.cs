namespace GTRC_Database_Viewer.ViewModels
{
    public class MainVM : GTRC_WPF.ViewModels.MainVM
    {
        private object? apiConSettingsVM;
        private object? databaseVM;

        public MainVM()
        {
            ApiConSettingsVM = new ApiConSettingsVM();
            DatabaseVM = new DatabaseVM();
        }

        public object? ApiConSettingsVM { get { return apiConSettingsVM; } set { apiConSettingsVM = value; RaisePropertyChanged(); } }
        public object? DatabaseVM { get { return databaseVM; } set { databaseVM = value; RaisePropertyChanged(); } }
    }
}
