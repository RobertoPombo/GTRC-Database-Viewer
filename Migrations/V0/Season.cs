using GTRC_Basics.Models;

namespace GTRC_Basics.Migrations.V0
{
    public class Season
    {
        public static readonly string DefaultName = "Season #1";

        public int Id { get; set; }
        public string Name { get; set; } = DefaultName;
        public Series Series { get; set; } = new();
        public Byte MinDriversPerEntry { get; set; } = Byte.MinValue;
        public Byte MaxDriversPerEntry { get; set; } = Byte.MaxValue;
        public Byte GridSlotsLimit { get; set; } = Byte.MinValue;
        public Byte CarLimitBallast { get; set; } = Byte.MinValue;
        public Byte GainBallast { get; set; } = Byte.MinValue;
        public Byte CarLimitRestrictor { get; set; } = Byte.MinValue;
        public Byte GainRestrictor { get; set; } = Byte.MinValue;
        public Byte CarLimitRegisterLimit { get; set; } = Byte.MinValue;
        public DateTime DateRegisterLimit { get; set; } = DateTime.UtcNow;
        public DateTime DateBoPFreeze { get; set; } = DateTime.UtcNow;
        public Byte NoShowLimit { get; set; } = Byte.MinValue;
        public Byte SignOutLimit { get; set; } = Byte.MinValue;
        public Byte CarChangeLimit { get; set; } = Byte.MinValue;
        public DateTime DateCarChangeLimit { get; set; } = DateTime.UtcNow;
        public bool GroupCarLimits { get; set; } = false;
        public bool BopLatestModelOnly { get; set; } = false;
        public ushort DaysIgnoreCarLimits { get; set; } = 0;
        public FormationLapType FormationLapType { get; set; } = FormationLapType.Manual;
        public string ShortDescription { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
