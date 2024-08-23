using GTRC_Basics;
using System.IO;

namespace GTRC_Database_Viewer
{
    public static class Directories
    {
        public static string DbMigrations { get { return GlobalValues.DatabaseDirectory + "migrations\\"; } }
        public static string DbQuickBackup { get { return GlobalValues.DatabaseDirectory + "viewer\\"; } }

        public static void CreateDirectories()
        {
            if (!Directory.Exists(DbMigrations)) { Directory.CreateDirectory(DbMigrations); }
            if (!Directory.Exists(DbQuickBackup)) { Directory.CreateDirectory(DbQuickBackup); }
        }
    }
}
