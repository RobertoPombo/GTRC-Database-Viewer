namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Carclass : GTRC_Basics.Models.Carclass
    {
        public static List<Carclass> List = [
            new Carclass() { Id = 1, Name = "General" },
            new Carclass() { Id = 2, Name = "GT2" },
            new Carclass() { Id = 3, Name = "GT3" },
            new Carclass() { Id = 4, Name = "GT4" },
            new Carclass() { Id = 5, Name = "GTC" },
            new Carclass() { Id = 6, Name = "Cup" },
            new Carclass() { Id = 7, Name = "TCX" }
        ];
    }
}
