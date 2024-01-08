namespace GTRC_Basics.Migrations.V0
{
    public class Track
    {
        public static readonly string DefaultAccTrackId = "trackid";
        public static readonly ushort DefaultAccTimePenDtS = 30;

        public int Id { get; set; }
        public string AccTrackId { get; set; } = DefaultAccTrackId;
        public string Name { get; set; } = string.Empty;
        public ushort PitBoxesCount { get; set; } = ushort.MinValue;
        public ushort ServerSlotsCount { get; set; } = ushort.MinValue;
        public ushort AccTimePenDtS { get; set; } = DefaultAccTimePenDtS;
        public string NameGtrc { get; set; } = string.Empty;

        public int AccTimePenDT { set { AccTimePenDtS = (ushort)value; } }
        public string Name_GTRC { set { NameGtrc = value; } }
    }
}
