using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class EntryEvent : GTRC_Basics.Models.EntryEvent
    {
        private static int nextId = GlobalValues.Id0;
        private int id = GlobalValues.NoId;
        public new int Id { get { return id; } set { if (value == GlobalValues.Id0) { nextId = GlobalValues.Id0; } id = nextId; nextId++; } }

        public bool Attended { set { DidAttend = value; } }
        public short Ballast { set { BallastKg = value; } }
        public bool ScorePoints { set { IsPointScorer = value; } }

        private DateTime signInDate = GlobalValues.DateTimeMaxValue;
        public new DateTime SignInDate
        {
            get { return signInDate; }
            set
            {
                if (value > GlobalValues.DateTimeMaxValue) { signInDate = GlobalValues.DateTimeMaxValue; }
                else if (value < new DateTime(DateTime.MinValue.Year + 1801, 1, 1, 0, 0, 0, 0, DateTime.MinValue.Kind)) { signInDate = GlobalValues.DateTimeMinValue; }
                else { signInDate = value.ToUniversalTime(); }
            }
        }
    }
}
