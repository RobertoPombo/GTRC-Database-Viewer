namespace GTRC_Database_Viewer.Migrations.V0
{
    public class UserRole : GTRC_Basics.Models.UserRole
    {
        public int DriverID { set { UserId = value; RoleId = 1; } }
    }
}
