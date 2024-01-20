namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Car : GTRC_Basics.Models.Car
    {
        public string manufacturer
        {
            set
            {
                if (V0.Manufacturer.List.Count > 0) { ManufacturerId = V0.Manufacturer.List[0].Id; }
                foreach (Manufacturer obj in V0.Manufacturer.List) { if (obj.Name == value) { ManufacturerId = obj.Id; break; } }
            }
        }

        public DateTime releaseDate { set { ReleaseDate = DateOnly.FromDateTime(value); } }

        public string Category
        {
            set
            {
                if (V0.Carclass.List.Count > 0) { CarclassId = V0.Carclass.List[0].Id; }
                foreach (Carclass obj in V0.Carclass.List) { if (obj.Name == value) { CarclassId = obj.Id; break; } }
            }
        }

        public string NameGtrc { set { NameGoogleSheets = value; } }
    }
}
