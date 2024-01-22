using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Team : GTRC_Basics.Models.Team
    {
        private static int nextId = GlobalValues.Id0;
        public static List<Team> List = [];

        private int id = GlobalValues.NoId;
        private string name = string.Empty;

        public int OldId = GlobalValues.NoId;

        public new int Id
        {
            get { return id; }
            set { OldId = value; }
        }

        public new string Name
        {
            get { return name; }
            set
            {
                name = value;
                bool isInList = false;
                foreach (Team t in List) { if (t.Name == name) { isInList = true; id = t.Id; } }
                if (!isInList) { id = nextId; nextId++; }
                List.Add(this);
            }
        }

        public new int OrganizationId
        {
            get
            {
                foreach (Organization obj in V0.Organization.List) { if (obj.TeamIds.Contains(OldId)) { return obj.Id; } }
                if (V0.Organization.List.Count > 0) { return V0.Organization.List[0].Id; }
                else { return GlobalValues.NoId; }
            }
        }
    }
}
