using GTRC_Basics.Models;

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

        public static readonly Dictionary<Type, List<Type>> DictOldDbVersionModels = new()
        {
            { typeof(Color), new List<Type>() { typeof(Color) } },
            { typeof(Car), new List<Type>() { typeof(GTRC_Basics.Migrations.V0.Car) } },
            { typeof(Track), new List<Type>() { typeof(GTRC_Basics.Migrations.V0.Track) } },
            { typeof(User), new List<Type>() { typeof(User) } },
            { typeof(Series), new List<Type>() { typeof(Series) } },
            { typeof(Season), new List<Type>() { typeof(GTRC_Basics.Migrations.V0.Season) } }
        };
    }
}
