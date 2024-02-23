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

        public new string NameLfm
        {
            get
            {
                if (AccCarId == 31) { return "Audi R8 LMS GT3 evo II"; }
                else if (AccCarId == 8) { return "Bentley Continental"; }
                else if (AccCarId == 20) { return "AMR V8 Vantage"; }
                else if (AccCarId == 30) { return "BMW M4 GT3"; }
                else if (AccCarId == 32) { return "Ferrari 296 GT3"; }
                else if (AccCarId == 21) { return "Honda NSX GT3 Evo"; }
                else if (AccCarId == 33) { return "Lamborghini Huracan GT3 EVO 2"; }
                else if (AccCarId == 35) { return "McLaren 720S GT3 Evo"; }
                else if (AccCarId == 25) { return "Mercedes-AMG GT3"; }
                else if (AccCarId == 34) { return "Porsche 992 GT3 R"; }
                else if (AccCarId == 6) { return "Nissan GT-R Nismo GT3"; }
                else if (AccCarId == 24) { return "Ferrari 488 GT3 Evo"; }
                else { return string.Empty; }
            }
        }
    }
}
