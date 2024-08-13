namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Series : GTRC_Basics.Models.Series
    {
        private int simId;
        private string name = DefaultName;

        public new int SimId
        {
            get
            {
                foreach (Sim obj in V0.Sim.List) { if (obj.Name == "Assetto Corsa Competizione") { return obj.Id; } }
                return simId;
            }
            set { simId = value; }
        }

        public new string Name { get { return name; } set { name = value.Replace("GTRC ", ""); } }

        public new ulong DiscordDriverRoleId { get { return 999414174381789184; } }

        public new ulong DiscordLogChannelId { get { return 926787693227147274; return 1004802942190243901; } }

        public new ulong DiscordRegistrationChannelId { get { return 1267807353458266112; return 1008453364448768070; } }

        public new ulong DiscordTrackReportChannelId { get { return 999415000005365880; return 1004802942190243901; } }
    }
}
