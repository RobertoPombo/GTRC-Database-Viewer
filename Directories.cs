using GTRC_Basics;
using System.IO;

namespace GTRC_Database_Viewer
{
    public static class Directories
    {
        public static string Database { get { return GlobalValues.DataDirectory + "database\\"; } }
        public static string DbBackup { get { return Database + "backups\\"; } }
        public static string DbMigrations { get { return Database + "migrations\\"; } }

        public static void CreateDirectories()
        {
            if (!Directory.Exists(Database)) { Directory.CreateDirectory(Database); }
            if (!Directory.Exists(DbBackup)) { Directory.CreateDirectory(DbBackup); }
            if (!Directory.Exists(DbMigrations)) { Directory.CreateDirectory(DbMigrations); }
        }
    }
}
