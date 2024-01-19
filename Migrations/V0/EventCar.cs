namespace GTRC_Database_Viewer.Migrations.V0
{
    public class EventCar : GTRC_Basics.Models.EventCar
    {
        public static List<Sim> List = [];

        private int id = GTRC_Basics.GlobalValues.NoId;

        public int Ballast { set { BallastKg = (byte)Math.Min(value, byte.MaxValue); } }
    }
}
