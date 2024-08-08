using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Season : GTRC_Basics.Models.Season
    {
        public new byte MinEntriesPerTeam { get { return 1; } }
        public new byte MaxEntriesPerTeam { get { return 2; } }
        public int carLimitBallast { set { CarLimitBallast = (byte)Math.Min(value, byte.MaxValue); } }
        public int carLimitRestrictor { set { CarLimitRestrictor = (byte)Math.Min(value, byte.MaxValue); } }
        public int carLimitRegisterLimit { set { CarRegistrationLimit = (byte)Math.Min(value, byte.MaxValue); } }
        public bool GroupCarLimits { set { GroupCarRegistrationLimits = value; } }
        public ushort DaysIgnoreCarLimits { set { DaysIgnoreCarRegistrationLimit = value; } }
        public int NoShowLimit { set { MaxNoShows = (byte)Math.Min(value, byte.MaxValue); } }
        public int SignOutLimit { set { MaxSignOuts = (byte)Math.Min(value, byte.MaxValue); } }

        private DateTime dateStartRegistration = DateTime.UtcNow;
        private DateTime dateEndRegistration = DateTime.UtcNow;
        private DateTime dateBoPFreeze = DateTime.UtcNow;
        public new DateTime DateStartRegistration
        {
            get { return dateStartRegistration; }
            set
            {
                if (value > GlobalValues.DateTimeMaxValue) { dateStartRegistration = GlobalValues.DateTimeMaxValue; }
                else if (value < new DateTime(DateTime.MinValue.Year + 1801, 1, 1, 0, 0, 0, 0, DateTime.MinValue.Kind)) { dateStartRegistration = GlobalValues.DateTimeMinValue; }
                else { dateStartRegistration = value.ToUniversalTime(); }
            }
        }
        public new DateTime DateEndRegistration
        {
            get { return dateEndRegistration; }
            set
            {
                if (value > GlobalValues.DateTimeMaxValue) { dateEndRegistration = GlobalValues.DateTimeMaxValue; }
                else if (value < new DateTime(DateTime.MinValue.Year + 1801, 1, 1, 0, 0, 0, 0, DateTime.MinValue.Kind)) { dateEndRegistration = GlobalValues.DateTimeMinValue; }
                else { dateEndRegistration = value.ToUniversalTime(); }
            }
        }
        public DateTime DateRegisterLimit
        {
            get { return DateStartCarRegistrationLimit; }
            set
            {
                if (value > GlobalValues.DateTimeMaxValue) { DateStartCarRegistrationLimit = GlobalValues.DateTimeMaxValue; }
                else if (value < new DateTime(DateTime.MinValue.Year + 1801, 1, 1, 0, 0, 0, 0, DateTime.MinValue.Kind)) { DateStartCarRegistrationLimit = GlobalValues.DateTimeMinValue; }
                else { DateStartCarRegistrationLimit = value.ToUniversalTime(); }
            }
        }
        public DateTime DateCarChangeLimit
        {
            get { return DateStartCarChangeLimit; }
            set
            {
                if (value > GlobalValues.DateTimeMaxValue) { DateStartCarChangeLimit = GlobalValues.DateTimeMaxValue; }
                else if (value < new DateTime(DateTime.MinValue.Year + 1801, 1, 1, 0, 0, 0, 0, DateTime.MinValue.Kind)) { DateStartCarChangeLimit = GlobalValues.DateTimeMinValue; }
                else { DateStartCarChangeLimit = value.ToUniversalTime(); }
            }
        }
        public new DateTime DateBoPFreeze
        {
            get { return dateBoPFreeze; }
            set
            {
                if (value > GlobalValues.DateTimeMaxValue) { dateBoPFreeze = GlobalValues.DateTimeMaxValue; }
                else if (value < new DateTime(DateTime.MinValue.Year + 1801, 1, 1, 0, 0, 0, 0, DateTime.MinValue.Kind)) { dateBoPFreeze = GlobalValues.DateTimeMinValue; }
                else { dateBoPFreeze = value.ToUniversalTime(); }
            }
        }

        public new ulong DiscordDriverRoleId { get { return 999414174381789184; } }

        public new ulong DiscordRegistrationChannelId { get { return 1267807353458266112; return 1008453364448768070; } }

        public new ulong DiscordTrackReportChannelId { get { return 999415000005365880; return 1004802942190243901; } }
    }
}
