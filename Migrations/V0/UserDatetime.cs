using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class UserDatetime : GTRC_Basics.Models.UserDatetime
    {
        private static int nextId = GlobalValues.Id0;
        private int id = GlobalValues.NoId;
        public new int Id { get { return id; } set { if (value == GlobalValues.Id0) { nextId = GlobalValues.Id0; } id = nextId; nextId++; } }

        public int DriverID { set { UserId = value; } }

        private DateTime date = DateTime.UtcNow;
        public new DateTime Date
        {
            get { return date; }
            set
            {
                if (value > GlobalValues.DateTimeMaxValue) { date = GlobalValues.DateTimeMaxValue; }
                else if (value < new DateTime(DateTime.MinValue.Year + 1801, 1, 1, 0, 0, 0, 0, DateTime.MinValue.Kind)) { date = GlobalValues.DateTimeMinValue; }
                else { date = value.ToUniversalTime(); }
            }
        }
    }
}
