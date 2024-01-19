namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Community : GTRC_Basics.Models.Community
    {
        public static List<Community> List = [];

        private int id = GTRC_Basics.GlobalValues.NoId;

        public new int Id
        {
            get { return id; }
            set
            {
                id = value;
                bool isInList = false;
                foreach (Community obj in List)
                {
                    if (obj.Id == value) { isInList = true; break; }
                }
                if (!isInList) { List.Add(this); }
            }
        }
    }
}
