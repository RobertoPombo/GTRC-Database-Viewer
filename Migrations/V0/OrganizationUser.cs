using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class OrganizationUser : GTRC_Basics.Models.OrganizationUser
    {
        private static int nextId = GTRC_Basics.GlobalValues.Id0;
        public static List<OrganizationUser> List = [];

        private int id = GTRC_Basics.GlobalValues.NoId;

        public new int Id
        {
            get { return id; }
            set { }
        }

        public int DriverID { set { UserId = value; } }

        public int TeamID
        {
            set
            {
                if (V0.Organization.List.Count > 0) { OrganizationId = V0.Organization.List[0].Id; }
                foreach (Organization obj in V0.Organization.List) { if (obj.TeamIds.Contains(value)) { OrganizationId = obj.Id; break; } }
                OrganizationUser? organizationUser = null;
                foreach (OrganizationUser _organizationUser in List)
                {
                    if (_organizationUser.OrganizationId == OrganizationId && _organizationUser.UserId == UserId) { organizationUser = _organizationUser; }
                }
                if (organizationUser is not null && OrganizationId != GlobalValues.NoId && UserId != GlobalValues.NoId) { id = organizationUser.Id; }
                else if (OrganizationId != GlobalValues.NoId && UserId != GlobalValues.NoId)
                {
                    List.Add(this);
                    id = nextId;
                    nextId++;
                }
            }
        }
    }
}
