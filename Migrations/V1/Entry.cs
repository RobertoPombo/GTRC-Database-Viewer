using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V1
{
    public class Entry : GTRC_Basics.Models.Entry
    {
        public static List<Entry> List = [];

        private int id = GlobalValues.NoId;

        public new int Id
        {
            get { return id; }
            set
            {
                id = value;
                bool isInList = false;
                foreach (Entry obj in List)
                {
                    if (obj.Id == value) { isInList = true; break; }
                }
                if (!isInList) { List.Add(this); }
            }
        }
    }
}
