namespace GTRC_Database_Viewer.Migrations.V0
{
    public class EventCarclass : GTRC_Basics.Models.EventCarclass
    {
        public int id { set { Id = value; EventId = value; CarclassId = 3; MaxGridSlots = byte.MaxValue; } }
    }
}
