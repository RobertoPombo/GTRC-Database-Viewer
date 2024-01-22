using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Entry : GTRC_Basics.Models.Entry
    {
        private int teamId = GlobalValues.NoId;
        public new int TeamId
        {
            get { return teamId; }
            set { foreach (Team t in V0.Team.List) { if (t.OldId == value) { teamId = t.Id; break; } } }
        }
        public int Ballast { set { BallastKg = (short)Math.Min(value, short.MaxValue); } }
        public int Category { set { AccDriverCategory = (AccDriverCategory)value; } }
        public bool Permanent { set { IsPermanent = value; } }
    }
}
