namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Track : GTRC_Basics.Models.Track
    {
        public int AccTimePenDT { set { AccTimePenDtS = (ushort)value; } }
    }
}
