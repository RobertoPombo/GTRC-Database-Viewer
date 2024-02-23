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
                if (AccCarId == 50) { return "Alpine A110 GT4"; }
                else if (AccCarId == 12) { return "AMR V12 Vantage GT3"; }
                else if (AccCarId == 20) { return "AMR V8 Vantage"; }
                else if (AccCarId == 51) { return "Aston Martin Vantage GT4"; }
                else if (AccCarId == 3) { return "Audi R8 LMS"; }
                else if (AccCarId == 19) { return "Audi R8 LMS Evo"; }
                else if (AccCarId == 31) { return "Audi R8 LMS GT3 evo II"; }
                else if (AccCarId == 52) { return "Audi R8 LMS GT4"; }
                else if (AccCarId == 8) { return "Bentley Continental"; }
                else if (AccCarId == 11) { return "Bentley Continental (2015)"; }
                else if (AccCarId == 27) { return "BMW M2 CS Racing"; }
                else if (AccCarId == 30) { return "BMW M4 GT3"; }
                else if (AccCarId == 53) { return "BMW M4 GT4"; }
                else if (AccCarId == 7) { return "BMW M6 GT3"; }
                else if (AccCarId == 55) { return "Chevrolet Camaro GT4"; }
                else if (AccCarId == 14) { return "Emil Frey Jaguar G3"; }
                else if (AccCarId == 32) { return "Ferrari 296 GT3"; }
                else if (AccCarId == 26) { return "Ferrari 488 Challenge Evo"; }
                else if (AccCarId == 2) { return "Ferrari 488 GT3"; }
                else if (AccCarId == 24) { return "Ferrari 488 GT3 Evo"; }
                else if (AccCarId == 56) { return "Ginetta G55 GT4"; }
                else if (AccCarId == 17) { return "Honda NSX GT3"; }
                else if (AccCarId == 21) { return "Honda NSX GT3 Evo"; }
                else if (AccCarId == 57) { return "KTM X-Bow GT4"; }
                else if (AccCarId == 4) { return "Lamborghini Huracan GT3"; }
                else if (AccCarId == 16) { return "Lamborghini Huracan GT3 Evo"; }
                else if (AccCarId == 33) { return "Lamborghini Huracan GT3 EVO 2"; }
                else if (AccCarId == 29) { return "Lamborghini Huracán Super Trofeo"; }
                else if (AccCarId == 15) { return "Lexus RC F GT3"; }
                else if (AccCarId == 58) { return "Maserati MC GT4"; }
                else if (AccCarId == 59) { return "McLaren 570S GT4"; }
                else if (AccCarId == 5) { return "McLaren 650S GT3"; }
                else if (AccCarId == 22) { return "McLaren 720S GT3"; }
                else if (AccCarId == 35) { return "McLaren 720S GT3 Evo"; }
                else if (AccCarId == 1) { return "Mercedes AMG GT4"; }
                else if (AccCarId == 25) { return "Mercedes-AMG GT3"; }
                else if (AccCarId == 60) { return "Mercedes-AMG GT3 (2015)"; }
                else if (AccCarId == 6) { return "Nissan GT-R Nismo GT3"; }
                else if (AccCarId == 10) { return "Nissan GT-R Nismo GT3 (2015)"; }
                else if (AccCarId == 61) { return "Porsche 718 Cayman GT4 Clubsport"; }
                else if (AccCarId == 28) { return "Porsche 911 GT3 Cup (Type 992)"; }
                else if (AccCarId == 0) { return "Porsche 991 GT3 R"; }
                else if (AccCarId == 9) { return "Porsche 991 II GT3 Cup"; }
                else if (AccCarId == 23) { return "Porsche 991II GT3 R"; }
                else if (AccCarId == 34) { return "Porsche 992 GT3 R"; }
                else if (AccCarId == 13) { return "Reiter Engineering R-EX GT3"; }
                else { return string.Empty; }
            }
        }
    }
}
