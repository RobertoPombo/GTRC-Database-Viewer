using GTRC_Basics.Models;

namespace GTRC_Database_Viewer.ViewModels
{
    public class MainVM : GTRC_WPF.ViewModels.MainVM
    {
        public static MainVM? Instance;

        private DatabaseVM? databaseVM;
        private DbApiConnectionConfigVM? dbApiConnectionConfigVM;
        private SqlConnectionConfigVM? sqlConnectionConfigVM;

        public MainVM()
        {
            Instance = this;
            DatabaseVM = new DatabaseVM();
            DbApiConnectionConfigVM = new DbApiConnectionConfigVM();
            SqlConnectionConfigVM = new SqlConnectionConfigVM();
        }

        public DatabaseVM? DatabaseVM { get { return databaseVM; } set { databaseVM = value; RaisePropertyChanged(); } }
        public DbApiConnectionConfigVM? DbApiConnectionConfigVM { get { return dbApiConnectionConfigVM; } set { dbApiConnectionConfigVM = value; RaisePropertyChanged(); } }
        public SqlConnectionConfigVM? SqlConnectionConfigVM { get { return sqlConnectionConfigVM; } set { sqlConnectionConfigVM = value; RaisePropertyChanged(); } }

        public static readonly Dictionary<Type, List<Type>> DictOldDbVersionModels = new()
        { // V0
            { typeof(Color), new List<Type>() { typeof(Color) } },
            { typeof(User), new List<Type>() { typeof(Migrations.V0.User) } },
            { typeof(Track), new List<Type>() { typeof(Migrations.V0.Track) } },
            { typeof(Carclass), new List<Type>() { typeof(Migrations.V0.Carclass) } },
            { typeof(Manufacturer), new List<Type>() { typeof(Migrations.V0.Manufacturer) } },
            { typeof(Car), new List<Type>() { typeof(Migrations.V0.Car) } },
            { typeof(Series), new List<Type>() { typeof(Series) } },
            { typeof(Season), new List<Type>() { typeof(Migrations.V0.Season) } },
            { typeof(SeasonCarclass), new List<Type>() { typeof(Migrations.V0.SeasonCarclass) } }
        };
    }
}
