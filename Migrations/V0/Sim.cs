namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Sim : GTRC_Basics.Models.Sim
    {
        public static List<Sim> List = [];

        private int id = GTRC_Basics.GlobalValues.NoId;

        public new int Id
        {
            get { return id; }
            set
            {
                id = value;
                bool isInList = false;
                foreach (Sim obj in List)
                {
                    if (obj.Id == value) { isInList = true; break; }
                }
                if (!isInList) { List.Add(this); }
            }
        }
    }
}
