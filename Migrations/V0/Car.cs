namespace GTRC_Basics.Migrations.V0
{
    public class Car
    {
        public int Id { get; set; }
        public uint AccCarId { get; set; } = uint.MinValue;
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public CarClass Class { get; set; } = CarClass.General;
        public ushort Year { get; set; } = (ushort)DateTime.UtcNow.Year;
        public DateOnly ReleaseDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public ushort WidthMm { get; set; } = 2000;
        public ushort LengthMm { get; set; } = 5000;
        public string NameGtrc { get; set; } = string.Empty;

        public DateTime releaseDate { set { ReleaseDate = DateOnly.FromDateTime(value); } }
        public string Category { set { if (CarClass.TryParse(value, out CarClass carClass)) { Class = carClass; } } }
        public string Name_GTRC { set { NameGtrc = value; } }
    }
}
