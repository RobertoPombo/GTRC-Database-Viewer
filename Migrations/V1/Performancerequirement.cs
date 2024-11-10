

using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V1
{
    public class Performancerequirement : GTRC_Basics.Models.Performancerequirement
    {
        public static List<Performancerequirement> List = [];

        private int id = GlobalValues.NoId;

        public new int Id
        {
            get { return id; }
            set
            {
                id = value;
                bool isInList = false;
                foreach (Performancerequirement obj in List)
                {
                    if (obj.Id == value) { isInList = true; break; }
                }
                if (!isInList) { List.Add(this); }
            }
        }
    }
}
