using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Car : GTRC_Basics.Models.Car
    {
        public DateTime releaseDate { set { ReleaseDate = DateOnly.FromDateTime(value); } }
        public string Category { set { if (CarClass.TryParse(value, out CarClass carClass)) { Class = carClass; } } }
    }
}
