namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Season : GTRC_Basics.Models.Season
    {
        public new byte MinEntriesPerTeam { get { return 2; } }
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
        public byte GridSlotsLimit { set { MaxGridSlots = value; } }
        public byte NoShowLimit { set { MaxNoShows = value; } }
        public byte SignOutLimit { set { MaxSignOuts = value; } }
    }
}
