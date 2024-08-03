using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Session : GTRC_Basics.Models.Session
    {
        private int gridSessionId = GlobalValues.NoId;
        public int sessionType
        {
            set
            {
                if (value == 1) { SessionType = SessionType.Qualifying; }
                else if (value == 2) { SessionType = SessionType.Race; }
                else { SessionType = SessionType.Practice; }
            }
        }
        public bool AttendanceObligated { set { IsObligatedAttendance = value; } }
        public new int GridSessionId { set { if (value < GlobalValues.Id0) { gridSessionId = GlobalValues.NoId; } else { gridSessionId = value; } } get { return gridSessionId; } }
        public int reverseGridFrom { set { ReverseGridFrom = (byte)Math.Min(value, byte.MaxValue); } }
        public int reverseGridTo { set { ReverseGridTo = (byte)Math.Min(value, byte.MaxValue); } }
        public int IngameStartTime { set { IngameStartTimeHour = (byte)Math.Min(value, byte.MaxValue); } }
        public int dayOfWeekend { set { DayOfWeekend = (byte)Math.Min(value, byte.MaxValue); } }
        public int timeMultiplier { set { TimeMultiplier = (byte)Math.Min(value, byte.MaxValue); } }
        public int entrylistType
        {
            set
            {
                if (value == 1) { EntrylistType = EntrylistType.RaceControl; }
                else if (value == 2) { EntrylistType = EntrylistType.AllDrivers; }
                else if (value == 3) { EntrylistType = EntrylistType.Season; }
                else { EntrylistType = EntrylistType.None; }
            }
        }
    }
}
