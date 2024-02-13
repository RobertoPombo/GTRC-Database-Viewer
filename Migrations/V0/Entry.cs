using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
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

        private int teamId = GlobalValues.NoId;
        public new int TeamId
        {
            get { return teamId; }
            set { foreach (Team t in V0.Team.List) { if (t.OldId == value) { teamId = t.Id; break; } } }
        }
        public int Ballast { set { BallastKg = (short)Math.Min(value, short.MaxValue); } }
        public bool ScorePoints { set { IsPointScorer = value; } }
        public bool Permanent { set { IsPermanent = value; } }
    }
}
