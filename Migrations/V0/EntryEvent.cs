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
    }
}
