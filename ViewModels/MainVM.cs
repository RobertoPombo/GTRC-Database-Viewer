namespace GTRC_Database_Viewer.ViewModels
{
    public class MainVM : GTRC_WPF.ViewModels.MainVM
    {
        public static MainVM? Instance;

        private ClientConnectionSettingsVM? clientConnectionSettingsVM;
        private DatabaseVM? databaseVM;

        public MainVM()
        {
            Instance = this;
            DatabaseVM = new DatabaseVM();
            ClientConnectionSettingsVM = new ClientConnectionSettingsVM();
        }

        public ClientConnectionSettingsVM? ClientConnectionSettingsVM { get { return clientConnectionSettingsVM; } set { clientConnectionSettingsVM = value; RaisePropertyChanged(); } }
        public DatabaseVM? DatabaseVM { get { return databaseVM; } set { databaseVM = value; RaisePropertyChanged(); } }
    }
}
