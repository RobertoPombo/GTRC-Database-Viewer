namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Car : GTRC_Basics.Models.Car
    {
        public string manufacturer
        {
            set
            {
                if (V0.Manufacturer.List.Count > 0) { ManufacturerId = V0.Manufacturer.List[0].Id; }
                foreach (Manufacturer _manufacturer in V0.Manufacturer.List) { if (_manufacturer.Name == value) { ManufacturerId = _manufacturer.Id; break; } }
            }
        }

        public DateTime releaseDate { set { ReleaseDate = DateOnly.FromDateTime(value); } }

        public string Category { set { if (Enum.TryParse(value, out GTRC_Basics.CarClass carClass)) { Class = carClass; } } }
    }
}
