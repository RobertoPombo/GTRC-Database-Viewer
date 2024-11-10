using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V1
{
    public class Event : GTRC_Basics.Models.Event
    {
        public static List<Event> List = [];

        private int id = GlobalValues.NoId;

        public new int Id
        {
            get { return id; }
            set
            {
                id = value;
                bool isInList = false;
                foreach (Event obj in List)
                {
                    if (obj.Id == value) { isInList = true; break; }
                }
                if (!isInList) { List.Add(this); }
            }
        }

        public new ResultsCombinationType PrequalifyingCombinationType
        {
            get { return ResultsCombinationType.Average; }
            set { }
        }

        public short AmbientTemp { get; set; }
        public byte CloudLevel { get; set; } = byte.MinValue;
        public byte RainLevel { get; set; } = byte.MinValue;
        public byte WeatherRandomness { get; set; } = byte.MinValue;
        public bool FixedConditions { get; set; } = false;
    }
}
