using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Team : GTRC_Basics.Models.Team
    {
        public new int Id { get; set; }

        public new int OrganizationId
        {
            get
            {
                foreach (Organization obj in V0.Organization.List) { if (obj.TeamIds.Contains(Id)) { return obj.Id; } }
                if (V0.Organization.List.Count > 0) { return V0.Organization.List[0].Id; }
                else { return GlobalValues.NoId; }
            }
        }
    }
}
