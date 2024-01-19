namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Season : GTRC_Basics.Models.Season
    {
        public new Byte MinEntriesPerTeam { get { return 2; } }
        public int gridSlotsLimit { set { GridSlotsLimit = (byte)Math.Min(value, byte.MaxValue); } }
        public int carLimitBallast { set { CarLimitBallast = (byte)Math.Min(value, byte.MaxValue); } }
        public int carLimitRestrictor { set { CarLimitRestrictor = (byte)Math.Min(value, byte.MaxValue); } }
        public int carLimitRegisterLimit { set { CarRegristrationLimit = (byte)Math.Min(value, byte.MaxValue); } }
        public int noShowLimit { set { NoShowLimit = (byte)Math.Min(value, byte.MaxValue); } }
        public int signOutLimit { set { SignOutLimit = (byte)Math.Min(value, byte.MaxValue); } }
        public DateTime DateRegisterLimit { set { DateStartCarRegristrationLimit = value; } }
        public DateTime DateCarChangeLimit { set { DateStartCarChangeLimit = value; } }
        public bool GroupCarLimits { set { GroupCarRegristrationLimits = value; } }
        public ushort DaysIgnoreCarLimits { set { DaysIgnoreCarRegristrationLimit = value; } }
        public Byte GridSlotsLimit { set { MaxGridSlots = value; } }
        public Byte NoShowLimit { set { MaxNoShows = value; } }
        public Byte SignOutLimit { set { MaxSignOuts = value; } }
    }
}
