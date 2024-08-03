using GTRC_Basics.Models;
using GTRC_WPF_UserControls.ViewModels;

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
            { typeof(Sim), new List<Type>() { typeof(Migrations.V0.Sim) } },
            { typeof(User), new List<Type>() { typeof(Migrations.V0.User) } },
            { typeof(Track), new List<Type>() { typeof(Migrations.V0.Track) } },
            { typeof(Carclass), new List<Type>() { typeof(Migrations.V0.Carclass) } },
            { typeof(Manufacturer), new List<Type>() { typeof(Migrations.V0.Manufacturer) } },
            { typeof(Car), new List<Type>() { typeof(Migrations.V0.Car) } },
            { typeof(Role), new List<Type>() { typeof(Role) } },
            { typeof(UserRole), new List<Type>() { typeof(Migrations.V0.UserRole) } },
            { typeof(UserDatetime), new List<Type>() { typeof(Migrations.V0.UserDatetime) } },
            { typeof(Bop), new List<Type>() { typeof(Bop) } },
            { typeof(BopTrackCar), new List<Type>() { typeof(BopTrackCar) } },
            { typeof(Series), new List<Type>() { typeof(Migrations.V0.Series) } },
            { typeof(Season), new List<Type>() { typeof(Migrations.V0.Season) } },
            { typeof(SeasonCarclass), new List<Type>() { typeof(Migrations.V0.SeasonCarclass) } },
            { typeof(Organization), new List<Type>() { typeof(Migrations.V0.Organization) } },
            { typeof(OrganizationUser), new List<Type>() { typeof(Migrations.V0.OrganizationUser) } },
            { typeof(Team), new List<Type>() { typeof(Migrations.V0.Team) } },
            { typeof(Entry), new List<Type>() { typeof(Migrations.V0.Entry) } },
            { typeof(EntryDatetime), new List<Type>() { typeof(Migrations.V0.EntryDatetime) } },
            { typeof(Event), new List<Type>() { typeof(Migrations.V0.Event) } },
            { typeof(EventCarclass), new List<Type>() { typeof(EventCarclass) } },
            { typeof(EventCar), new List<Type>() { typeof(Migrations.V0.EventCar) } },
            { typeof(EntryEvent), new List<Type>() { typeof(Migrations.V0.EntryEvent) } },
            { typeof(EntryUserEvent), new List<Type>() { typeof(EntryUserEvent) } },
            { typeof(Pointssystem), new List<Type>() { typeof(Pointssystem) } },
            { typeof(PointssystemPosition), new List<Type>() { typeof(Migrations.V0.PointssystemPosition) } },
            { typeof(Server), new List<Type>() { typeof(Server) } },
            { typeof(Session), new List<Type>() { typeof(Migrations.V0.Session) } },
            { typeof(Resultsfile), new List<Type>() { typeof(Resultsfile) } },
            { typeof(Lap), new List<Type>() { typeof(Lap) } },
            { typeof(Leaderboardline), new List<Type>() { typeof(Leaderboardline) } },
            { typeof(Incident), new List<Type>() { typeof(Incident) } },
            { typeof(IncidentEntry), new List<Type>() { typeof(IncidentEntry) } }
        };
    }
}
