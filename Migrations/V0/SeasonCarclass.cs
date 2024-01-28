namespace GTRC_Database_Viewer.Migrations.V0
{
    public class SeasonCarclass : GTRC_Basics.Models.SeasonCarclass
    {
        public int id { set { Id = value; SeasonId = value; CarclassId = 3; MaxGridSlots = byte.MaxValue; } }
    }
}
