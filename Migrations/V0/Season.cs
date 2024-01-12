namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Season : GTRC_Basics.Models.Season
    {
        public int gridSlotsLimit { set { GridSlotsLimit = (byte)Math.Min(value, byte.MaxValue); } }
        public int carLimitBallast { set { CarLimitBallast = (byte)Math.Min(value, byte.MaxValue); } }
        public int carLimitRestrictor { set { CarLimitRestrictor = (byte)Math.Min(value, byte.MaxValue); } }
        public int carLimitRegisterLimit { set { CarLimitRegisterLimit = (byte)Math.Min(value, byte.MaxValue); } }
        public int noShowLimit { set { NoShowLimit = (byte)Math.Min(value, byte.MaxValue); } }
        public int signOutLimit { set { SignOutLimit = (byte)Math.Min(value, byte.MaxValue); } }
    }
}
