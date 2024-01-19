using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Series : GTRC_Basics.Models.Series
    {
        private int simId;

        public new int CommunityId { get { if (V0.Community.List.Count > 0) { return V0.Community.List[0].Id; } else { return GlobalValues.NoId; } } }

        public new int SimId
        {
            get
            {
                foreach (Sim obj in V0.Sim.List) { if (obj.Name == "Assetto Corsa Competizione") { return obj.Id; } }
                return simId;
            }
            set { simId = value; }
        }
    }
}
