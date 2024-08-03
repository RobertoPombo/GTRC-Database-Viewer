using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
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

        private DateTime date = DateTime.UtcNow;
        public new DateTime Date
        {
            get { return date; }
            set
            {
                if (value > GlobalValues.DateTimeMaxValue) { date = GlobalValues.DateTimeMaxValue; }
                else if (value < new DateTime(DateTime.MinValue.Year + 1801, 1, 1, 0, 0, 0, 0, DateTime.MinValue.Kind)) { date = GlobalValues.DateTimeMinValue; }
                else { date = value; }
            }
        }

        public new ushort SessionOvertimeSeconds { get { return 140; } set { } }
        public new ushort PreRaceWaitingTimeSeconds { get { return 120; } set { } }
        public new ushort PostQualiSeconds { get { return 30; } set { } }
        public new ushort PostRaceSeconds { get { return 140; } set { } }
    }
}
