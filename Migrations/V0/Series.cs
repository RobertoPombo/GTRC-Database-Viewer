namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Series : GTRC_Basics.Models.Series
    {
        private int simId;
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
