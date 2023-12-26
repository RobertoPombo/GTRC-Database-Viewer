namespace GTRC_Database_Viewer.ViewModels
{
    public class MainVM : GTRC_WPF.ViewModels.MainVM
    {
        private object? clientConnectionSettingsVM;
        private object? databaseVM;

        public MainVM()
        {
            DatabaseVM = new DatabaseVM();
            ClientConnectionSettingsVM = new ClientConnectionSettingsVM();
        }

        public object? ClientConnectionSettingsVM { get { return clientConnectionSettingsVM; } set { clientConnectionSettingsVM = value; RaisePropertyChanged(); } }
        public object? DatabaseVM { get { return databaseVM; } set { databaseVM = value; RaisePropertyChanged(); } }
    }
}
