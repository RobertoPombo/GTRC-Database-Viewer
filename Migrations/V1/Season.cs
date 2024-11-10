using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V1
{
    public class Season : GTRC_Basics.Models.Season
    {
        public static List<Season> List = [];

        private int id = GlobalValues.NoId;

        public new int Id
        {
            get { return id; }
            set
            {
                id = value;
                bool isInList = false;
                foreach (Season obj in List)
                {
                    if (obj.Id == value) { isInList = true; break; }
                }
                if (!isInList) { List.Add(this); }
            }
        }

        public new DateTime DateStartResetPointsForCarChange
        {
            get { return DateEndAutoGenerateRaceNumbers; }
            set { }
        }
    }
}
